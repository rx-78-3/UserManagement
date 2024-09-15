using Common.Cqrs;
using Common.DataAccess.Users;
using UserManagement.Api.Methods.GetUsers.Models;

namespace UserManagement.Api.Methods.GetUsers;

internal class GetUsersHandler(IUserRepository repository) :
    IQueryHandler<GetUsersQuery, GetUsersResult>
{
    public async Task<GetUsersResult> Handle(GetUsersQuery query, CancellationToken cancellationToken)
    {
        var usersResult = await repository.GetAsync(query.PageIndex, query.PageSize);

        return new GetUsersResult(usersResult);
    }
}
