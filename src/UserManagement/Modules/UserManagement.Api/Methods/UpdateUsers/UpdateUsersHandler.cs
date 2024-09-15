using Common.Cqrs;
using Common.DataAccess.Users;
using Mapster;
using UserManagement.Api.Methods.UpdateUsers.Models;

namespace UserManagement.Api.Methods.UpdateUsers;

internal class UpdateUsersHandler(IUserRepository repository) :
    ICommandHandler<UpdateUsersCommand, UpdateUsersResult>
{
    public async Task<UpdateUsersResult> Handle(UpdateUsersCommand command, CancellationToken cancellationToken)
    {
        var usersToUpdate = command.Users.Adapt<User[]>();

        var updetedUsersNumber = await repository.UpdateAsync(usersToUpdate);
        
        var result = new UpdateUsersResult(updetedUsersNumber);

        return result;
    }
}
