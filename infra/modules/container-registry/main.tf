locals {
  # ACR names must be alphanumeric only
  acr_name = replace("${var.resource_prefix}acr", "-", "")
}

resource "azurerm_container_registry" "main" {
  name                = local.acr_name
  resource_group_name = var.resource_group_name
  location            = var.location
  sku                 = var.sku
  admin_enabled       = false
  tags                = var.tags

  # Image scanning is enabled by default on Standard/Premium SKUs via
  # Microsoft Defender for Containers. No explicit config needed.
}
