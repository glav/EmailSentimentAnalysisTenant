provider "azurerm" { }

# Create a resource group
resource "azurerm_resource_group" "EmailSentiment" {
  name     = "${local.resource_group_name}"
  location = "${var.location}"
}

# Queues
resource "azurerm_storage_queue" "EmailSentimentTriggerMail" {
  name                 = "${var.queue_trigger_name}"
  resource_group_name  = "${azurerm_resource_group.EmailSentiment.name}"
  storage_account_name = "${azurerm_storage_account.EmailSentiment.name}"
}

resource "azurerm_storage_queue" "EmailSentimentCollectMail" {
  name                 = "${var.queue_collect_name}"
  resource_group_name  = "${azurerm_resource_group.EmailSentiment.name}"
  storage_account_name = "${azurerm_storage_account.EmailSentiment.name}"
}

resource "azurerm_storage_queue" "EmailSentimentCleanMail" {
  name                 = "${var.queue_clean_name}"
  resource_group_name  = "${azurerm_resource_group.EmailSentiment.name}"
  storage_account_name = "${azurerm_storage_account.EmailSentiment.name}"
}

resource "azurerm_storage_queue" "EmailSentimentProcessMail" {
  name                 = "${var.queue_process_name}"
  resource_group_name  = "${azurerm_resource_group.EmailSentiment.name}"
  storage_account_name = "${azurerm_storage_account.EmailSentiment.name}"
}

