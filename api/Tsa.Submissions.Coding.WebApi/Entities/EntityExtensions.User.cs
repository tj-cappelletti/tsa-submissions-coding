using System.Collections.Generic;
using System.Linq;
using Tsa.Submissions.Coding.WebApi.Models;

namespace Tsa.Submissions.Coding.WebApi.Entities;

public static partial class EntityExtensions
{
    public static UserModel ToModel(this User user)
    {
        var userModel = new UserModel
        {
            Id = user.Id,
            Role = user.Role,
            UserName = user.UserName,
            Team = user.Team?.ToModel()
        };

        return userModel;
    }

    public static List<UserModel> ToModels(this IList<User> users)
    {
        return UsersToUserModels(users);
    }

    public static List<UserModel> ToModels(this IEnumerable<User> users)
    {
        return UsersToUserModels(users);
    }

    private static List<UserModel> UsersToUserModels(this IEnumerable<User> users)
    {
        return [.. users.Select(user => user.ToModel())];
    }
}
