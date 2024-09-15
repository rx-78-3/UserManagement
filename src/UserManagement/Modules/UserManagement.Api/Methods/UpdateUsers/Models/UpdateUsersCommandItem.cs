namespace UserManagement.Api.Methods.UpdateUsers.Models
{
    // Model for updating not login related user information.
    public record UpdateUsersCommandItem(Guid Id, bool IsActive);
}
