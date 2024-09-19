using Dapper;
using Microsoft.Data.SqlClient;

namespace Common.DataAccess.Users.Migrations;

public static class DatabaseInitialization
{
    private const int MaxRetries = 10;
    private const int RetryDelayMilliseconds = 10000;

    public static async Task InitializeDatabase(string masterConnectionString, string connectionString)
    {
        await WaitForDatabaseAvailability(masterConnectionString);

        using (var masterConnection = new SqlConnection(masterConnectionString))
        {
            var databaseExists = await masterConnection.ExecuteScalarAsync<int>(
                "SELECT COUNT(*) FROM sys.databases WHERE name = @DbName",
                new { DbName = "AuthDb" });

            if (databaseExists == 0)
            {
                await masterConnection.ExecuteAsync("CREATE DATABASE [AuthDb]");
            }
        }

        using var connection = new SqlConnection(connectionString);

        var tableExists = await connection.ExecuteScalarAsync<int>(@"
                SELECT COUNT(*) 
                FROM INFORMATION_SCHEMA.TABLES 
                WHERE TABLE_NAME = 'Users'");

        if (tableExists == 0)
        {
            await connection.ExecuteAsync(@"
                    CREATE TABLE Users (
                        Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
                        UserName NVARCHAR(100) NOT NULL,
                        PasswordHash NVARCHAR(256) NOT NULL,
                        IsActive BIT NOT NULL,
                        CONSTRAINT UQ_Users_UserName UNIQUE (UserName)
                    );

                    CREATE TYPE dbo.UsersToUpdate AS TABLE
                    (
                        Id UNIQUEIDENTIFIER,
                        IsActive BIT
                    );");

            await connection.ExecuteAsync(@"
                    CREATE PROCEDURE UpdateUser
                        @UserUpdates dbo.UsersToUpdate READONLY
                    AS
                    BEGIN
                        UPDATE u
                        SET u.IsActive = updates.IsActive
                        FROM Users u
                        INNER JOIN @UserUpdates updates ON u.Id = updates.Id
                            AND u.IsActive <> updates.IsActive
                    END");
        }
    }

    private static async Task WaitForDatabaseAvailability(string connectionString)
    {
        int retryCount = 0;
        while (retryCount < MaxRetries)
        {
            try
            {
                using (var connection = new SqlConnection(connectionString))
                {
                    await connection.OpenAsync();
                    return;
                }
            }
            catch (SqlException)
            {
                retryCount++;
                await Task.Delay(RetryDelayMilliseconds);
            }
        }

        throw new Exception("Database is not available after multiple retries.");
    }
}
