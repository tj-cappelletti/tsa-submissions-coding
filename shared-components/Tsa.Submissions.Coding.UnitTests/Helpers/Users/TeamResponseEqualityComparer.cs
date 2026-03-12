using System;
using System.Diagnostics.CodeAnalysis;
using Tsa.Submissions.Coding.Contracts.Users;

namespace Tsa.Submissions.Coding.UnitTests.Helpers.Users;

[ExcludeFromCodeCoverage]
internal class TeamResponseEqualityComparer : EqualityComparerBase<TeamResponse>
{
    protected override bool EqualsCore(TeamResponse x, TeamResponse y)
    {
        var competitionLevelsMatch = x.CompetitionLevel == y.CompetitionLevel;
        var schoolNumbersMatch = x.SchoolNumber == y.SchoolNumber;
        var teamNumbersMatch = x.TeamNumber == y.TeamNumber;

        return competitionLevelsMatch && schoolNumbersMatch && teamNumbersMatch;
    }
    
    public override int GetHashCode(TeamResponse? obj)
    {
        throw new NotImplementedException();
    }

    protected override Func<TeamResponse, bool> GetItemPredicate(TeamResponse item)
    {
        // CompetitionLevel is not included in the predicate because the school number also denotes the competition level.
        // For example, school number 1001 is for middle school, while school number 2001 is for high school.
        return teamResponse => teamResponse.SchoolNumber == item.SchoolNumber && teamResponse.TeamNumber == item.TeamNumber;
    }

    protected override object GetOrderByKey(TeamResponse item)
    {
        return (item.SchoolNumber, item.TeamNumber, item.CompetitionLevel);
    }
}
