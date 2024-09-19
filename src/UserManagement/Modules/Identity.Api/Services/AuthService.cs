using Common.DataAccess.Users;
using Identity.Api.Services.Abstractions;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Text;

namespace Identity.Api.Services;

public class AuthService : IAuthService
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly string _issuer;
    private readonly string _audience;
    // ToDo Change to asymmetric key.
    private readonly SymmetricSecurityKey _key;

    public AuthService(IUserRepository userRepository, IPasswordHasher passwordHasher, IConfiguration configuration)
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;

        var jwtConfigurationSection = configuration.GetSection("JwtSettings");
        var secretKey = jwtConfigurationSection["SecretKey"]!;

        _issuer = jwtConfigurationSection["Issuer"]!;
        _audience = jwtConfigurationSection["Audience"]!;
        _key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
    }

    public string GenerateJwtToken(User user)
    {
        var tokenHandler = new System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler();
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(
            [
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            ]),
            Expires = DateTime.UtcNow.AddMinutes(30),
            Issuer = _issuer,
            Audience = _audience,
            SigningCredentials = new SigningCredentials(_key, SecurityAlgorithms.HmacSha256Signature)
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }

    public async Task<User?> ValidateUserAsync(string userName, string password)
    {
        var user = await _userRepository.GetByUserNameAsync(userName);

        if (user == null)
        {
            return null;
        }

        var computedPasswordHash = _passwordHasher.ComputeHash(password, user.Id.ToString());
        computedPasswordHash = _passwordHasher.ComputeHash(computedPasswordHash, user.UserName);

        if (!user.PasswordHash.Equals(computedPasswordHash))
        {
            return null;
        }

        return user;
    }
}
