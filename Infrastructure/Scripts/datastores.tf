#Datastorage
locals {
    sa_name = "emailsentiment${var.environment}sa"
}
resource "azurerm_storage_account" "EmailSentimentTableStorage" {
  name                     = "${local.sa_name}"
  resource_group_name      = "${azurerm_resource_group.EmailSentiment.name}"
  location                 = "${var.location}"
  account_tier             = "Standard"
  account_replication_type = "GRS"

  tags {
    environment = "${var.environment}"
  }
}

output "blob_access_key" {
  value = "${azurerm_storage_account.EmailSentimentTableStorage.secondary_access_key }"
}

output "blob_connection_string" {
  value = "${azurerm_storage_account.EmailSentimentTableStorage.secondary_connection_string}"
}

output "storage_account_name" {
    value = "${local.sa_name}"
}

