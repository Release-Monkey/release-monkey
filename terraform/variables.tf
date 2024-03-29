variable "mandatory_tags" {
  type = map(string)
  default = {
    owner         = "bryce.grahn@bbd.co.za"
    created-using = "terraform"
  }
}