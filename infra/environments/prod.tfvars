environment          = "prod"
location             = "eastus2"
project_name         = "eshop"
kubernetes_version   = "1.28"
system_node_count    = 3
windows_node_count   = 3
system_node_vm_size  = "Standard_DS4_v2"
windows_node_vm_size = "Standard_DS4_v2"
db_sku_name          = "S3"

private_cluster_enabled = true

tags = {
  CostCenter = "production"
}
