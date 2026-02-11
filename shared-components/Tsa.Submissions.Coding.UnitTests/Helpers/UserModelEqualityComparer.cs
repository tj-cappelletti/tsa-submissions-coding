using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Tsa.Submissions.Coding.Contracts.Users;

namespace Tsa.Submissions.Coding.UnitTests.Helpers;

//TODO: Turn into code generator
[ExcludeFromCodeCoverage]
public class UserModelEqualityComparer : IEqualityComparer<UserResponse?>, IEqualityComparer<IList<UserResponse>?>
{
    private readonly TeamModelEqualityComparer _teamModelEqualityComparer = new();

    public bool Equals(UserResponse? x, UserResponse? y)
    {
        if (ReferenceEquals(x, y)) return true;
        if (x is null) return false;
        if (y is null) return false;

        var idsMatch = x.Id == y.Id;
        var rolesMatch = x.Role == y.Role;
        var teamsMatch = _teamModelEqualityComparer.Equals(x.Team, y.Team);
        var userNamesMatch = x.UserName == y.UserName;

        return idsMatch && rolesMatch && teamsMatch && userNamesMatch;
    }

    public bool Equals(IList<UserResponse>? x, IList<UserResponse>? y)
    {
        if (ReferenceEquals(x, y)) return true;
        if (x is null) return false;
        if (y is null) return false;
        if (x.Count != y.Count) return false;

        foreach (var leftProblem in x)
        {
            var rightProblem = y.SingleOrDefault(userModel => userModel.Id == leftProblem.Id);

            if (!Equals(leftProblem, rightProblem)) return false;
        }

        return true;
    }

    public int GetHashCode(UserResponse? obj)
    {
        return obj == null ? 0 : obj.GetHashCode();
    }

    public int GetHashCode(IList<UserResponse>? obj)
    {
        return obj == null ? 0 : obj.GetHashCode();
    }
}
