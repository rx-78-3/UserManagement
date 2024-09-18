namespace Common.DataAccess.Users;

public class User
{
    public Guid Id { get; set; }
    public string UserName { get; set; }
    public string PasswordHash { get; set; }
    public bool IsActive { get; set; }
}