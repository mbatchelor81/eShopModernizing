output "sql_server_fqdn" {
  description = "SQL Server fully qualified domain name"
  value       = azurerm_mssql_server.main.fully_qualified_domain_name
}

output "sql_server_id" {
  description = "SQL Server resource ID"
  value       = azurerm_mssql_server.main.id
}

output "database_name" {
  description = "Catalog database name"
  value       = azurerm_mssql_database.catalog.name
}

output "connection_string" {
  description = "ADO.NET connection string for the catalog database"
  value       = "Server=tcp:${azurerm_mssql_server.main.fully_qualified_domain_name},1433;Initial Catalog=${azurerm_mssql_database.catalog.name};Persist Security Info=False;User ID=${var.sql_admin_username};Password=${var.sql_admin_password};MultipleActiveResultSets=True;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;"
  sensitive   = true
}
