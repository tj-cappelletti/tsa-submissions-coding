using System;
using FluentValidation;
using Tsa.Submissions.Coding.WebApi.Authorization;
using Tsa.Submissions.Coding.WebApi.Models;

namespace Tsa.Submissions.Coding.WebApi.Validators;

public class UserModelValidator : AbstractValidator<UserModel>
{
    public UserModelValidator()
    {
        RuleFor(user => user.Role)
            .NotEmpty()
            .Must(role => string.Equals(role, SubmissionRoles.Judge, StringComparison.CurrentCultureIgnoreCase) ||
                          string.Equals(role, SubmissionRoles.Participant, StringComparison.CurrentCultureIgnoreCase))
            .WithMessage("A user must have a valid role of 'Judge' or 'Participant'.");

        RuleFor(user => user.Team)
            .NotNull()
            .When(user => string.Equals(user.Role, SubmissionRoles.Participant, StringComparison.InvariantCultureIgnoreCase))
            .WithMessage("A participant must be associated with a team.");

        RuleFor(user => user.UserName)
            .NotEmpty()
            .WithMessage("A user must have a username.");
    }
}
