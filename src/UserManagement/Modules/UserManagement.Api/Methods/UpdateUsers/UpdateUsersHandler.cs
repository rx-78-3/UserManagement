using Common.Cache.Users;
using Common.Cqrs;
using Common.DataAccess.Users;
using Mapster;
using UserManagement.Api.Methods.UpdateUsers.Models;

namespace UserManagement.Api.Methods.UpdateUsers;

internal class UpdateUsersHandler(IUserRepository repository, IUserCacheService userCacheService) :
    ICommandHandler<UpdateUsersCommand, UpdateUsersResult>
{
    public async Task<UpdateUsersResult> Handle(UpdateUsersCommand command, CancellationToken cancellationToken)
    {
        var usersToUpdate = command.Users.Adapt<User[]>();

        var updatedUsersNumber = await repository.UpdateAsync(usersToUpdate);
        userCacheService.UpdateInactiveUsers(usersToUpdate);
        
        var result = new UpdateUsersResult(updatedUsersNumber);

        return result;
    }
}
