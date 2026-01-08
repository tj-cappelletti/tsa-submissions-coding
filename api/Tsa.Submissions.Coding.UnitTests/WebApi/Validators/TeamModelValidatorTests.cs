using System.Diagnostics.CodeAnalysis;
using FluentValidation.TestHelper;
using Tsa.Submissions.Coding.UnitTests.Data;
using Tsa.Submissions.Coding.WebApi.Entities;
using Tsa.Submissions.Coding.WebApi.Validators;
using Xunit;

namespace Tsa.Submissions.Coding.UnitTests.WebApi.Validators;

[ExcludeFromCodeCoverage]
public class TeamModelValidatorTests
{
    [Theory]
    [ClassData(typeof(TeamsTestData))]
    [Trait("TestCategory", "UnitTest")]
    public void Should_Successfully_Validate_TeamModel(Team team, TeamDataIssues teamDataIssues)
    {
        // Arrange
        var teamModel = team.ToModel();

        var teamModelValidator = new TeamModelValidator();

        // Act

        var testValidationResult = teamModelValidator.TestValidate(teamModel);

        // Assert

        if (teamDataIssues == TeamDataIssues.None)
        {
            testValidationResult.ShouldNotHaveAnyValidationErrors();
        }
        else
        {
            testValidationResult.ShouldHaveValidationErrors();
        }

        if (teamDataIssues.HasFlag(TeamDataIssues.InvalidCompetitionLevel))
        {
            testValidationResult.ShouldHaveValidationErrorFor(model => model.CompetitionLevel);
        }
        else
        {
            testValidationResult.ShouldNotHaveValidationErrorFor(model => model.CompetitionLevel);
        }

        if (teamDataIssues.HasFlag(TeamDataIssues.InvalidSchoolNumber))
        {
            testValidationResult.ShouldHaveValidationErrorFor(model => model.SchoolNumber);
        }
        else
        {
            testValidationResult.ShouldNotHaveValidationErrorFor(model => model.SchoolNumber);
        }

        if (teamDataIssues.HasFlag(TeamDataIssues.InvalidTeamNumber))
        {
            testValidationResult.ShouldHaveValidationErrorFor(model => model.TeamNumber);
        }
        else
        {
            testValidationResult.ShouldNotHaveValidationErrorFor(model => model.TeamNumber);
        }
    }
}
