using System;
using Tsa.Submissions.Coding.WebApi.Entities;

namespace Tsa.Submissions.Coding.WebApi.Models;

public static partial class ModelExtensions
{
    public static Team ToEntity(this TeamModel teamModel)
    {
        return new Team
        {
            // CompetitionLevel is required, if null, we are in a bad state
            // TeamModelValidator will ensure that this is not null
            CompetitionLevel = Enum.Parse<CompetitionLevel>(teamModel.CompetitionLevel!),
            SchoolNumber = teamModel.SchoolNumber,
            TeamNumber = teamModel.TeamNumber
        };
    }
}
