using System;
using FluentValidation;
using Tsa.Submissions.Coding.Contracts.Users;

namespace Tsa.Submissions.Coding.WebApi.Validators;

public class UserRequestValidator<T> : AbstractValidator<T> where T : IUserRequest
{
    public UserRequestValidator()
    {
        RuleFor(user => user.Role)
            .NotEmpty()
            .Must(role => string.Equals(role, "Judge", StringComparison.CurrentCultureIgnoreCase) ||
                          string.Equals(role, "Participant", StringComparison.CurrentCultureIgnoreCase))
            .WithMessage("A user must have a valid role of 'Judge' or 'Participant'.");

        RuleFor(user => user.Team)
            .NotNull()
            .When(user => string.Equals(user.Role, "Participant", StringComparison.InvariantCultureIgnoreCase))
            .WithMessage("A participant must be associated with a team.");

        RuleFor(user => user.UserName)
            .NotEmpty()
            .WithMessage("A user must have a username.");
    }
}
