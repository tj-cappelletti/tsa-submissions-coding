using MongoDB.Driver;
using Tsa.Submissions.Coding.WebApi.Entities;
using Tsa.Submissions.Coding.WebApi.Services;

namespace Tsa.Submissions.Coding.WebApi.Models;

public static partial class ModelExtensions
{
    public static User ToEntity(this UserModel userModel)
    {
        var user = new User
        {
            Id = userModel.Id,
            Role = userModel.Role,
            Team = userModel.Team?.ToEntity(),
            UserName = userModel.UserName
        };

        return user;
    }
}
