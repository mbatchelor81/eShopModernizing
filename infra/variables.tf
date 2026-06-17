variable "environment" {
  description = "Deployment environment (dev, staging, prod)"
  type        = string
  default     = "dev"

  validation {
    condition     = contains(["dev", "staging", "prod"], var.environment)
    error_message = "Environment must be one of: dev, staging, prod."
  }
}

variable "location" {
  description = "Azure region for all resources"
  type        = string
  default     = "eastus"
}

variable "project_name" {
  description = "Project name used for resource naming"
  type        = string
  default     = "eshop"
}

variable "kubernetes_version" {
  description = "Kubernetes version for AKS cluster"
  type        = string
  default     = "1.28"
}

variable "linux_node_count" {
  description = "Number of Linux system pool nodes"
  type        = number
  default     = 1
}

variable "linux_node_vm_size" {
  description = "VM size for Linux system pool nodes"
  type        = string
  default     = "Standard_DS2_v2"
}

variable "windows_node_count" {
  description = "Number of Windows node pool nodes"
  type        = number
  default     = 1
}

variable "windows_node_vm_size" {
  description = "VM size for Windows node pool nodes"
  type        = string
  default     = "Standard_DS2_v2"
}

variable "sql_admin_username" {
  description = "Administrator username for managed SQL instance"
  type        = string
  default     = "sqladmin"
}

variable "sql_sku" {
  description = "SKU name for Azure SQL Database"
  type        = string
  default     = "S0"
}

variable "vnet_address_space" {
  description = "Address space for the virtual network"
  type        = list(string)
  default     = ["10.0.0.0/16"]
}

variable "tags" {
  description = "Tags applied to all resources"
  type        = map(string)
  default     = {}
}
