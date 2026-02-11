using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Tsa.Submissions.Coding.Contracts.Users;

namespace Tsa.Submissions.Coding.UnitTests.Helpers;

[ExcludeFromCodeCoverage]
internal class TeamModelEqualityComparer : IEqualityComparer<TeamResponse?>, IEqualityComparer<IList<TeamResponse>?>
{
    public bool Equals(TeamResponse? x, TeamResponse? y)
    {
        if (ReferenceEquals(x, y)) return true;
        if (x is null) return false;
        if (y is null) return false;

        var competitionLevelsMatch = x.CompetitionLevel == y.CompetitionLevel;
        var schoolNumbersMatch = x.SchoolNumber == y.SchoolNumber;
        var teamNumbersMatch = x.TeamNumber == y.TeamNumber;

        return competitionLevelsMatch &&
               schoolNumbersMatch &&
               teamNumbersMatch;
    }

    public bool Equals(IList<TeamResponse>? x, IList<TeamResponse>? y)
    {
        if (ReferenceEquals(x, y)) return true;
        if (x is null) return false;
        if (y is null) return false;
        if (x.Count != y.Count) return false;

        foreach (var leftTeamModel in x)
        {
            var rightTeamModel = y.SingleOrDefault(teamModel => teamModel.SchoolNumber == leftTeamModel.SchoolNumber);

            if (!Equals(leftTeamModel, rightTeamModel)) return false;
        }

        return true;
    }

    public int GetHashCode(TeamResponse? obj)
    {
        return obj == null ? 0 : obj.GetHashCode();
    }

    public int GetHashCode(IList<TeamResponse>? obj)
    {
        return obj == null ? 0 : obj.GetHashCode();
    }
}
