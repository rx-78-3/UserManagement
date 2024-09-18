using Microsoft.Data.SqlClient;
using Dapper;

namespace Common.DataAccess.Users.Migrations
{
    public static class TestDataInitialization
    {
        public static async Task InitializeTestData(string connectionString)
        {
            using var connection = new SqlConnection(connectionString);

            var tableExists = await connection.ExecuteScalarAsync<int>(@"
                SELECT COUNT(*) 
                FROM Users");

            if (tableExists == 0)
            {
                await connection.ExecuteAsync(@"
                    INSERT INTO Users (Id, Username, PasswordHash, IsActive) VALUES ('9A8034C8-1DFB-4BD9-A16A-200979B8282F', 'someUser1', 'hashed_password', 1);
                    INSERT INTO Users (Id, Username, PasswordHash, IsActive) VALUES ('9FDA9345-F78B-49A6-9ECA-1DBC45A8AC59', 'someUser2', 'hashed_password', 0);");
            }
        }
    }
}
