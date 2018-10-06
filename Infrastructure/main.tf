provider "azurerm" { }

# Create a resource group
resource "azurerm_resource_group" "EmailSentiment" {
  name     = "EmailSentiment${var.environment}"
  location = "australiaeast"
}
