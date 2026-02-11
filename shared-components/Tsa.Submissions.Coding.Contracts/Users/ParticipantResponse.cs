using System.Text.Json.Serialization;

namespace Tsa.Submissions.Coding.Contracts.Users;

public record ParticipantResponse
{
    [JsonPropertyName("participantNumber")]
    public string ParticipantNumber { get; init; }

    [JsonPropertyName("schoolNumber")]
    public string SchoolNumber { get; init; }

    public ParticipantResponse(string schoolNumber, string participantNumber)
    {
        ParticipantNumber = participantNumber;
        SchoolNumber = schoolNumber;
    }
}
