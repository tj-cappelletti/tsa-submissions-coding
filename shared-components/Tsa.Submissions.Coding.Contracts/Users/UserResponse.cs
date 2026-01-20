namespace Tsa.Submissions.Coding.Contracts.Users;

public record UserResponse(
    string Id,
    string Role,
    TeamResponse? Team,
    string UserName
);
