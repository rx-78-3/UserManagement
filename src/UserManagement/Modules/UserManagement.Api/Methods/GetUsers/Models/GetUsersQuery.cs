using Common.Cqrs;
using Common.Pagination;

namespace UserManagement.Api.Methods.GetUsers.Models;

internal record GetUsersQuery : PaginationRequest, IQuery<GetUsersResult>
{
    public GetUsersQuery(PaginationRequest original) : base(original)
    {
    }
}
