using System.Text.Json.Serialization;

namespace Tsa.Submissions.Coding.Contracts.Users;

public record TeamResponse
{
    [JsonPropertyName("competitionLevel")]
    public string CompetitionLevel { get; init; }

    [JsonPropertyName("schoolNumber")]
    public string SchoolNumber { get; init; }

    [JsonPropertyName("teamNumber")]
    public string TeamNumber { get; init; }

    public TeamResponse(string competitionLevel, string schoolNumber, string teamNumber)
    {
        CompetitionLevel = competitionLevel;
        SchoolNumber = schoolNumber;
        TeamNumber = teamNumber;
    }
}
