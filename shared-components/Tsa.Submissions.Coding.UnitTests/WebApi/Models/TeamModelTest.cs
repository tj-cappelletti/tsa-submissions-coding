using System.Diagnostics.CodeAnalysis;
using Tsa.Submissions.Coding.WebApi.Models;
using Xunit;

namespace Tsa.Submissions.Coding.UnitTests.WebApi.Models;

[ExcludeFromCodeCoverage]
public class TeamModelTest
{
    [Fact]
    [Trait("TestCategory", "UnitTest")]
    public void TeamId_Should_Return_A_Value()
    {
        // Arrange
        var teamNumber = "001";
        var schoolNumber = "1234";
        var teamId = $"{schoolNumber}-{teamNumber}";

        // Act
        var team = new TeamModel
        {
            SchoolNumber = schoolNumber,
            TeamNumber = teamNumber
        };

        // Assert
        Assert.Equal(teamId, team.TeamId);
    }

    [Fact]
    [Trait("TestCategory", "UnitTest")]
    public void TeamId_Should_Return_Null_When_SchoolNumber_Is_Null()
    {
        // Arrange and Act
        var team = new TeamModel
        {
            SchoolNumber = null,
            TeamNumber = "001"
        };

        // Assert
        Assert.Null(team.TeamId);
    }

    [Fact]
    [Trait("TestCategory", "UnitTest")]
    public void TeamId_Should_Return_Null_When_TeamNumber_Is_Null()
    {
        // Arrange and Act
        var team = new TeamModel
        {
            SchoolNumber = "1234",
            TeamNumber = null
        };

        // Assert
        Assert.Null(team.TeamId);
    }
}
