using Common.DataAccess.Users;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;

namespace Common.Cache.Users;

public class UserCacheService : IUserCacheService
{
    private const string InactiveUserKeyPrefix = "InactiveUser";

    private IMemoryCache _memoryCache;

    public UserCacheService(IMemoryCache memoryCache, IServiceProvider serviceProvider)
    {
        _memoryCache = memoryCache;

        using var scope = serviceProvider.CreateScope();
        var userRepository = scope.ServiceProvider.GetRequiredService<IUserRepository>();
        Initialize(userRepository);
    }

    public bool IsUserInactive(Guid userId)
    {
        return _memoryCache.TryGetValue($"{InactiveUserKeyPrefix}_{userId}", out _);
    }

    public void UpdateInactiveUsers(IEnumerable<User> updatedUsers)
    {
        foreach (var user in updatedUsers)
        {
            var isInactiveUserCached = IsUserInactive(user.Id);

            if (isInactiveUserCached)
            {
                if (user.IsActive)
                {
                    _memoryCache.Remove($"{InactiveUserKeyPrefix}_{user.Id}");
                }
            }
            else
            {
                _memoryCache.Set($"{InactiveUserKeyPrefix}_{user.Id}", true); // Just a placeholder since we don't need the user object.
            }
        }
    }

    private void Initialize(IUserRepository userRepository)
    {
        var users = userRepository.GetByActivityAsync(false).Result;

        // Initialize the cache with inactive users.
        foreach (var user in users)
        {
            _memoryCache.Set($"{InactiveUserKeyPrefix}_{user.Id}", true); // Just a placeholder since we don't need the user object.
        }
    }
}
