using System.Collections.Generic;
using System.Linq;
using Tsa.Submissions.Coding.WebApi.Models;

namespace Tsa.Submissions.Coding.WebApi.Entities;

public static partial class EntityExtensions
{
    public static TeamModel ToModel(this Team team)
    {
        return new TeamModel
        {
            CompetitionLevel = team.CompetitionLevel.ToString(),
            SchoolNumber = team.SchoolNumber,
            TeamNumber = team.TeamNumber
        };
    }

    private static List<TeamModel> TeamsToTeamModels(IEnumerable<Team> teams)
    {
        return teams.Select(team => team.ToModel()).ToList();
    }

    public static List<TeamModel> ToModels(this IList<Team> teams)
    {
        return TeamsToTeamModels(teams);
    }

    public static List<TeamModel> ToModels(this IEnumerable<Team> teams)
    {
        return TeamsToTeamModels(teams);
    }
}
