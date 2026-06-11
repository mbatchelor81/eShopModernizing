output "server_fqdn" {
  description = "SQL Server fully qualified domain name"
  value       = azurerm_mssql_server.main.fully_qualified_domain_name
}

output "database_name" {
  description = "SQL Database name"
  value       = azurerm_mssql_database.catalog.name
}

output "server_id" {
  description = "SQL Server resource ID"
  value       = azurerm_mssql_server.main.id
}
