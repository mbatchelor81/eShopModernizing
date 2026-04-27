terraform {
  backend "azurerm" {
    # Configure via -backend-config or environment variables:
    #   ARM_ACCESS_KEY, container_name, storage_account_name, key
    # Example:
    #   terraform init \
    #     -backend-config="storage_account_name=eshoptfstate" \
    #     -backend-config="container_name=tfstate" \
    #     -backend-config="key=eshop.terraform.tfstate" \
    #     -backend-config="resource_group_name=eshop-tfstate-rg"
  }
}

locals {
  resource_prefix = "${var.project_name}-${var.environment}"
  common_tags = merge(var.tags, {
    Environment = var.environment
    Project     = var.project_name
    ManagedBy   = "terraform"
  })
}

resource "azurerm_resource_group" "main" {
  name     = "${local.resource_prefix}-rg"
  location = var.location
  tags     = local.common_tags
}

module "networking" {
  source = "./modules/networking"

  resource_group_name = azurerm_resource_group.main.name
  location            = azurerm_resource_group.main.location
  resource_prefix     = local.resource_prefix
  vnet_address_space  = var.vnet_address_space
  aks_subnet_prefix   = var.aks_subnet_prefix
  db_subnet_prefix    = var.db_subnet_prefix
  tags                = local.common_tags
}

module "container_registry" {
  source = "./modules/container-registry"

  resource_group_name = azurerm_resource_group.main.name
  location            = azurerm_resource_group.main.location
  resource_prefix     = local.resource_prefix
  tags                = local.common_tags
}

module "secrets" {
  source = "./modules/secrets"

  resource_group_name = azurerm_resource_group.main.name
  location            = azurerm_resource_group.main.location
  resource_prefix     = local.resource_prefix
  tags                = local.common_tags
}

module "database" {
  source = "./modules/database"

  resource_group_name = azurerm_resource_group.main.name
  location            = azurerm_resource_group.main.location
  resource_prefix     = local.resource_prefix
  sku_name            = var.db_sku_name
  admin_login         = var.db_admin_login
  subnet_id           = module.networking.db_subnet_id
  key_vault_id        = module.secrets.key_vault_id
  tags                = local.common_tags
}

module "kubernetes_cluster" {
  source = "./modules/kubernetes-cluster"

  resource_group_name     = azurerm_resource_group.main.name
  location                = azurerm_resource_group.main.location
  resource_prefix         = local.resource_prefix
  kubernetes_version      = var.kubernetes_version
  system_node_count       = var.system_node_count
  system_node_vm_size     = var.system_node_vm_size
  windows_node_count      = var.windows_node_count
  windows_node_vm_size    = var.windows_node_vm_size
  subnet_id               = module.networking.aks_subnet_id
  acr_id                  = module.container_registry.acr_id
  private_cluster_enabled = var.private_cluster_enabled
  tags                    = local.common_tags
}
