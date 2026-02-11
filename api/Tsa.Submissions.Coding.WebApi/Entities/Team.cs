namespace Tsa.Submissions.Coding.WebApi.Entities;

public class Team
{
    public CompetitionLevel CompetitionLevel { get; set; }

    public string SchoolNumber { get; set; }

    public string TeamNumber { get; set; }

    public Team(CompetitionLevel competitionLevel, string schoolNumber, string teamNumber)
    {
        CompetitionLevel = competitionLevel;
        SchoolNumber = schoolNumber;
        TeamNumber = teamNumber;
    }
}
