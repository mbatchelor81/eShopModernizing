output "key_vault_uri" {
  description = "Key Vault URI"
  value       = azurerm_key_vault.main.vault_uri
}

output "key_vault_id" {
  description = "Key Vault resource ID"
  value       = azurerm_key_vault.main.id
}

output "sql_admin_password" {
  description = "Generated SQL admin password"
  value       = random_password.sql_admin.result
  sensitive   = true
}
