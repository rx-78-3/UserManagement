using Common.DataAccess.Models;
using Dapper;
using Microsoft.Data.SqlClient;
using System.Data;

namespace Common.DataAccess.Users;

public class UserRepository(string connectionString) : IUserRepository
{
    public async Task<PaginatedResult<User>> GetAsync(int pageIndex, int pageSize)
    {
        using var connection = new SqlConnection(connectionString);
        await connection.OpenAsync();
        using var transaction = await connection.BeginTransactionAsync(IsolationLevel.ReadCommitted);

        var offset = pageIndex * pageSize;

        try
        {
            using var gridReader = await connection.QueryMultipleAsync(
                "GetUsers",
                new { Offset = offset, PageSize = pageSize },
                transaction,
                commandType: CommandType.StoredProcedure);

            var users = await gridReader.ReadAsync<User>();
            var totalCount = await gridReader.ReadFirstAsync<long>();

            await transaction.CommitAsync();

            return new PaginatedResult<User>(totalCount, users);
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    public async Task<IEnumerable<User>> GetByActivityAsync(bool isActive)
    {
        using var connection = new SqlConnection(connectionString);

        var sql = "SELECT * FROM Users WHERE IsActive = @IsActive";

        return await connection.QueryAsync<User>(sql, new { IsActive = isActive });
    }

    public async Task<User?> GetByUserNameAsync(string userName)
    {
        using var connection = new SqlConnection(connectionString);

        var sql = "SELECT * FROM Users WHERE UserName = @UserName";

        return await connection.QueryFirstOrDefaultAsync<User>(sql, new { UserName = userName });
    }

    /// <summary>
    /// Updates the specified users.
    /// </summary>
    /// <param name="users">Users to update.</param>
    /// <returns>Number of updated users.</returns>
    public async Task<int> UpdateAsync(IEnumerable<User> users)
    {
        using var connection = new SqlConnection(connectionString);

        var userTable = new DataTable();
        userTable.Columns.Add("Id", typeof(Guid));
        userTable.Columns.Add("IsActive", typeof(bool));

        foreach (var user in users)
        {
            userTable.Rows.Add(user.Id, user.IsActive);
        }

        var parameters = new { UserUpdates = userTable.AsTableValuedParameter("dbo.UsersToUpdate") };
        var updatedUsersNumber = await connection.ExecuteAsync("UpdateUser", parameters);

        return updatedUsersNumber;
    }
}
