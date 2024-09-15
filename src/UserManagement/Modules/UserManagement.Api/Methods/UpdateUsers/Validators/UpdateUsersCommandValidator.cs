using FluentValidation;
using UserManagement.Api.Methods.UpdateUsers.Models;

namespace UserManagement.Api.Methods.UpdateUsers.Validators;

public class UpdateUsersCommandValidator : AbstractValidator<UpdateUsersCommand>
{
    public UpdateUsersCommandValidator()
    {
        RuleForEach(x => x.Users).ChildRules(user =>
        {
            user.RuleFor(x => x.Id).NotEmpty();

        });
    }
}
