using Common.DataAccess.Users;

namespace Common.Cache.Users;

public interface IUserCacheService
{
    bool IsUserInactive(Guid userId);
    void UpdateInactiveUsers(IEnumerable<User> allUsers);
}
