using System;
using System.Diagnostics.CodeAnalysis;
using Tsa.Submissions.Coding.Contracts.Users;

namespace Tsa.Submissions.Coding.UnitTests.Helpers.Users;

[ExcludeFromCodeCoverage]
internal class UserResponseEqualityComparer : EqualityComparerBase<UserResponse>
{
    protected override bool EqualsCore(UserResponse x, UserResponse y)
    {
        var idsMatch = x.Id == y.Id;
        var participantsMatch = new StringEqualityComparer().Equals(x.Participants, y.Participants);
        var rolesMatch = x.Role == y.Role;
        var teamsMatch = new TeamResponseEqualityComparer().Equals(x.Team, y.Team);
        var userNamesMatch = x.UserName == y.UserName;

        return idsMatch && participantsMatch && rolesMatch && teamsMatch && userNamesMatch;
    }

    public override int GetHashCode(UserResponse? obj)
    {
        return obj == null ? 0 : HashCode.Combine(obj.Id, obj.Participants, obj.Role, obj.Team, obj.UserName);
    }

    protected override Func<UserResponse, bool> GetItemPredicate(UserResponse item)
    {
        return userResponse => userResponse.Id == item.Id;
    }

    protected override object GetOrderByKey(UserResponse item)
    {
        return item.Id;
    }
}
