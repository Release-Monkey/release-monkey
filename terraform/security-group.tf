# Create a security group so that ms sql studio can connect to the database
resource "aws_security_group" "sql_serv_security_group" {
  name        = "sql_serv_security_group"
  description = "Security group for sql serv"
  vpc_id= aws_vpc.release_monkey_vpc.id

  #Enable internet access to the database
  # Allow inbound traffic from IPv4
  ingress {
      from_port   = 3306
      to_port     = 3306
      protocol    = "tcp"
      cidr_blocks = ["0.0.0.0/0"] # Allow inbound traffic from any IPv4 address
  }

  # Allow inbound traffic from IPv6
  ingress {
      from_port   = 3306
      to_port     = 3306
      protocol    = "tcp"
      ipv6_cidr_blocks = ["::/0"] # Allow inbound traffic from any IPv6 address
  }
  tags = var.mandatory_tags

  ingress {
      from_port   = 8080
      to_port     = 8080
      protocol    = "tcp"
      cidr_blocks = ["0.0.0.0/0"] # Allow inbound traffic from any IPv4 address
  }

   ingress {
      from_port   = 8080
      to_port     = 8080
      protocol    = "tcp"
      ipv6_cidr_blocks = ["::/0"] # Allow inbound traffic from any IPv4 address
  }

  # Define outbound rule to allow traffic to the EC2 instance from the RDS database
  egress {
    from_port   = 0
    to_port     = 0
    protocol    = "-1"
    cidr_blocks = ["0.0.0.0/0"]
  }

}

# Define security group for the EC2 instance
resource "aws_security_group" "ec2_security_group" {
  vpc_id = aws_vpc.release_monkey_vpc.id
  name        = "release_monkey_ec2_security_group"
  description = "Security group for release monkey EC2 instance"

  # Define inbound rules to allow traffic from anywhere to the EC2 instance on port 8080 (for Spring Boot)
  ingress {
    from_port   = 8080
    to_port     = 8080
    protocol    = "tcp"
    cidr_blocks = ["0.0.0.0/0"]
  }

   # Define inbound rules to allow traffic from anywhere to the EC2 instance on port 22 (for SSH)
  ingress {
    from_port   = 22
    to_port     = 22
    protocol    = "tcp"
    cidr_blocks = ["0.0.0.0/0"]
  }

  # Define outbound rules to allow the EC2 instance to communicate with the RDS database
  egress {
    from_port   = 0
    to_port     = 0
    protocol    = "-1"
    cidr_blocks = ["0.0.0.0/0"]
    # security_groups = [aws_security_group.mssql_security_group.id] # this line ensures that the EC2 instance can communicate with the RDS SQL Server database on port 1433 (the default port for SQL Server) by allowing outbound traffic to any destination covered by the rules of the rds_security_group security group.
  }

  tags = var.mandatory_tags
}