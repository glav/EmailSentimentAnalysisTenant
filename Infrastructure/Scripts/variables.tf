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
  default="995"
}

variable "pop_mail_username" {
  default="popusername"
}

variable "pop_mail_password" {
}

variable "pop_mail_use_ssl" {
  default="true"
}

locals {
  queue_name = "${var.app}${var.environment}"
  resource_group_name = "${var.app}${var.environment}"
}