module "app_insights_functions" {
    source = "./app_insights"

    environment = "${var.environment}"
    location = "${var.location}"
    resource_group_name = "${local.resource_group_name}"
}