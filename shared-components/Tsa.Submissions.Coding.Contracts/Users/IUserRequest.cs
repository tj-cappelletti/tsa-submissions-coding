namespace Tsa.Submissions.Coding.Contracts.Users;

public interface IUserRequest
{
    string? Password { get; }

    string Role { get; }

    TeamRequest? Team { get; }

    string UserName { get; }
}
