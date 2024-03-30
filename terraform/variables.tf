variable "mandatory_tags" {
  type = map(string)
  default = {
    owner         = "abdul.osman@bbd.co.za"
    created-using = "terraform"
  }
}