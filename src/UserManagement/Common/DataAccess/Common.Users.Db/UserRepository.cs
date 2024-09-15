using Common.DataAccess.Models;
using Dapper;
using Microsoft.Data.SqlClient;
using System.Data;

namespace Common.DataAccess.Users;

public class UserRepository(string connectionString) : IUserRepository
{
    private const int UniqueConstraintViolationCode = 2627;

    public async Task<PaginatedResult<User>> GetAsync(int pageIndex, int pageSize)
    {
        using var connection = new SqlConnection(connectionString);

        // ToDo move to a stored procedure
        var offset = pageIndex * pageSize;
        var sql = @"
                SELECT * 
                FROM Users
                ORDER BY Id
                OFFSET @Offset ROWS
                FETCH NEXT @PageSize ROWS ONLY
                
                SELECT COUNT(*) 
                FROM Users";

        using var gridReader = await connection.QueryMultipleAsync(sql, new { Offset = offset, PageSize = pageSize });
        var users = await gridReader.ReadAsync<User>();
        var count = await gridReader.ReadFirstAsync<long>();

        return new PaginatedResult<User>(count, users);
    }

    public async Task<IEnumerable<User>> GetByIdsAsync(IEnumerable<Guid> ids)
    {
        using var connection = new SqlConnection(connectionString);

        var sql = "SELECT * FROM Users WHERE Id IN @Ids";

        return await connection.QueryAsync<User>(sql, new { Ids = ids });
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
        await connection.OpenAsync();
        using var transaction = await connection.BeginTransactionAsync();

        var userTable = new DataTable();
        userTable.Columns.Add("Id", typeof(Guid));
        userTable.Columns.Add("IsActive", typeof(bool));

        foreach (var user in users)
        {
            userTable.Rows.Add(user.Id, user.IsActive);
        }

        var parameters = new { UserUpdates = userTable.AsTableValuedParameter("dbo.UsersToUpdate") };

        var udpatedUsersNumber = await connection.ExecuteAsync("UpdateUser", parameters, transaction);

        await transaction.CommitAsync();

        return udpatedUsersNumber;
    }
}