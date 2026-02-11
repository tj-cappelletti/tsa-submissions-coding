using System.Text.Json.Serialization;

namespace Tsa.Submissions.Coding.Contracts.Users;

public record UserModifyRequest : IUserRequest
{
    [JsonPropertyName("participants")]
    public List<string>? Participants { get; init; }

    [JsonPropertyName("password")]
    public string? Password { get; init; }

    [JsonPropertyName("role")]
    public string Role { get; init; }

    [JsonPropertyName("team")]
    public TeamRequest? Team { get; init; }

    public UserModifyRequest(
        string? password,
        string role,
        TeamRequest? team,
        List<string>? participants)
    {
        Participants = participants;
        Password = password;
        Role = role;
        Team = team;
    }
}
