resource "azurerm_storage_account" "EmailSentiment" {
  name                     = "emailsentimentsa${var.environment}"
  resource_group_name      = "${azurerm_resource_group.EmailSentiment.name}"
  location                 = "${azurerm_resource_group.EmailSentiment.location}"
  account_tier             = "Standard"
  account_replication_type = "LRS"
}

resource "azurerm_app_service_plan" "EmailSentiment" {
  name                = "triggermail-service-plan-${var.environment}"
  location            = "${azurerm_resource_group.EmailSentiment.location}"
  resource_group_name = "${azurerm_resource_group.EmailSentiment.name}"
  kind                = "FunctionApp"

  sku {
    tier = "Dynamic"
    size = "Y1"
  }
}

resource "azurerm_function_app" "EmailSentimentTriggerMailFuncApp" {
  name                      = "trigger-mail-function-${var.environment}"
  location                  = "${azurerm_resource_group.EmailSentiment.location}"
  resource_group_name       = "${azurerm_resource_group.EmailSentiment.name}"
  app_service_plan_id       = "${azurerm_app_service_plan.EmailSentiment.id}"
  storage_connection_string = "${azurerm_storage_account.EmailSentiment.primary_connection_string}"
}


resource "azurerm_function_app" "EmailSentimentCollectMailFuncApp" {
  name                      = "collect-mail-function-${var.environment}"
  location                  = "${azurerm_resource_group.EmailSentiment.location}"
  resource_group_name       = "${azurerm_resource_group.EmailSentiment.name}"
  app_service_plan_id       = "${azurerm_app_service_plan.EmailSentiment.id}"
  storage_connection_string = "${azurerm_storage_account.EmailSentiment.primary_connection_string}"
}

resource "azurerm_function_app" "EmailSentimentCleanMailFuncApp" {
  name                      = "clean-mail-function-${var.environment}"
  location                  = "${azurerm_resource_group.EmailSentiment.location}"
  resource_group_name       = "${azurerm_resource_group.EmailSentiment.name}"
  app_service_plan_id       = "${azurerm_app_service_plan.EmailSentiment.id}"
  storage_connection_string = "${azurerm_storage_account.EmailSentiment.primary_connection_string}"
}

resource "azurerm_function_app" "EmailSentimentProcessMailFuncApp" {
  name                      = "process-mail-function-${var.environment}"
  location                  = "${azurerm_resource_group.EmailSentiment.location}"
  resource_group_name       = "${azurerm_resource_group.EmailSentiment.name}"
  app_service_plan_id       = "${azurerm_app_service_plan.EmailSentiment.id}"
  storage_connection_string = "${azurerm_storage_account.EmailSentiment.primary_connection_string}"
}