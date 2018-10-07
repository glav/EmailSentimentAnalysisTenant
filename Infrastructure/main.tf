provider "azurerm" { }

# Create a resource group
resource "azurerm_resource_group" "EmailSentiment" {
  name     = "EmailSentiment${var.environment}"
  location = "${var.location}"
}

resource "azurerm_servicebus_namespace" "EmailSentiment" {
  name                = "${var.servicebus_name}"
  location            = "${var.location}"
  resource_group_name = "${azurerm_resource_group.EmailSentiment.name}"
  sku                 = "standard"

  tags {
    source = "terraform"
  }
}

resource "azurerm_servicebus_topic" "EmailSentiment" {
  name                = "${var.servicebus_topic_name}"
  resource_group_name = "${azurerm_resource_group.EmailSentiment.name}"
  namespace_name      = "${azurerm_servicebus_namespace.EmailSentiment.name}"

  enable_partitioning = true
}
