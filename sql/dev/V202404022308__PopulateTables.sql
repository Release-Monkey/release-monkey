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
    (1, 1, 1, 'Approved 1'),
    (1, 2, 1, 'Approved 1'),
    (2, 3, 1, 'Approved 2'),
    (2, 4, 2, 'Denied 2'),
    (3, 3, 1, 'Approved 3'),
    (3, 4, 1, 'Approved 3'),
    (5, 9, 2, 'Denied 5'),
    (5, 10, 2, 'Denied 5'),
    (8, 9, 1, 'Approved 8');


-- Insert data into [UserProject] table
INSERT INTO [UserProject] ([UserID], [ProjectID], [Role])
VALUES 
    (1, 7, 1),
    (2, 9, 1),
    (3, 8, 1),
    (4, 10, 1),
    (5, 5, 1),
    (6, 8, 1),
    (6, 6, 1),
    (7, 1, 1),
    (8, 2, 1),
    (4, 3, 1),
    (9, 4, 1),
    (10, 10, 1),
    (1, 1, 2),
    (2, 1, 2),
    (3, 2, 2),
    (4, 2, 2),
    (5, 3, 2),
    (6, 3, 2),
    (7, 4, 2),
    (8, 4, 2),
    (9, 5, 2),
    (10, 5, 2),
    (2, 6, 2),
    (6, 7, 2),
    (9, 8, 2),
    (3, 9, 2),
    (1, 10, 2),
    (5, 10, 2);

COMMIT TRANSACTION DBPOPULATE



