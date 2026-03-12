using Tsa.Submissions.Coding.Contracts.Users;

namespace Tsa.Submissions.Coding.Contracts.Submissions;

public record SubmissionListResponse
{
    public DateTimeOffset? EvaluatedOn { get; init; }

    public string Id { get; init; }

    public string ProblemId { get; init; }

    public string ProgrammingLanguageId { get; init; }

    public string ProgrammingLanguageVersionTag { get; init; }

    // TODO: Need to come up with a way to summarize the scorecard for a list response
    // public SubmissionScorecard? Scorecard { get; set; }

    public DateTimeOffset SubmittedOn { get; init; }

    // TODO: Need to come up with a way to summarize test case results for a list response
    // public List<TestCaseResult> TestCaseResults { get; init; } = [];

    public UserResponse User { get; init; }

    public SubmissionListResponse(
        string id,
        string problemId,
        string programmingLanguageId,
        string programmingLanguageVersionTag,
        DateTimeOffset submittedOn,
        DateTimeOffset? evaluatedOn,
        UserResponse user)
    {
        EvaluatedOn = evaluatedOn;
        Id = id;
        ProblemId = problemId;
        ProgrammingLanguageId = programmingLanguageId;
        ProgrammingLanguageVersionTag = programmingLanguageVersionTag;
        SubmittedOn = submittedOn;
        User = user;
    }
}
