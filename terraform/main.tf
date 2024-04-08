
resource "aws_s3_bucket" "backend-bucket" {
  bucket = "release-monkey-backend-bucket"
  tags   = var.mandatory_tags
}

resource "aws_vpc" "release_monkey_vpc" {
  cidr_block           = "15.0.0.0/16"
  enable_dns_hostnames = true
  tags                 = merge(var.mandatory_tags, { Name = "release-monkey-vpc" })
}

resource "aws_subnet" "subnet_a" {
  vpc_id            = aws_vpc.release_monkey_vpc.id
  cidr_block        = "15.0.1.0/24"
  tags              = var.mandatory_tags
  availability_zone = "eu-west-1a"
}

resource "aws_subnet" "subnet_b" {
  vpc_id            = aws_vpc.release_monkey_vpc.id
  cidr_block        = "15.0.2.0/24"
  tags              = var.mandatory_tags
  availability_zone = "eu-west-1b"
}

resource "aws_db_subnet_group" "release_monkey_subnet_group" {
  name = "release_monkey_subnet_group"
  subnet_ids = [
    aws_subnet.subnet_a.id,
    aws_subnet.subnet_b.id,
  ]
  tags = var.mandatory_tags
}

# Internet Gateway
resource "aws_internet_gateway" "release_monkey_gateway" {
  vpc_id = aws_vpc.release_monkey_vpc.id

  tags = var.mandatory_tags
}

# Routing table
resource "aws_route_table" "release_monkey_route_table" {
  vpc_id = aws_vpc.release_monkey_vpc.id

  route {
    cidr_block = "0.0.0.0/0"
    gateway_id = aws_internet_gateway.release_monkey_gateway.id
  }

  tags = var.mandatory_tags
}

# Resource association table
resource "aws_route_table_association" "association_a" {
  subnet_id      = aws_subnet.subnet_a.id
  route_table_id = aws_route_table.release_monkey_route_table.id
}

resource "aws_route_table_association" "association_b" {
  subnet_id      = aws_subnet.subnet_b.id
  route_table_id = aws_route_table.release_monkey_route_table.id
}

resource "aws_db_instance" "release-monkey-db-sql-serv" {
  identifier                  = "release-monkey-db-sql-serv"
  allocated_storage           = 20
  engine                      = "sqlserver-ex"
  engine_version              = "16.00.4095.4.v1"
  instance_class              = "db.t3.micro"
  publicly_accessible         = true
  username                    = "admin"
  multi_az                    = false # Free tier supports only single AZ
  manage_master_user_password = true  #Fetch password from console
  apply_immediately           = true
  copy_tags_to_snapshot       = true
  db_subnet_group_name        = aws_db_subnet_group.release_monkey_subnet_group.name
  skip_final_snapshot         = true

  vpc_security_group_ids = [
    aws_security_group.sql_serv_security_group.id
  ]
  tags = var.mandatory_tags
}

resource "aws_db_instance" "release-monkey-db-sql-serv-grad" {
  identifier                  = "release-monkey-db-sql-serv-grad"
  allocated_storage           = 20
  engine                      = "sqlserver-ex"
  engine_version              = "16.00.4095.4.v1"
  instance_class              = "db.t3.micro"
  publicly_accessible         = true
  username                    = "grad"
  multi_az                    = false # Free tier supports only single AZ
  manage_master_user_password = true  #Fetch password from console
  apply_immediately           = true
  copy_tags_to_snapshot       = true
  db_subnet_group_name        = aws_db_subnet_group.release_monkey_subnet_group.name
  skip_final_snapshot         = true

  vpc_security_group_ids = [
    aws_security_group.sql_serv_security_group.id
  ]
  tags = var.mandatory_tags
}

# Define EC2 instance resource
resource "aws_instance" "release_monkey_instance" {
  ami           = "ami-0ef9e689241f0bb6e"
  instance_type = "t2.micro"
  subnet_id     = aws_subnet.subnet_a.id

  # Associate the instance with the EC2 security group
  vpc_security_group_ids = [aws_security_group.ec2_security_group.id]

  # Associate the instance with an existing key pair for SSH access
  key_name = "release_monkey_ec2_key_pair"

  tags = merge(var.mandatory_tags, { Name = "ReleaseMonkeyInstance" })
}

resource "aws_eip" "lb" {
  instance = aws_instance.release_monkey_instance.id
  domain   = "vpc"
  tags     = var.mandatory_tags
}




