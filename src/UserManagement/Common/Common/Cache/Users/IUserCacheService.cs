using Common.DataAccess.Users;

namespace Common.Cache.Users;

public interface IUserCacheService
{
    bool TryGetInactiveUser(object key, out object? value);
    void UpdateInactiveUsers(IEnumerable<User> allUsers);
}
