using Common.DataAccess.Models;
using Common.DataAccess.Users;

namespace UserManagement.Api.Methods.GetUsers.Models;

internal record GetUsersResult : PaginatedResult<User>
{
    public GetUsersResult(PaginatedResult<User> original) : base(original)
    {
    }
}
