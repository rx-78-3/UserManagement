using Common.Cache.Users;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Common.Middleware;

public class UserIsActiveClaimMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<UserIsActiveClaimMiddleware> _logger;
    private readonly IUserCacheService _userCacheService;

    public UserIsActiveClaimMiddleware(RequestDelegate next, ILogger<UserIsActiveClaimMiddleware> logger, IUserCacheService userCacheService)
    {
        _next = next;
        _logger = logger;
        _userCacheService = userCacheService;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        if (context.Request.Headers.TryGetValue("Authorization", out var authorizationHeader))
        {
            var token = authorizationHeader.FirstOrDefault()?.Split(" ").Last();

            if (!string.IsNullOrEmpty(token) && context.User.Identity?.IsAuthenticated == true)
            {
                var userId = context.User.Claims.FirstOrDefault(c => c.Type == "Id")?.Value;

                if (string.IsNullOrWhiteSpace(userId) || _userCacheService.TryGetInactiveUser(userId, out object? user))
                {
                    _logger.LogWarning("Unauthorized: Name claim not found.");
                    context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                    await context.Response.WriteAsync("Unauthorized: Name claim not found.");
                    return;
                }
            }
        }

        await _next(context);
    }
}
