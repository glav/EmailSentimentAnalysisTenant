
resource "azurerm_app_service_plan" "emailsentimentdevwebsite" {
  name                = "emailsentiment-website-${var.environment}-plan"
  location            = "${var.location}"
  resource_group_name = "${azurerm_resource_group.EmailSentiment.name}"

  sku {
    tier = "Standard"
    size = "S1"
  }
}

resource "azurerm_app_service" "emailsentimentdevwebsite" {
  name                = "emailsentiment-website-${var.environment}"
  location            = "${var.location}"
  resource_group_name = "${azurerm_resource_group.EmailSentiment.name}"
  app_service_plan_id = "${azurerm_app_service_plan.emailsentimentdevwebsite.id}"

  site_config {
    dotnet_framework_version = "v4.0"
    default_documents = ["index.htm", "index.html", "default.htm","default.html","hostingstart.html"]
  }

  app_settings {
    "ApiEndpoint" = "https://host.com"
  }

}