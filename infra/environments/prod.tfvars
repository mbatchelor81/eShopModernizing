environment          = "prod"
location             = "eastus"
project_name         = "eshop"
kubernetes_version   = "1.28"
linux_node_count     = 3
linux_node_vm_size   = "Standard_DS3_v2"
windows_node_count   = 3
windows_node_vm_size = "Standard_DS3_v2"
sql_sku              = "S2"

tags = {
  cost_center = "production"
}
