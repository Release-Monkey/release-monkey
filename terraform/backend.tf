# State bucket for storing and sharing terraform state
terraform {
    backend "s3" {
        bucket = "release-monkey-backend-bucket"
        key   = "release-monkey/terraform.tfstate"
        region = "eu-west-1"
        encrypt = true
    }
}