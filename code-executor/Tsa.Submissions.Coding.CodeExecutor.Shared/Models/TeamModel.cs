namespace Tsa.Submissions.Coding.CodeExecutor.Shared.Models;

public class TeamModel
{
    public string? CompetitionLevel { get; set; }

    public string? SchoolNumber { get; set; }

    public string? TeamId => string.IsNullOrWhiteSpace(SchoolNumber) || string.IsNullOrWhiteSpace(TeamNumber)
        ? null
        : $"{SchoolNumber}-{TeamNumber}";

    public string? TeamNumber { get; set; }
}
