namespace Tsa.Submissions.Coding.Contracts.Users;

public record UserCreateRequest(
    string Password,
    string Role,
    TeamRequest? Team,
    string UserName) : IUserRequest
{
    public string Password { get; } = Password;

    public string Role { get; } = Role;

    public TeamRequest? Team { get; } = Team;

    public string UserName { get; } = UserName;
}
