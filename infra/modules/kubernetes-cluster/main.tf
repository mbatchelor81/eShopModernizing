resource "azurerm_kubernetes_cluster" "main" {
  name                    = "${var.resource_prefix}-aks"
  resource_group_name     = var.resource_group_name
  location                = var.location
  dns_prefix              = "${var.resource_prefix}-aks"
  kubernetes_version      = var.kubernetes_version
  private_cluster_enabled = var.private_cluster_enabled
  tags                    = var.tags

  default_node_pool {
    name                = "system"
    node_count          = var.system_node_count
    vm_size             = var.system_node_vm_size
    os_sku              = "Ubuntu"
    vnet_subnet_id      = var.subnet_id
    enable_auto_scaling = false
    tags                = var.tags
  }

  identity {
    type = "SystemAssigned"
  }

  network_profile {
    network_plugin = "azure"
    network_policy = "calico"
    service_cidr   = "10.1.0.0/16"
    dns_service_ip = "10.1.0.10"
  }

  azure_active_directory_role_based_access_control {
    managed            = true
    azure_rbac_enabled = true
  }
}

resource "azurerm_kubernetes_cluster_node_pool" "windows" {
  name                  = "win"
  kubernetes_cluster_id = azurerm_kubernetes_cluster.main.id
  vm_size               = var.windows_node_vm_size
  node_count            = var.windows_node_count
  os_type               = "Windows"
  os_sku                = "Windows2022"
  vnet_subnet_id        = var.subnet_id
  tags                  = var.tags
}

resource "azurerm_role_assignment" "acr_pull" {
  principal_id                     = azurerm_kubernetes_cluster.main.kubelet_identity[0].object_id
  role_definition_name             = "AcrPull"
  scope                            = var.acr_id
  skip_service_principal_aad_check = true
}
