namespace Tsa.Submissions.Coding.WebApi.Entities;

public class Participant
{
    public string? ParticipantId => string.IsNullOrWhiteSpace(SchoolNumber) || string.IsNullOrWhiteSpace(ParticipantNumber)
        ? null
        : $"{SchoolNumber}-{ParticipantNumber}";

    public string? ParticipantNumber { get; set; }

    public string? SchoolNumber { get; set; }
}
