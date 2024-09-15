using Common.DataAccess.Models;
using Common.DataAccess.Users;
using Common.Pagination;

namespace UserManagement.Api.Methods.GetUsers.Models;

internal record GetUsersResponse : PaginatedResponse<User>
{
    public GetUsersResponse(PaginationRequest request, PaginatedResult<User> result) : base(request, result)
    {
    }
}
