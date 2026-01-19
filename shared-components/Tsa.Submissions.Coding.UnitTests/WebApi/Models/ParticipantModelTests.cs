using System.Diagnostics.CodeAnalysis;
using Tsa.Submissions.Coding.WebApi.Models;
using Xunit;

namespace Tsa.Submissions.Coding.UnitTests.WebApi.Models;

[ExcludeFromCodeCoverage]
public class ParticipantModelTests
{
    [Fact]
    [Trait("TestCategory", "UnitTest")]
    public void ParticipantId_Should_Return_A_Value()
    {
        // Arrange
        var participantNumber = "001";
        var schoolNumber = "1234";
        var participantId = $"{schoolNumber}-{participantNumber}";

        // Act
        var participantModel = new ParticipantModel
        {
            ParticipantNumber = participantNumber,
            SchoolNumber = schoolNumber
        };

        // Assert
        Assert.Equal(participantId, participantModel.ParticipantId);
    }

    [Fact]
    [Trait("TestCategory", "UnitTest")]
    public void ParticipantId_Should_Return_Null_When_ParticipantNumber_Is_Null()
    {
        // Arrange and Act
        var participantModel = new ParticipantModel
        {
            ParticipantNumber = null,
            SchoolNumber = "1234"
        };

        // Assert
        Assert.Null(participantModel.ParticipantId);
    }

    [Fact]
    [Trait("TestCategory", "UnitTest")]
    public void ParticipantId_Should_Return_Null_When_SchoolNumber_Is_Null()
    {
        // Arrange and Act
        var participantModel = new ParticipantModel
        {
            ParticipantNumber = "001",
            SchoolNumber = null
        };

        // Assert
        Assert.Null(participantModel.ParticipantId);
    }
}
