variable "resource_group_name" {
  description = "Name of the resource group"
  type        = string
}

variable "location" {
  description = "Azure region"
  type        = string
}

variable "name_prefix" {
  description = "Naming prefix for resources"
  type        = string
}

variable "kubernetes_version" {
  description = "Kubernetes version"
  type        = string
}

variable "linux_node_count" {
  description = "Number of Linux system pool nodes"
  type        = number
}

variable "linux_node_vm_size" {
  description = "VM size for Linux nodes"
  type        = string
}

variable "windows_node_count" {
  description = "Number of Windows node pool nodes"
  type        = number
}

variable "windows_node_vm_size" {
  description = "VM size for Windows nodes"
  type        = string
}

variable "subnet_id" {
  description = "Subnet ID for AKS nodes"
  type        = string
}

variable "acr_id" {
  description = "Azure Container Registry ID for pull permissions"
  type        = string
}

variable "tags" {
  description = "Tags to apply to resources"
  type        = map(string)
  default     = {}
}
