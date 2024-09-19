using Common.DataAccess.Users;

namespace Identity.Api.Services.Abstractions;

public interface IAuthService
{
    Task<User?> ValidateUserAsync(string userName, string password);
    string GenerateJwtToken(User user);
}
