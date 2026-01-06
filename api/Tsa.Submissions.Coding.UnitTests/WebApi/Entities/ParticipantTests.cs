using System.Diagnostics.CodeAnalysis;
using Tsa.Submissions.Coding.WebApi.Entities;
using Xunit;

namespace Tsa.Submissions.Coding.UnitTests.WebApi.Entities;

[ExcludeFromCodeCoverage]
public class ParticipantTests
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
        var participant = new Participant
        {
            ParticipantNumber = participantNumber,
            SchoolNumber = schoolNumber
        };

        // Assert
        Assert.Equal(participantId, participant.ParticipantId);
    }

    [Fact]
    [Trait("TestCategory", "UnitTest")]
    public void ParticipantId_Should_Return_Null_When_ParticipantNumber_Is_Null()
    {
        // Arrange and Act
        var participant = new Participant
        {
            ParticipantNumber = null,
            SchoolNumber = "1234"
        };

        // Assert
        Assert.Null(participant.ParticipantId);
    }

    [Fact]
    [Trait("TestCategory", "UnitTest")]
    public void ParticipantId_Should_Return_Null_When_SchoolNumber_Is_Null()
    {
        // Arrange and Act
        var participant = new Participant
        {
            ParticipantNumber = "001",
            SchoolNumber = null
        };

        // Assert
        Assert.Null(participant.ParticipantId);
    }
}
