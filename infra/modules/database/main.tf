resource "azurerm_mssql_server" "main" {
  name                         = "sql-${var.name_prefix}"
  resource_group_name          = var.resource_group_name
  location                     = var.location
  version                      = "12.0"
  administrator_login          = var.sql_admin_username
  administrator_login_password = var.sql_admin_password
  minimum_tls_version          = "1.2"
  tags                         = var.tags

  azuread_administrator {
    login_username = "AzureAD Admin"
    object_id      = data.azuread_client_config.current.object_id
  }
}

data "azuread_client_config" "current" {}

resource "azurerm_mssql_database" "catalog" {
  name      = "CatalogDb"
  server_id = azurerm_mssql_server.main.id
  sku_name  = var.sql_sku
  tags      = var.tags

  short_term_retention_policy {
    retention_days = 7
  }
}

resource "azurerm_mssql_virtual_network_rule" "aks" {
  name      = "aks-vnet-rule"
  server_id = azurerm_mssql_server.main.id
  subnet_id = var.subnet_id
}

resource "azurerm_mssql_firewall_rule" "allow_azure" {
  name             = "AllowAzureServices"
  server_id        = azurerm_mssql_server.main.id
  start_ip_address = "0.0.0.0"
  end_ip_address   = "0.0.0.0"
}
