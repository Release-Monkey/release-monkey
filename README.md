# Release Monkey

## Terraform setup
- Download and install terraform (https://developer.hashicorp.com/terraform/tutorials/aws-get-started/install-cli)\
- Set AWS environment variables
- Run terraform fmt
- Run terraform plan
- Run terraform apply

## SQL Migrations instructions
- upload your sql to the migrations folder as a new file with this naming convention V{year}{month}{day}{24hour}{min}__{description}.sql

## Postman for mock api calls
- Launch postman and import postman folder 

## CLI

### Running CLI

You must be in the /cli folder to run the CLI.

```bash
$ dotnet run COMMAND
```

Where `COMMAND` is the command to be executed. See `Program.cs` for the available commands.

### Building the CLI

You must be in the /cli folder to build the CLI.

```bash
$ dotnet publish -c Release
```

The executable file can be found in folder `cli\bin\Release\net8.0\publish`

## Access the database using MS SQL Server studio
- Open ms sql sever studio
- Set server type to database engine
- Insert {} as the server name
- Select SQL sever authentication
- login as admin
- Password: 

## ERD Design
    QuickDBD was used for ERD Design and Generation, the code for it can be found in the documentation/Diagrams folder

## Enums Used
 - Role
    - 0 = Releaser
    - 1 = Tester
    - 2 = Both (Rather then assigning the user the both role, add a new entry in the table)
 - State
    - 0 = Accepted
    - 1 = Rejected
    - 2 = Pending

