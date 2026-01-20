using System;
using Tsa.Submissions.Coding.Contracts.Users;

namespace Tsa.Submissions.Coding.WebApi.Models;

[Obsolete($"This model is obsolete and should be replaced with {nameof(UserResponse)}")]
public class UserModel
{
    public string? Id { get; set; }

    public string? Password { get; set; }

    public string? Role { get; set; }

    public TeamModel? Team { get; set; }

    public string? UserName { get; set; }
}
