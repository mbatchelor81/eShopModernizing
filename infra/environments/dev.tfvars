environment          = "dev"
location             = "eastus"
project_name         = "eshop"
kubernetes_version   = "1.28"
linux_node_count     = 1
linux_node_vm_size   = "Standard_DS2_v2"
windows_node_count   = 1
windows_node_vm_size = "Standard_DS2_v2"
sql_sku              = "S0"

tags = {
  cost_center = "development"
}
