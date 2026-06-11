variable "environment" {
  description = "Deployment environment (dev, staging, prod)"
  type        = string
  validation {
    condition     = contains(["dev", "staging", "prod"], var.environment)
    error_message = "Environment must be one of: dev, staging, prod."
  }
}

variable "location" {
  description = "Azure region for all resources"
  type        = string
  default     = "eastus2"
}

variable "project_name" {
  description = "Project name used as prefix for resource naming"
  type        = string
  default     = "eshop"
}

variable "kubernetes_version" {
  description = "Kubernetes version for AKS cluster"
  type        = string
  default     = "1.28"
}

variable "system_node_count" {
  description = "Number of Linux system pool nodes"
  type        = number
  default     = 2
}

variable "windows_node_count" {
  description = "Number of Windows node pool nodes"
  type        = number
  default     = 1
}

variable "system_node_vm_size" {
  description = "VM size for Linux system pool"
  type        = string
  default     = "Standard_DS2_v2"
}

variable "windows_node_vm_size" {
  description = "VM size for Windows node pool"
  type        = string
  default     = "Standard_DS3_v2"
}

variable "db_sku_name" {
  description = "SKU name for Azure SQL Database"
  type        = string
  default     = "S0"
}

variable "db_admin_login" {
  description = "Administrator login for SQL Server"
  type        = string
  default     = "sqladmin"
}

variable "vnet_address_space" {
  description = "Address space for the virtual network"
  type        = list(string)
  default     = ["10.0.0.0/16"]
}

variable "aks_subnet_prefix" {
  description = "Address prefix for the AKS subnet"
  type        = string
  default     = "10.0.1.0/24"
}

variable "db_subnet_prefix" {
  description = "Address prefix for the database subnet"
  type        = string
  default     = "10.0.2.0/24"
}

variable "private_cluster_enabled" {
  description = "Whether to enable private API endpoint for AKS"
  type        = bool
  default     = false
}

variable "ci_runner_ip" {
  description = "CI runner public IP (CIDR) to allow Key Vault data plane access during provisioning"
  type        = string
  default     = ""
}

variable "tags" {
  description = "Tags applied to all resources"
  type        = map(string)
  default     = {}
}
