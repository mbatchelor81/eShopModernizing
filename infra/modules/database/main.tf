resource "random_password" "sql_admin" {
  length  = 32
  special = true
}

resource "azurerm_mssql_server" "main" {
  name                         = "${var.resource_prefix}-sqlserver"
  resource_group_name          = var.resource_group_name
  location                     = var.location
  version                      = "12.0"
  administrator_login          = var.admin_login
  administrator_login_password = random_password.sql_admin.result
  minimum_tls_version          = "1.2"
  tags                         = var.tags

  azuread_administrator {
    login_username = "AzureAD Admin"
    object_id      = data.azuread_client_config.current.object_id
  }
}

data "azuread_client_config" "current" {}

resource "azurerm_mssql_database" "catalog" {
  name      = "eShopCatalogDb"
  server_id = azurerm_mssql_server.main.id
  sku_name  = var.sku_name
  tags      = var.tags
}

resource "azurerm_mssql_virtual_network_rule" "aks" {
  name      = "${var.resource_prefix}-aks-vnet-rule"
  server_id = azurerm_mssql_server.main.id
  subnet_id = var.subnet_id
}

resource "azurerm_key_vault_secret" "db_connection_string" {
  name         = "eshop-db-connection-string"
  value        = "Server=tcp:${azurerm_mssql_server.main.fully_qualified_domain_name},1433;Database=${azurerm_mssql_database.catalog.name};User ID=${var.admin_login};Password=${random_password.sql_admin.result};Encrypt=True;TrustServerCertificate=False;"
  key_vault_id = var.key_vault_id
}

resource "azurerm_key_vault_secret" "db_password" {
  name         = "eshop-db-password"
  value        = random_password.sql_admin.result
  key_vault_id = var.key_vault_id
}
