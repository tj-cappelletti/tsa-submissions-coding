using Tsa.Submissions.Coding.Contracts.Users;

namespace Tsa.Submissions.Coding.WebApi.Entities;

public static partial class EntityExtensions
{
    public static TeamResponse ToResponse(this Team team)
    {
        return new TeamResponse(
            team.CompetitionLevel.ToString(),
            team.SchoolNumber,
            team.TeamNumber);
    }
}
