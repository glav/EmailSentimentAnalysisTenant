resource "azurerm_application_insights" "EmailSentimentMetrics" {
  name                = "emailsentiment-appinsights-${var.environment}"
  location            = "${var.app_insights_location}"
  resource_group_name = "${azurerm_resource_group.EmailSentiment.name}"
  application_type    = "Web"
}

resource "azurerm_storage_account" "EmailSentiment" {
  name                     = "emailsentimentsa${var.environment}"
  resource_group_name      = "${azurerm_resource_group.EmailSentiment.name}"
  location                 = "${var.location}"
  account_tier             = "Standard"
  account_replication_type = "LRS"
}

resource "azurerm_app_service_plan" "EmailSentiment" {
  name                = "triggermail-service-plan-${var.environment}"
  resource_group_name = "${azurerm_resource_group.EmailSentiment.name}"
  location            = "${var.location}"
  kind                = "FunctionApp"

  sku {
    tier = "Dynamic"
    size = "Y1"
  }
}

resource "azurerm_function_app" "EmailSentimentTriggerMailFuncApp" {
  name                      = "trigger-mail-function-${var.environment}"
  resource_group_name       = "${azurerm_resource_group.EmailSentiment.name}"
  location                  = "${var.location}"
  app_service_plan_id       = "${azurerm_app_service_plan.EmailSentiment.id}"
  storage_connection_string = "${azurerm_storage_account.EmailSentiment.primary_connection_string}"
  version                   = "~2"

  app_settings {
    "FUNCTIONS_EXTENSION_VERSION"    = "~2"
    "APPINSIGHTS_INSTRUMENTATIONKEY" = "${azurerm_application_insights.EmailSentimentMetrics.instrumentation_key}"
    "StorageConnectionString" = "${azurerm_storage_account.EmailSentiment.primary_connection_string}"
  }
}

resource "azurerm_function_app" "EmailSentimentCollectMailFuncApp" {
  name                      = "collect-mail-function-${var.environment}"
  resource_group_name       = "${azurerm_resource_group.EmailSentiment.name}"
  location                  = "${var.location}"
  app_service_plan_id       = "${azurerm_app_service_plan.EmailSentiment.id}"
  storage_connection_string = "${azurerm_storage_account.EmailSentiment.primary_connection_string}"
  version                   = "~2"

  app_settings {
    "FUNCTIONS_EXTENSION_VERSION"    = "~2"
    "APPINSIGHTS_INSTRUMENTATIONKEY" = "${azurerm_application_insights.EmailSentimentMetrics.instrumentation_key}",
    "popemailhostname" = "${var.popemailhostname}"
    "popemailpassword" = "${var.popemailpassword}"
    "popemailport"     = "${var.popemailport}"
    "popemailusername" = "${var.popemailusername}"
    "popemailusessl"   = "${var.popemailusessl}"
    "StorageConnectionString" = "${azurerm_storage_account.EmailSentiment.primary_connection_string}"
  }
}

resource "azurerm_function_app" "EmailSentimentCleanMailFuncApp" {
  name                      = "clean-mail-function-${var.environment}"
  location                  = "${azurerm_resource_group.EmailSentiment.location}"
  resource_group_name       = "${azurerm_resource_group.EmailSentiment.name}"
  app_service_plan_id       = "${azurerm_app_service_plan.EmailSentiment.id}"
  storage_connection_string = "${azurerm_storage_account.EmailSentiment.primary_connection_string}"
  version                   = "~2"

  app_settings {
    "FUNCTIONS_EXTENSION_VERSION"    = "~2"
    "APPINSIGHTS_INSTRUMENTATIONKEY" = "${azurerm_application_insights.EmailSentimentMetrics.instrumentation_key}"
    "StorageConnectionString" = "${azurerm_storage_account.EmailSentiment.primary_connection_string}"
  }
}

resource "azurerm_function_app" "EmailSentimentProcessMailFuncApp" {
  name                      = "process-mail-function-${var.environment}"
  location                  = "${azurerm_resource_group.EmailSentiment.location}"
  resource_group_name       = "${azurerm_resource_group.EmailSentiment.name}"
  app_service_plan_id       = "${azurerm_app_service_plan.EmailSentiment.id}"
  storage_connection_string = "${azurerm_storage_account.EmailSentiment.primary_connection_string}"
  version                   = "~2"

  app_settings {
    "FUNCTIONS_EXTENSION_VERSION"    = "~2"
    "APPINSIGHTS_INSTRUMENTATIONKEY" = "${azurerm_application_insights.EmailSentimentMetrics.instrumentation_key}"
  }
}

    
output "instrumentation_key" {
  value = "${azurerm_application_insights.EmailSentimentMetrics.instrumentation_key}"
}

output "app_id" {
  value = "${azurerm_application_insights.EmailSentimentMetrics.app_id}"
}

output "id" {
  value = "${azurerm_application_insights.EmailSentimentMetrics.id}"
}
