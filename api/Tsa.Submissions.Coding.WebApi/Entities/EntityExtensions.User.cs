using System;
using System.Collections.Generic;
using System.Linq;
using Tsa.Submissions.Coding.Contracts.Users;

namespace Tsa.Submissions.Coding.WebApi.Entities;

public static partial class EntityExtensions
{
    public static UserResponse ToResponse(this User user)
    {
        if (string.IsNullOrWhiteSpace(user.Id)) throw new InvalidOperationException("User ID cannot be null or empty.");

        if (string.IsNullOrWhiteSpace(user.Role)) throw new InvalidOperationException("User Role cannot be null or empty.");

        if (string.IsNullOrWhiteSpace(user.UserName)) throw new InvalidOperationException("User UserName cannot be null or empty.");

        return new UserResponse(
            user.Id,
            user.Role,
            user.Team?.ToResponse(),
            user.UserName);
    }

    public static IEnumerable<UserResponse> ToResponses(this IEnumerable<User> users)
    {
        return users.Select(user => user.ToResponse());
    }
}
