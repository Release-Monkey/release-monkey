USE release_monkey_db;

BEGIN TRANSACTION QUICKDBD

CREATE TABLE [User] (
    [UserID] int IDENTITY(1,1) NOT NULL ,
    [Name] varchar(200)  NOT NULL ,
    [EmailAddress] varchar(200)  NOT NULL ,
    CONSTRAINT [PK_User] PRIMARY KEY CLUSTERED (
        [UserID] ASC
    )
)

CREATE TABLE [Project] (
    [ProjectID] int IDENTITY(1,1) NOT NULL ,
    [ProjectName] varchar(200)  NOT NULL ,
    [Repo] varchar(200)  NOT NULL ,
    [Token] varchar(200)  NOT NULL ,
    CONSTRAINT [PK_Project] PRIMARY KEY CLUSTERED (
        [ProjectID] ASC
    ),
    CONSTRAINT [UK_Project_Repo] UNIQUE (
        [Repo]
    )
)

CREATE TABLE [UserProject] (
    [UserProjectID] int IDENTITY(1,1) NOT NULL ,
    [UserID] int  NOT NULL ,
    [ProjectID] int  NOT NULL ,
    [Role] int  NOT NULL ,
    CONSTRAINT [PK_UserProject] PRIMARY KEY CLUSTERED (
        [UserProjectID] ASC
    )
)

CREATE TABLE [Release] (
    [ReleaseID] int IDENTITY(1,1) NOT NULL ,
    [ReleaseName] varchar(200)  NOT NULL ,
    [ProjectID] int  NOT NULL ,
    CONSTRAINT [PK_Release] PRIMARY KEY CLUSTERED (
        [ReleaseID] ASC
    )
)

CREATE TABLE [ReleaseTester] (
    [ReleaseTesterID] int IDENTITY(1,1) NOT NULL ,
    [ReleaseID] int  NOT NULL ,
    [TesterID] int  NOT NULL ,
    [State] int  NOT NULL ,
    [Comment] nvarchar(2000)  NOT NULL ,
    CONSTRAINT [PK_ReleaseTester] PRIMARY KEY CLUSTERED (
        [ReleaseTesterID] ASC
    )
)

ALTER TABLE [UserProject] WITH CHECK ADD CONSTRAINT [FK_UserProject_UserID] FOREIGN KEY([UserID])
REFERENCES [User] ([UserID])

ALTER TABLE [UserProject] CHECK CONSTRAINT [FK_UserProject_UserID]

ALTER TABLE [UserProject] WITH CHECK ADD CONSTRAINT [FK_UserProject_ProjectID] FOREIGN KEY([ProjectID])
REFERENCES [Project] ([ProjectID])

ALTER TABLE [UserProject] CHECK CONSTRAINT [FK_UserProject_ProjectID]

ALTER TABLE [Release] WITH CHECK ADD CONSTRAINT [FK_Release_ProjectID] FOREIGN KEY([ProjectID])
REFERENCES [Project] ([ProjectID])

ALTER TABLE [Release] CHECK CONSTRAINT [FK_Release_ProjectID]

ALTER TABLE [ReleaseTester] WITH CHECK ADD CONSTRAINT [FK_ReleaseTester_ReleaseID] FOREIGN KEY([ReleaseID])
REFERENCES [Release] ([ReleaseID])

ALTER TABLE [ReleaseTester] CHECK CONSTRAINT [FK_ReleaseTester_ReleaseID]

ALTER TABLE [ReleaseTester] WITH CHECK ADD CONSTRAINT [FK_ReleaseTester_TesterID] FOREIGN KEY([TesterID])
REFERENCES [User] ([UserID])

ALTER TABLE [ReleaseTester] CHECK CONSTRAINT [FK_ReleaseTester_TesterID]

COMMIT TRANSACTION QUICKDBD