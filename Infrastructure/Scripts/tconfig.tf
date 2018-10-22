terraform {
  backend "azurerm" {
    resource_group_name = "GeneralStorage"
    storage_account_name = "terraformstore"
    container_name       = "deploystate"
    key                  = "dev.terraform.tfstate"
  }
}