using System;
using FluentValidation;
using Tsa.Submissions.Coding.Contracts.Users;
using Tsa.Submissions.Coding.WebApi.Authorization;

namespace Tsa.Submissions.Coding.WebApi.Validators;

public class UserRequestValidator<T> : AbstractValidator<T> where T : IUserRequest
{
    private const string ValidIndividualParticipantNumberRegEx = @"^(?:[0-8])\d{2}$";

    public UserRequestValidator()
    {
        RuleFor(user => user.Participants)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
                .WithMessage("A participant user must have at least one participant associated with them.")
            .Must(participants => participants!.Count <= 2)
                .WithMessage("A participant user can have at most two participants associated with them.")
            .When(user => string.Equals(user.Role, SubmissionRoles.Participant, StringComparison.InvariantCultureIgnoreCase));

        RuleForEach(user => user.Participants)
            .Matches(ValidIndividualParticipantNumberRegEx)
            .When(user => string.Equals(user.Role, SubmissionRoles.Participant, StringComparison.InvariantCultureIgnoreCase))
            .WithMessage("Each participant's individual number is required and must be 3 digits in length and start with a digit from 0 to 8.");

        RuleFor(user => user.Role)
            .NotEmpty()
            .Must(role => string.Equals(role, SubmissionRoles.Judge, StringComparison.CurrentCultureIgnoreCase) ||
                          string.Equals(role, SubmissionRoles.Participant, StringComparison.CurrentCultureIgnoreCase))
            .WithMessage("A user must have a valid role of 'Judge' or 'Participant'.");

        When(user => string.Equals(user.Role, SubmissionRoles.Participant, StringComparison.InvariantCultureIgnoreCase), () =>
        {
            RuleFor(user => user.Team)
                .NotNull()
                .WithMessage("A participant must be associated with a team.")
                // Null forgiveness is needed here because the validator is only applied when the role is Participant, which requires a team, so it will never be null in that context.
                .SetValidator(new TeamRequestValidator()!);
        });
    }
}
