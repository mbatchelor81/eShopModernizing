output "vnet_id" {
  description = "Virtual network ID"
  value       = azurerm_virtual_network.main.id
}

output "aks_subnet_id" {
  description = "AKS subnet ID"
  value       = azurerm_subnet.aks.id
}

output "db_subnet_id" {
  description = "Database subnet ID"
  value       = azurerm_subnet.database.id
}
