namespace Tsa.Submissions.Coding.WebApi.Authorization;

public static class SubmissionRoles
{
    public const string All = $"{Judge},{Participant},{System}";

    public const string Judge = "judge";

    public const string JudgeOrParticipant = $"{Judge},{Participant}";

    public const string JudgeOrSystem = $"{Judge},{System}";

    public const string Participant = "participant";

    public const string System = "system";
}
