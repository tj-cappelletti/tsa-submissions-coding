namespace Tsa.Submissions.Coding.WebApi.Authorization;

public static class SubmissionRoles
{
    public const string Judge = "judge";

    public const string Participant = "participant";

    public const string All = $"{Judge},{Participant}";
}
