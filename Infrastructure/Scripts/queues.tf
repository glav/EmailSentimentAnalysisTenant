provider "azurerm" { }

# Create a resource group
resource "azurerm_resource_group" "EmailSentiment" {
  name     = "${local.resource_group_name}"
  location = "${var.location}"
}

resource "azurerm_servicebus_namespace" "EmailSentimentSb" {
  name                = "${local.servicebus_name}"
  location            = "${var.location}"
  resource_group_name = "${azurerm_resource_group.EmailSentiment.name}"
  sku                 = "standard"

  tags {
    source = "terraform"
  }
}

# Service Bus
resource "azurerm_servicebus_topic" "EmailSentimentCollectTopic" {
  name                = "${var.servicebus_collect_topic_name}"
  resource_group_name = "${azurerm_resource_group.EmailSentiment.name}"
  namespace_name      = "${azurerm_servicebus_namespace.EmailSentimentSb.name}"

  enable_partitioning = true
}
resource "azurerm_servicebus_topic" "EmailSentimentCleanTopic" {
  name                = "${var.servicebus_clean_topic_name}"
  resource_group_name = "${azurerm_resource_group.EmailSentiment.name}"
  namespace_name      = "${azurerm_servicebus_namespace.EmailSentimentSb.name}"

  enable_partitioning = true
}
resource "azurerm_servicebus_topic" "EmailSentimentProcessTopic" {
  name                = "${var.servicebus_process_topic_name}"
  resource_group_name = "${azurerm_resource_group.EmailSentiment.name}"
  namespace_name      = "${azurerm_servicebus_namespace.EmailSentimentSb.name}"

  enable_partitioning = true
}