output "cluster_endpoint" {
  description = "AKS cluster API server endpoint"
  value       = module.kubernetes_cluster.cluster_endpoint
}

output "cluster_name" {
  description = "AKS cluster name"
  value       = module.kubernetes_cluster.cluster_name
}

output "registry_url" {
  description = "Azure Container Registry login server URL"
  value       = module.container_registry.registry_login_server
}

output "database_fqdn" {
  description = "Azure SQL Server fully qualified domain name"
  value       = module.database.sql_server_fqdn
}

output "key_vault_uri" {
  description = "Azure Key Vault URI"
  value       = module.secrets.key_vault_uri
}

output "resource_group_name" {
  description = "Name of the resource group"
  value       = azurerm_resource_group.main.name
}
