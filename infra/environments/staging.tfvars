environment          = "staging"
location             = "eastus2"
project_name         = "eshop"
kubernetes_version   = "1.28"
system_node_count    = 2
windows_node_count   = 2
system_node_vm_size  = "Standard_DS2_v2"
windows_node_vm_size = "Standard_DS3_v2"
db_sku_name          = "S1"

private_cluster_enabled = false

tags = {
  CostCenter = "staging"
}
