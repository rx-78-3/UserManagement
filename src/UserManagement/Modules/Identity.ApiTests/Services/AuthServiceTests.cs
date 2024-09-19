using Common.DataAccess.Users;
using Identity.Api.Services.Abstractions;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Moq;
using System.IdentityModel.Tokens.Jwt;
using System.Text;

namespace Identity.Api.Services.Tests;

[TestClass()]
public class AuthServiceTests
{
    private const string SecretKey = "your_very_long_secret_key_1234567890";

    private Mock<IUserRepository> _userRepositoryMock;
    private Mock<IPasswordHasher> _passwordHasherMock;
    private IConfiguration _configuration;
    private AuthService _authService;

    [TestInitialize]
    public void Setup()
    {
        _userRepositoryMock = new Mock<IUserRepository>();
        _passwordHasherMock = new Mock<IPasswordHasher>();

        var inMemorySettings = new Dictionary<string, string> {
            {"JwtSettings:SecretKey", SecretKey},
            {"JwtSettings:Issuer", "your_issuer"},
            {"JwtSettings:Audience", "your_audience"}
        };

        _configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(inMemorySettings)
            .Build();

        _authService = new AuthService(_userRepositoryMock.Object, _passwordHasherMock.Object, _configuration);
    }

    [TestMethod]
    public void GenerateJwtToken_ValidUser_ReturnsToken()
    {
        // Arrange
        var user = new User { Id = Guid.NewGuid(), UserName = "testuser" };

        // Act
        var token = _authService.GenerateJwtToken(user);

        // Assert
        Assert.IsNotNull(token);

        var tokenHandler = new JwtSecurityTokenHandler();
        var validationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = "your_issuer",
            ValidAudience = "your_audience",
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(SecretKey))
        };

        SecurityToken validatedToken;
        var principal = tokenHandler.ValidateToken(token, validationParameters, out validatedToken);

        Assert.IsNotNull(principal);
        Assert.AreEqual("testuser", principal.Identity.Name);
        Assert.IsTrue(principal.HasClaim(c => c.Type == JwtRegisteredClaimNames.Jti));
    }

    [TestMethod]
    public async Task ValidateUserAsync_UserNotFound_ReturnsNull()
    {
        // Arrange
        _userRepositoryMock.Setup(repo => repo.GetByUserNameAsync(It.IsAny<string>()))
            .ReturnsAsync((User)null);

        // Act
        var result = await _authService.ValidateUserAsync("nonexistentuser", "password");

        // Assert
        Assert.IsNull(result);
    }

    [TestMethod]
    public async Task ValidateUserAsync_PasswordHashDoesNotMatch_ReturnsNull()
    {
        // Arrange
        var user = new User { Id = Guid.NewGuid(), UserName = "testuser", PasswordHash = "hashedpassword" };
        _userRepositoryMock.Setup(repo => repo.GetByUserNameAsync(It.IsAny<string>()))
            .ReturnsAsync(user);

        _passwordHasherMock.Setup(hasher => hasher.ComputeHash(It.IsAny<string>(), It.IsAny<string>()))
            .Returns("wronghash");

        // Act
        var result = await _authService.ValidateUserAsync("testuser", "password");

        // Assert
        Assert.IsNull(result);
    }

    [TestMethod]
    public async Task ValidateUserAsync_PasswordHashMatches_ReturnsUser()
    {
        // Arrange
        var user = new User { Id = Guid.NewGuid(), UserName = "testuser", PasswordHash = "correcthash" };
        _userRepositoryMock.Setup(repo => repo.GetByUserNameAsync(It.IsAny<string>()))
            .ReturnsAsync(user);

        _passwordHasherMock.SetupSequence(hasher => hasher.ComputeHash(It.IsAny<string>(), It.IsAny<string>()))
            .Returns("intermediatehash")
            .Returns("correcthash");

        // Act
        var result = await _authService.ValidateUserAsync("testuser", "password");

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(user, result);
    }
}