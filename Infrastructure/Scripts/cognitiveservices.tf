resource "azurerm_cognitive_account" "EmailSentimentCognitiveService" {
  name                      = "email-sentiment-text-analysis"
  location                  = "${var.location}"
  resource_group_name       = "${azurerm_resource_group.EmailSentiment.name}"
  kind                      = "TextAnalytics"

  sku {
    name = "${var.cognitive_service_tier_sku_name}"
    tier = "${var.cognitive_service_tier_sku_tier}"
  }

  tags {
    Acceptance = "Test"
  }
}