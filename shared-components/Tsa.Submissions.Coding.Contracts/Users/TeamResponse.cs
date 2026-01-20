namespace Tsa.Submissions.Coding.Contracts.Users;

public record TeamResponse(
    string CompetitionLevel,
    string SchoolNumber,
    string TeamNumber)
{
    public string? TeamId => string.IsNullOrWhiteSpace(SchoolNumber) || string.IsNullOrWhiteSpace(TeamNumber)
        ? null
        : $"{SchoolNumber}-{TeamNumber}";
}
