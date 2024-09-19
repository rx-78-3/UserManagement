using Common.DataAccess.Users;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Security.Claims;

namespace Common.Middleware;

public class UserIsActiveClaimMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<UserIsActiveClaimMiddleware> _logger;

    public UserIsActiveClaimMiddleware(RequestDelegate next, ILogger<UserIsActiveClaimMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context, IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var repository = scope.ServiceProvider.GetRequiredService<IUserRepository>();

        if (context.Request.Headers.TryGetValue("Authorization", out var authorizationHeader))
        {
            var token = authorizationHeader.FirstOrDefault()?.Split(" ").Last();

            User? user = null;

            if (!string.IsNullOrEmpty(token) && context.User.Identity?.IsAuthenticated == true)
            {
                var userName = context.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value;

                if (!string.IsNullOrEmpty(userName))
                {
                    user = await repository.GetByUserNameAsync(userName);
                }

                if (user == null || !user.IsActive)
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
