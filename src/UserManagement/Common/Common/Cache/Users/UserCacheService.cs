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

    public bool TryGetInactiveUser(object key, out object? value)
    {
        return _memoryCache.TryGetValue($"{InactiveUserKeyPrefix}_{key}", out value);
    }

    public void UpdateInactiveUsers(IEnumerable<User> updatedUsers)
    {
        foreach (var user in updatedUsers)
        {
            var isUserCached = TryGetInactiveUser(user.Id, out object? cachedUser);
            
            if (user.IsActive)
            {
                if (isUserCached)
                {
                    _memoryCache.Remove($"{InactiveUserKeyPrefix}_{user.Id}");
                }

                return;
            }

            if (!isUserCached)
            {
                _memoryCache.CreateEntry($"{InactiveUserKeyPrefix}_{user.Id}");
            }

            _memoryCache.Set($"{InactiveUserKeyPrefix}_{user.Id}", new { user.IsActive });
        }
    }

    private void Initialize(IUserRepository userRepository)
    {
        var users = userRepository.GetByActivityAsync(false).Result;

        // Initialize the cache with inactive users.
        foreach (var user in users)
        {
            _memoryCache.Set($"{InactiveUserKeyPrefix}_{user.Id}", new { user.IsActive });
        }
    }
}
