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

resource "azurerm_servicebus_topic" "EmailSentimentCollectTopic" {
  name                = "${var.servicebus_collect_topic_name}"
  resource_group_name = "${azurerm_resource_group.EmailSentiment.name}"
  namespace_name      = "${azurerm_servicebus_namespace.EmailSentiment.name}"

  enable_partitioning = true
}
resource "azurerm_servicebus_topic" "EmailSentimentCleanTopic" {
  name                = "${var.servicebus_clean_topic_name}"
  resource_group_name = "${azurerm_resource_group.EmailSentiment.name}"
  namespace_name      = "${azurerm_servicebus_namespace.EmailSentiment.name}"

  enable_partitioning = true
}
resource "azurerm_servicebus_topic" "EmailSentimentProcessTopic" {
  name                = "${var.servicebus_process_topic_name}"
  resource_group_name = "${azurerm_resource_group.EmailSentiment.name}"
  namespace_name      = "${azurerm_servicebus_namespace.EmailSentiment.name}"

  enable_partitioning = true
}
