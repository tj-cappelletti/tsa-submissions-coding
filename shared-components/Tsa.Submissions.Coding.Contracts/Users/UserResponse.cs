using System.Text.Json.Serialization;

namespace Tsa.Submissions.Coding.Contracts.Users;

public record UserResponse
{
    [JsonPropertyName("id")]
    public string Id { get; init; }

    [JsonPropertyName("participants")]
    public List<ParticipantResponse>? Participants { get; init; }

    [JsonPropertyName("role")]
    public string Role { get; init; }

    [JsonPropertyName("team")]
    public TeamResponse? Team { get; init; }

    [JsonPropertyName("userName")]
    public string UserName { get; init; }

    public UserResponse(
        string id,
        string userName,
        string role,
        TeamResponse? team,
        List<ParticipantResponse>? participants)
    {
        Id = id;
        Participants = participants;
        Role = role;
        Team = team;
        UserName = userName;
    }
}
