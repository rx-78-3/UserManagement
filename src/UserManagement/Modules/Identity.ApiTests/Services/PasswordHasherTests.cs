using System.Security.Cryptography;
using System.Text;

namespace Identity.Api.Services.Tests;

[TestClass]
public class PasswordHasherTests
{
    private PasswordHasher _passwordHasher;

    [TestInitialize]
    public void Setup()
    {
        _passwordHasher = new PasswordHasher();
    }

    [TestMethod]
    public void ComputeHash_ValidInputAndSalt_ReturnsExpectedHash()
    {
        // Arrange
        var input = "password";
        var salt = "salt";
        var expectedHash = ComputeSha256Hash($"{input}{salt}");

        // Act
        var result = _passwordHasher.ComputeHash(input, salt);

        // Assert
        Assert.AreEqual(expectedHash, result);
    }

    [TestMethod]
    public void ComputeHash_DifferentInputSameSalt_ReturnsDifferentHash()
    {
        // Arrange
        var input1 = "password1";
        var input2 = "password2";
        var salt = "salt";

        // Act
        var result1 = _passwordHasher.ComputeHash(input1, salt);
        var result2 = _passwordHasher.ComputeHash(input2, salt);

        // Assert
        Assert.AreNotEqual(result1, result2);
    }

    [TestMethod]
    public void ComputeHash_SameInputDifferentSalt_ReturnsDifferentHash()
    {
        // Arrange
        var input = "password";
        var salt1 = "salt1";
        var salt2 = "salt2";

        // Act
        var result1 = _passwordHasher.ComputeHash(input, salt1);
        var result2 = _passwordHasher.ComputeHash(input, salt2);

        // Assert
        Assert.AreNotEqual(result1, result2);
    }

    private string ComputeSha256Hash(string rawData)
    {
        using (var sha256 = SHA256.Create())
        {
            var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(rawData));
            var builder = new StringBuilder();
            foreach (var b in bytes)
            {
                builder.Append(b.ToString("x2"));
            }
            return builder.ToString();
        }
    }
}