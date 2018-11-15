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

variable "queue_trigger_name" {
  default="triggeremail"
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

variable "pop_mail_host" {
}

variable "pop_mail_port" {
}

variable "pop_mail_username" {
}

variable "pop_mail_password" {
}

variable "pop_mail_use_ssl" {
  default="true"
}

variable "delete_mail_once_collected" {
  default="true"
}

variable "max_emails_to_retrieve" {
  default = "10"
}

variable "sentimement_api_key" {
}

variable "sentiment_api_location" {
}

locals {
  queue_name = "${var.app}${var.environment}"
  resource_group_name = "${var.app}${var.environment}"
}