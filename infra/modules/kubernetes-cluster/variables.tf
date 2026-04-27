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

variable "kubernetes_version" {
  description = "Kubernetes version"
  type        = string
}

variable "system_node_count" {
  description = "Number of Linux system pool nodes"
  type        = number
}

variable "system_node_vm_size" {
  description = "VM size for Linux system pool"
  type        = string
}

variable "windows_node_count" {
  description = "Number of Windows node pool nodes"
  type        = number
}

variable "windows_node_vm_size" {
  description = "VM size for Windows node pool"
  type        = string
}

variable "subnet_id" {
  description = "Subnet ID for AKS nodes"
  type        = string
}

variable "acr_id" {
  description = "Container registry resource ID for pull access"
  type        = string
}

variable "private_cluster_enabled" {
  description = "Whether to enable private API endpoint"
  type        = bool
  default     = false
}

variable "tags" {
  description = "Tags applied to all resources"
  type        = map(string)
  default     = {}
}
