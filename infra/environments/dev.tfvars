environment          = "dev"
location             = "eastus2"
project_name         = "eshop"
kubernetes_version   = "1.28"
system_node_count    = 1
windows_node_count   = 1
system_node_vm_size  = "Standard_DS2_v2"
windows_node_vm_size = "Standard_DS3_v2"
db_sku_name          = "S0"

private_cluster_enabled = false

tags = {
  CostCenter = "development"
}
