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

variable "queue_collect_name" {
  default="collectemail"
}

variable "queue_clean_name" {
  default="cleanemail"
}

variable "queue_process_name" {
  default="processemail"
}

locals {
  queue_name = "${var.app}${var.environment}"
  resource_group_name = "${var.app}${var.environment}"
}