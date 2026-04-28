terraform {
  backend "azurerm" {
    # Configure via -backend-config or environment variables:
    #   ARM_ACCESS_KEY, or use managed identity
    # resource_group_name  = "tfstate-rg"
    # storage_account_name = "tfstateeshop"
    # container_name       = "tfstate"
    # key                  = "eshop.terraform.tfstate"
  }
}

locals {
  name_prefix = "${var.project_name}-${var.environment}"
  common_tags = merge(var.tags, {
    project     = var.project_name
    environment = var.environment
    managed_by  = "terraform"
  })
}

resource "azurerm_resource_group" "main" {
  name     = "rg-${local.name_prefix}"
  location = var.location
  tags     = local.common_tags
}

module "networking" {
  source = "./modules/networking"

  resource_group_name = azurerm_resource_group.main.name
  location            = azurerm_resource_group.main.location
  name_prefix         = local.name_prefix
  vnet_address_space  = var.vnet_address_space
  tags                = local.common_tags
}

module "container_registry" {
  source = "./modules/container-registry"

  resource_group_name = azurerm_resource_group.main.name
  location            = azurerm_resource_group.main.location
  name_prefix         = local.name_prefix
  tags                = local.common_tags
}

module "kubernetes_cluster" {
  source = "./modules/kubernetes-cluster"

  resource_group_name    = azurerm_resource_group.main.name
  location               = azurerm_resource_group.main.location
  name_prefix            = local.name_prefix
  kubernetes_version     = var.kubernetes_version
  linux_node_count       = var.linux_node_count
  linux_node_vm_size     = var.linux_node_vm_size
  windows_node_count     = var.windows_node_count
  windows_node_vm_size   = var.windows_node_vm_size
  subnet_id              = module.networking.aks_subnet_id
  acr_id                 = module.container_registry.registry_id
  tags                   = local.common_tags
}

module "database" {
  source = "./modules/database"

  resource_group_name  = azurerm_resource_group.main.name
  location             = azurerm_resource_group.main.location
  name_prefix          = local.name_prefix
  sql_admin_username   = var.sql_admin_username
  sql_admin_password   = module.secrets.sql_admin_password
  sql_sku              = var.sql_sku
  subnet_id            = module.networking.db_subnet_id
  tags                 = local.common_tags
}

module "secrets" {
  source = "./modules/secrets"

  resource_group_name = azurerm_resource_group.main.name
  location            = azurerm_resource_group.main.location
  name_prefix         = local.name_prefix
  tags                = local.common_tags
}
