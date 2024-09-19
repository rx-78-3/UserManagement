using Identity.Api.Services.Abstractions;
using System.Security.Cryptography;
using System.Text;

namespace Identity.Api.Services;

public class PasswordHasher : IPasswordHasher
{
    public string ComputeHash(string input, string salt)
    {
        // ToDo change to pbkdf2.
        using (var sha256 = SHA256.Create())
        {
            var saltedInput = $"{input}{salt}";
            var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(saltedInput));
            var builder = new StringBuilder();
            foreach (var b in bytes)
            {
                builder.Append(b.ToString("x2"));
            }
            return builder.ToString();
        }
    }
}
