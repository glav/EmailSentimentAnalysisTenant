variable "environment" {
  default="dev"
}
variable "location" {
  default = "australiaeast"
}

variable "servicebus_name" {
  default="EmailSentiment"
}
variable "servicebus_topic_name" {
  default="EmailFlowTopic"
}