USE release_monkey_db;
GO;

BEGIN TRANSACTION QUICKDBD

CREATE TABLE [Releaser] (
    [ReleaserID] int  NOT NULL ,
    [Name] string  NOT NULL ,
    [EmailAddress] string  NOT NULL ,
    CONSTRAINT [PK_Releaser] PRIMARY KEY CLUSTERED (
        [ReleaserID] ASC
    )
)

CREATE TABLE [Project] (
    [ProjectID] int  NOT NULL ,
    [ProjectName] varchar(200)  NOT NULL ,
    [Repo] string  NOT NULL ,
    CONSTRAINT [PK_Project] PRIMARY KEY CLUSTERED (
        [ProjectID] ASC
    ),
    CONSTRAINT [UK_Project_ProjectName] UNIQUE (
        [ProjectName]
    ),
    CONSTRAINT [UK_Project_Repo] UNIQUE (
        [Repo]
    )
)

CREATE TABLE [ReleaserProject] (
    [ReleaserProjectID] int  NOT NULL ,
    [ReleaserID] int  NOT NULL ,
    [ProjectID] int  NOT NULL ,
    CONSTRAINT [PK_ReleaserProject] PRIMARY KEY CLUSTERED (
        [ReleaserProjectID] ASC
    )
)

-- Table documentation comment 1 (try the PDF/RTF export)
CREATE TABLE [Tester] (
    [TesterID] int  NOT NULL ,
    [Name] string  NOT NULL ,
    [EmailAddress] string  NOT NULL ,
    CONSTRAINT [PK_Tester] PRIMARY KEY CLUSTERED (
        [TesterID] ASC
    )
)

CREATE TABLE [TesterProject] (
    [TesterProjectID] int  NOT NULL ,
    [TesterID] int  NOT NULL ,
    [ProjectID] int  NOT NULL ,
    CONSTRAINT [PK_TesterProject] PRIMARY KEY CLUSTERED (
        [TesterProjectID] ASC
    )
)

CREATE TABLE [Release] (
    [ReleaseID] int  NOT NULL ,
    [ReleaseName] string  NOT NULL ,
    [ProjectID] int  NOT NULL ,
    [State] enum  NOT NULL ,
    CONSTRAINT [PK_Release] PRIMARY KEY CLUSTERED (
        [ReleaseID] ASC
    )
)

CREATE TABLE [ReleaseTester] (
    [ReleaseTesterID] int  NOT NULL ,
    [ReleaseID] int  NOT NULL ,
    [TesterID] int  NOT NULL ,
    [State] enum  NOT NULL ,
    [Comment] string  NOT NULL ,
    CONSTRAINT [PK_ReleaseTester] PRIMARY KEY CLUSTERED (
        [ReleaseTesterID] ASC
    )
)

CREATE TABLE [Bug] (
    [BugID] int  NOT NULL ,
    [ReleaseTester] int  NOT NULL ,
    [Comment] string  NOT NULL ,
    CONSTRAINT [PK_Bug] PRIMARY KEY CLUSTERED (
        [BugID] ASC
    )
)

ALTER TABLE [ReleaserProject] WITH CHECK ADD CONSTRAINT [FK_ReleaserProject_ReleaserID] FOREIGN KEY([ReleaserID])
REFERENCES [Releaser] ([ReleaserID])

ALTER TABLE [ReleaserProject] CHECK CONSTRAINT [FK_ReleaserProject_ReleaserID]

ALTER TABLE [ReleaserProject] WITH CHECK ADD CONSTRAINT [FK_ReleaserProject_ProjectID] FOREIGN KEY([ProjectID])
REFERENCES [Project] ([ProjectID])

ALTER TABLE [ReleaserProject] CHECK CONSTRAINT [FK_ReleaserProject_ProjectID]

ALTER TABLE [TesterProject] WITH CHECK ADD CONSTRAINT [FK_TesterProject_TesterID] FOREIGN KEY([TesterID])
REFERENCES [Tester] ([TesterID])

ALTER TABLE [TesterProject] CHECK CONSTRAINT [FK_TesterProject_TesterID]

ALTER TABLE [TesterProject] WITH CHECK ADD CONSTRAINT [FK_TesterProject_ProjectID] FOREIGN KEY([ProjectID])
REFERENCES [Project] ([ProjectID])

ALTER TABLE [TesterProject] CHECK CONSTRAINT [FK_TesterProject_ProjectID]

ALTER TABLE [Release] WITH CHECK ADD CONSTRAINT [FK_Release_ProjectID] FOREIGN KEY([ProjectID])
REFERENCES [Project] ([ProjectID])

ALTER TABLE [Release] CHECK CONSTRAINT [FK_Release_ProjectID]

ALTER TABLE [ReleaseTester] WITH CHECK ADD CONSTRAINT [FK_ReleaseTester_ReleaseID] FOREIGN KEY([ReleaseID])
REFERENCES [Release] ([ReleaseID])

ALTER TABLE [ReleaseTester] CHECK CONSTRAINT [FK_ReleaseTester_ReleaseID]

ALTER TABLE [ReleaseTester] WITH CHECK ADD CONSTRAINT [FK_ReleaseTester_TesterID] FOREIGN KEY([TesterID])
REFERENCES [Tester] ([TesterID])

ALTER TABLE [ReleaseTester] CHECK CONSTRAINT [FK_ReleaseTester_TesterID]

ALTER TABLE [Bug] WITH CHECK ADD CONSTRAINT [FK_Bug_ReleaseTester] FOREIGN KEY([ReleaseTester])
REFERENCES [ReleaseTester] ([ReleaseTesterID])

ALTER TABLE [Bug] CHECK CONSTRAINT [FK_Bug_ReleaseTester]

COMMIT TRANSACTION QUICKDBD