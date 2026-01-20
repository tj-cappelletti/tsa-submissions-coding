using System;
using Tsa.Submissions.Coding.Contracts.Users;

namespace Tsa.Submissions.Coding.WebApi.Models;

[Obsolete($"This model is obsolete and should be replaced with {nameof(TeamResponse)}")]
public class TeamModel
{
    public string? CompetitionLevel { get; set; }

    public string? SchoolNumber { get; set; }

    public string? TeamId => string.IsNullOrWhiteSpace(SchoolNumber) || string.IsNullOrWhiteSpace(TeamNumber)
        ? null
        : $"{SchoolNumber}-{TeamNumber}";

    public string? TeamNumber { get; set; }
}
