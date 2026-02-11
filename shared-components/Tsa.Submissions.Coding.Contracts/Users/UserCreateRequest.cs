using System.Text.Json.Serialization;

namespace Tsa.Submissions.Coding.Contracts.Users;

public record UserCreateRequest : IUserRequest
{
    [JsonPropertyName("participants")]
    public List<string>? Participants { get; init; }

    [JsonPropertyName("password")]
    public string Password { get; init; }

    [JsonPropertyName("role")]
    public string Role { get; init; }

    [JsonPropertyName("team")]
    public TeamRequest? Team { get; init; }

    [JsonPropertyName("userName")]
    public string UserName { get; init; }

    public UserCreateRequest(
        string userName,
        string password,
        string role,
        TeamRequest? team,
        List<string>? participants)
    {
        Participants = participants;
        Password = password;
        Role = role;
        Team = team;
        UserName = userName;
    }
}
