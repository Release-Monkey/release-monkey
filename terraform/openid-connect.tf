
resource "aws_iam_openid_connect_provider" "default" {
  url = "https://token.actions.githubusercontent.com"

  client_id_list = [
    "sts.amazonaws.com",
  ]
  thumbprint_list = ["1b511abead59c6ce207077c0bf0e0043b1382612"]

    tags = var.mandatory_tags
}

# Create IAM Role
resource "aws_iam_role" "github_action_role" {
  name               = "GitHubActionRole"
  assume_role_policy = jsonencode({"Version": "2008-10-17",
            "Statement": [
                {
                    "Effect": "Allow",
                    "Principal": {
                        "Federated": "arn:aws:iam::434468814231:oidc-provider/token.actions.githubusercontent.com"
                    },
                    "Action": "sts:AssumeRoleWithWebIdentity",
                    "Condition": {
                        "StringEquals": {
                            "token.actions.githubusercontent.com:aud": "sts.amazonaws.com"
                         },
                        "StringLike": {
                            "token.actions.githubusercontent.com:sub": "repo:Release-Monkey/release-monkey:*"
                        }
                    }
                }
            ]
        })

        tags = var.mandatory_tags
}

# Attach policies to the IAM Role
resource "aws_iam_role_policy_attachment" "github_action_role_attachment" {
  role       = aws_iam_role.github_action_role.name
  policy_arn = "arn:aws:iam::aws:policy/AmazonRDSFullAccess"
}

resource "aws_iam_role_policy_attachment" "github_action_role_attachment_iam" {
  role       = aws_iam_role.github_action_role.name
  policy_arn = "arn:aws:iam::aws:policy/IAMFullAccess"
}

resource "aws_iam_role_policy_attachment" "github_action_role_attachment_secrets" {
  role       = aws_iam_role.github_action_role.name
  policy_arn = "arn:aws:iam::aws:policy/SecretsManagerReadWrite"
}

resource "aws_iam_role_policy_attachment" "github_action_role_attachment_ec2" {
  role       = aws_iam_role.github_action_role.name
  policy_arn = "arn:aws:iam::aws:policy/AmazonEC2FullAccess"
}

resource "aws_iam_role_policy_attachment" "github_action_role_attachment_s3" {
  role       = aws_iam_role.github_action_role.name
  policy_arn = "arn:aws:iam::aws:policy/AmazonS3FullAccess"
}
