namespace Identity.Api.Services.Abstractions;

public interface IPasswordHasher
{
    string ComputeHash(string input, string salt);
}
