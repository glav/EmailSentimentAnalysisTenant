module "app_insights_functions" {
    source = "./app_insights"

    environment = "${var.environment}"
    location = "${var.location}"
    resource_group_name = "${azurerm_resource_group.EmailSentiment.name}"
}