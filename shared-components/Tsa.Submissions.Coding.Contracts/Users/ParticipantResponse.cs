using System.Text.Json.Serialization;

namespace Tsa.Submissions.Coding.Contracts.Users;

public record ParticipantResponse
{
    [JsonPropertyName("individualNumber")]
    public string IndividualNumber { get; init; }

    [JsonPropertyName("schoolNumber")]
    public string SchoolNumber { get; init; }

    public ParticipantResponse(string schoolNumber, string individualNumber)
    {
        IndividualNumber = individualNumber;
        SchoolNumber = schoolNumber;
    }
}
