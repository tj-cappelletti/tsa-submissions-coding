using FluentValidation;
using Tsa.Submissions.Coding.Contracts.Users;

namespace Tsa.Submissions.Coding.WebApi.Validators;

public class UserCreateRequestValidator : UserRequestValidator<UserCreateRequest>
{
    public UserCreateRequestValidator()
    {
        RuleFor(user => user.Password)
            .NotEmpty()
            .WithMessage("A user must have a password.");

        RuleFor(user => user.UserName)
            .NotEmpty()
            .WithMessage("A user must have a username.");
    }
}
