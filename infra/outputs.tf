output "cluster_endpoint" {
  description = "AKS cluster API server endpoint"
  value       = module.kubernetes_cluster.cluster_endpoint
}

output "cluster_name" {
  description = "AKS cluster name"
  value       = module.kubernetes_cluster.cluster_name
}

output "registry_url" {
  description = "Container registry login server URL"
  value       = module.container_registry.login_server
}

output "db_fqdn" {
  description = "SQL Server fully qualified domain name"
  value       = module.database.server_fqdn
}

output "db_name" {
  description = "SQL Database name"
  value       = module.database.database_name
}

output "key_vault_name" {
  description = "Key Vault name"
  value       = module.secrets.key_vault_name
}

output "key_vault_uri" {
  description = "Key Vault URI"
  value       = module.secrets.key_vault_uri
}

output "resource_group_name" {
  description = "Resource group name"
  value       = azurerm_resource_group.main.name
}
