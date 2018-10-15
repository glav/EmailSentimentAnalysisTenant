
# Application insights
variable "environment" {
}
variable "location" {
}
variable "resource_group_name" {
}

resource "azurerm_application_insights" "EmailSentimentMetrics" {
  name                = "emailsentiment-appinsights-${var.environment}"
  location            = "${var.location}"
  resource_group_name = "${var.resource_group_name}"
  application_type    = "Web"
}

output "instrumentation_key" {
  value = "${azurerm_application_insights.EmailSentimentMetrics.instrumentation_key}"
}

output "app_id" {
  value = "${azurerm_application_insights.EmailSentimentMetrics.app_id}"
}
