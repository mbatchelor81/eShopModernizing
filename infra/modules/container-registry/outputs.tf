output "registry_id" {
  description = "Container registry ID"
  value       = azurerm_container_registry.main.id
}

output "registry_login_server" {
  description = "Container registry login server URL"
  value       = azurerm_container_registry.main.login_server
}

output "registry_name" {
  description = "Container registry name"
  value       = azurerm_container_registry.main.name
}
