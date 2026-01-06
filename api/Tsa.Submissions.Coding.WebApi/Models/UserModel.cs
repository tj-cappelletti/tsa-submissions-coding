namespace Tsa.Submissions.Coding.WebApi.Models;

public class UserModel
{
    public string? Id { get; set; }

    public string? Password { get; set; }

    public string? Role { get; set; }

    public TeamModel? Team { get; set; }

    public string? UserName { get; set; }
}
