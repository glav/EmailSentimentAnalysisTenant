variable "environment" {
  default="dev"
}
variable "location" {
  default = "australiaeast"
}

variable "servicebus_name" {
  default="EmailSentiment"
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