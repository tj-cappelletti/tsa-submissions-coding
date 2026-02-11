using System;
using FluentValidation;
using Tsa.Submissions.Coding.Contracts.Users;
using Tsa.Submissions.Coding.WebApi.Authorization;

namespace Tsa.Submissions.Coding.WebApi.Validators;

public class UserRequestValidator<T> : AbstractValidator<T> where T : IUserRequest
{
    public UserRequestValidator()
    {
        RuleFor(user => user.Participants)
            .NotEmpty()
            .When(user => string.Equals(user.Role, SubmissionRoles.Participant, StringComparison.InvariantCultureIgnoreCase))
            .WithMessage("A participant user must have at least one participant associated with them.");

        RuleFor(user => user.Participants)
            .NotEmpty()
            .Must(participants => participants!.Count <= 2)
            .When(user => string.Equals(user.Role, SubmissionRoles.Participant, StringComparison.InvariantCultureIgnoreCase))
            .WithMessage("A participant user can have at most two participants associated with them.");

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
