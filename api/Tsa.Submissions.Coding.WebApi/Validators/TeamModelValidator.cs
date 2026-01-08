using System;
using FluentValidation;
using Tsa.Submissions.Coding.WebApi.Entities;
using Tsa.Submissions.Coding.WebApi.Models;

namespace Tsa.Submissions.Coding.WebApi.Validators;

public class TeamModelValidator : AbstractValidator<TeamModel>
{
    private const string ValidSchoolNumberRegEx = @"(?:1|2)[\d]{3}";
    private const string ValidTeamNumberRegEx = @"9[\d]{2}";

    public TeamModelValidator()
    {
        RuleFor(team => team.CompetitionLevel)
            .NotEmpty()
            .IsEnumName(typeof(CompetitionLevel), false)
            .WithMessage("The competition level must be either 'MiddleSchool' or 'HighSchool'.");

        RuleFor(team => team.SchoolNumber)
            .NotEmpty()
            .Matches(ValidSchoolNumberRegEx)
            .WithMessage("The School Number is required and must be 4 digits in length and start with a 1 for Middle School and 2 for High School")
            .Must(schoolNumber => schoolNumber.StartsWith('1'))
            .When(team => string.Equals(team.CompetitionLevel, CompetitionLevel.MiddleSchool.ToString(), StringComparison.CurrentCultureIgnoreCase), ApplyConditionTo.CurrentValidator)
            .WithMessage("The School Number must start with a 1 when the competition level is Middle School")
            .Must(schoolNumber => schoolNumber.StartsWith('2'))
            .When(team => string.Equals(team.CompetitionLevel, CompetitionLevel.HighSchool.ToString(), StringComparison.CurrentCultureIgnoreCase), ApplyConditionTo.CurrentValidator)
            .WithMessage("The School Number must start with a 2 when the competition level is High School");

        RuleFor(team => team.TeamNumber)
            .NotEmpty()
            .Matches(ValidTeamNumberRegEx)
            .WithMessage("The Team Number is required and must 3 digits that starts with a 9");

        //RuleFor(team => team.Participants)
        //    .SetValidator(new ParticipantsPropertyValidator())
        //    .WithMessage("The participant(s) are not valid");
    }
}
