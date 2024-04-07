Use release_monkey_db;

BEGIN TRANSACTION DBPOPULATE
-- Insert data into [User] table
INSERT INTO [User] ([Name], [EmailAddress])
VALUES 
    ('John Doe', 'john@example.com'),
    ('Jane Smith', 'jane@example.com'),
    ('Alice Johnson', 'alice@example.com'),
    ('Bob Williams', 'bob@example.com'),
    ('Eva Brown', 'eva@example.com'),
    ('Michael Davis', 'michael@example.com'),
    ('Sophia Miller', 'sophia@example.com'),
    ('William Jones', 'william@example.com'),
    ('Olivia Wilson', 'olivia@example.com'),
    ('Daniel Taylor', 'daniel@example.com');

-- Insert data into [Project] table
INSERT INTO [Project] ([ProjectName], [Repo], [Token])
VALUES 
    ('Project A', 'https://github.com/projectA', 'tokenA'),
    ('Project B', 'https://github.com/projectB', 'tokenB'),
    ('Project C', 'https://github.com/projectC', 'tokenC'),
    ('Project D', 'https://github.com/projectD', 'tokenD'),
    ('Project E', 'https://github.com/projectE', 'tokenE'),
    ('Project F', 'https://github.com/projectF', 'tokenF'),
    ('Project G', 'https://github.com/projectG', 'tokenG'),
    ('Project H', 'https://github.com/projectH', 'tokenH'),
    ('Project I', 'https://github.com/projectI', 'tokenI'),
    ('Project J', 'https://github.com/projectJ', 'tokenJ');

-- Insert data into [Release] table
INSERT INTO [Release] ([ReleaseName], [ProjectID])
VALUES 
    ('Release 1', 1),
    ('Release 1', 2),
    ('Release 2', 2),
    ('Release 1', 3),
    ('Release 1', 5),
    ('Release 1', 7),
    ('Release 2', 7),
    ('Release 1', 8),
    ('Release 3', 7);

-- Insert data into [ReleaseTester] table
INSERT INTO [ReleaseTester] ([ReleaseID], [TesterID], [State], [Comment])
VALUES 
    (1, 1, 0, 'Approved'),
    (1, 2, 0, 'Approved'),
    (2, 3, 2, 'Pending'),
    (2, 4, 1, 'Denied'),
    (3, 3, 0, 'Approved'),
    (3, 4, 0, 'Approved'),
    (5, 9, 1, 'Denied'),
    (5, 10, 1, 'Denied'),
    (8, 9, 2, 'Pending');


-- Insert data into [UserProject] table
INSERT INTO [UserProject] ([UserID], [ProjectID], [Role])
VALUES 
    (1, 7, 0),
    (2, 9, 0),
    (3, 8, 0),
    (4, 10, 0),
    (5, 5, 0),
    (6, 8, 0),
    (6, 6, 0),
    (7, 1, 0),
    (8, 2, 0),
    (4, 3, 0),
    (9, 4, 0),
    (10, 10, 0),
    (1, 1, 1),
    (2, 1, 1),
    (3, 2, 1),
    (4, 2, 1),
    (5, 3, 1),
    (6, 3, 1),
    (7, 4, 1),
    (8, 4, 1),
    (9, 5, 1),
    (10, 5, 1),
    (2, 6, 1),
    (6, 7, 1),
    (9, 8, 1),
    (3, 9, 1),
    (1, 10, 1),
    (5, 10, 1);

COMMIT TRANSACTION DBPOPULATE



