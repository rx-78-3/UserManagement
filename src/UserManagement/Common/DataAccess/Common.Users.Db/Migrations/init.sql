CREATE DATABASE [AuthDb]
GO

USE [AuthDb];
GO

CREATE TABLE Users (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    UserName NVARCHAR(100) NOT NULL,
    PasswordHash NVARCHAR(256) NOT NULL,
    IsActive BIT NOT NULL,
    CONSTRAINT UQ_Users_UserName UNIQUE (UserName)
);
GO

CREATE TYPE dbo.UsersToUpdate AS TABLE
(
    Id UNIQUEIDENTIFIER,
    IsActive BIT
);
GO

CREATE PROCEDURE UpdateUser
    @UserUpdates dbo.UsersToUpdate READONLY
AS
BEGIN
    UPDATE u
    SET u.IsActive = updates.IsActive
    FROM Users u
    INNER JOIN @UserUpdates updates ON u.Id = updates.Id
        AND u.IsActive <> updates.IsActive
END
GO

INSERT INTO Users (Id, Username, PasswordHash, IsActive) VALUES ('9A8034C8-1DFB-4BD9-A16A-200979B8282F', 'someuser1', 'hashed_password', 1);
INSERT INTO Users (Id, Username, PasswordHash, IsActive) VALUES ('9FDA9345-F78B-49A6-9ECA-1DBC45A8AC59', 'someuser2', 'hashed_password', 0);

PRINT 'The database update succeeded.';