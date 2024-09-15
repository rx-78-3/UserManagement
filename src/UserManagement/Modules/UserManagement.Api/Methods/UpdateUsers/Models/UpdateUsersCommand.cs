using Common.Cqrs;
using Common.DataAccess.Users;

namespace UserManagement.Api.Methods.UpdateUsers.Models;

public record UpdateUsersCommand : ICommand<UpdateUsersResult>
{
    public IEnumerable<UpdateUsersCommandItem> Users { get; set; } = Array.Empty<UpdateUsersCommandItem>();
}
