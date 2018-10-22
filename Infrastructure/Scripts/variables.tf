variable "environment" {
  default="dev"
}
variable "location" {
  default = "australiaeast"
}

variable "app_insights_location" {
  default = "southeastasia"
}

variable "app" {
  default = "EmailSentiment"
}

variable "servicebus_collect_topic_name" {
  default="CollectEmailTopic"
}

variable "servicebus_clean_topic_name" {
  default="CleanEmailTopic"
}

variable "servicebus_process_topic_name" {
  default="ProcessEmailTopic"
}

locals {
  servicebus_name = "${var.app}${var.environment}"
  resource_group_name = "${var.app}${var.environment}"
}