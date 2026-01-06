using System.Collections.Generic;
using System.Text.RegularExpressions;
using FluentValidation;
using FluentValidation.Validators;
using Tsa.Submissions.Coding.WebApi.Models;

namespace Tsa.Submissions.Coding.WebApi.Validators;

public class ParticipantsPropertyValidator : PropertyValidator<TeamModel, IList<ParticipantModel>>
{
    public override string Name => "ParticipantsPropertyValidator";

    public override bool IsValid(ValidationContext<TeamModel> context, IList<ParticipantModel> value)
    {
        switch (value.Count)
        {
            case < 1:
                context.AddFailure("There must be at least one participant for the team");
                return false;
            case > 2:
                context.AddFailure("The team cannot have more then two participants.");
                return false;
        }

        foreach (var participant in value)
        {
            if (string.IsNullOrWhiteSpace(participant.SchoolNumber) ||
                string.IsNullOrWhiteSpace(participant.ParticipantNumber))
            {
                context.AddFailure("Both the school number and participant number are required.");
                return false;
            }

            if (participant.SchoolNumber != context.InstanceToValidate.SchoolNumber)
            {
                context.AddFailure("The school number of the participants must match the school number of the team.");
                return false;
            }

            if (!Regex.IsMatch(participant.ParticipantNumber, "[0-8][\\d]{2}"))
            {
                context.AddFailure("The participant number must be three digits in length and not start with a 9.");
            }
        }

        return true;
    }
}
