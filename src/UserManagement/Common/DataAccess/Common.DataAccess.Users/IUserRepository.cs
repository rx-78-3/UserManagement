using Common.DataAccess.Models;

namespace Common.DataAccess.Users;

public interface IUserRepository
{
    Task<PaginatedResult<User>> GetAsync(int pageIndex, int pageSize);
    Task<IEnumerable<User>> GetByIdsAsync(IEnumerable<Guid> ids);
    Task<User?> GetByUserNameAsync(string userName);

    /// <summary>
    /// Updates the specified users.
    /// </summary>
    /// <param name="users">Users to update.</param>
    /// <returns>Number of updated users.</returns>
    Task<int> UpdateAsync(IEnumerable<User> users);
}