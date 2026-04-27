variable "resource_group_name" {
  description = "Name of the resource group"
  type        = string
}

variable "location" {
  description = "Azure region"
  type        = string
}

variable "resource_prefix" {
  description = "Prefix for resource names"
  type        = string
}

variable "sku_name" {
  description = "SKU name for the SQL Database"
  type        = string
  default     = "S0"
}

variable "admin_login" {
  description = "Administrator login for SQL Server"
  type        = string
  default     = "sqladmin"
}

variable "subnet_id" {
  description = "Subnet ID for virtual network rule"
  type        = string
}

variable "key_vault_id" {
  description = "Key Vault ID for storing the connection string"
  type        = string
}

variable "tags" {
  description = "Tags applied to all resources"
  type        = map(string)
  default     = {}
}
