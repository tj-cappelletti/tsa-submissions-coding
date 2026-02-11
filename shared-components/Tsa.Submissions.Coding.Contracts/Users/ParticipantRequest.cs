using System.Text.Json.Serialization;

namespace Tsa.Submissions.Coding.Contracts.Users;

public record ParticipantRequest
{
    [JsonPropertyName("participantNumber")]
    public string ParticipantNumber { get; init; }

    [JsonPropertyName("schoolNumber")]
    public string SchoolNumber { get; init; }

    public ParticipantRequest(string schoolNumber, string participantNumber)
    {
        ParticipantNumber = participantNumber;
        SchoolNumber = schoolNumber;
    }
}
