using Tsa.Submissions.Coding.Contracts.Languages;
using Tsa.Submissions.Coding.Contracts.Problems;
using Tsa.Submissions.Coding.Contracts.Users;

namespace Tsa.Submissions.Coding.Contracts.Submissions;

public record SubmissionListResponse
{
    public DateTimeOffset? EvaluatedOn { get; init; }

    public string Id { get; init; }

    public ProblemListResponse Problem { get; init; }

    // Need to think about this one
    // What do we want to see in the UI when returning a list??
    public ProgrammingLanguageResponse ProgrammingLanguage { get; init; }

    public string ProgrammingLanguageVersionTag { get; init; }

    // TODO: Need to come up with a way to summarize the scorecard for a list response
    // public SubmissionScorecard? Scorecard { get; set; }

    public DateTimeOffset SubmittedOn { get; init; }

    // TODO: Need to come up with a way to summarize test case results for a list response
    // public List<TestCaseResult> TestCaseResults { get; init; } = [];

    public UserResponse User { get; init; }

    public SubmissionListResponse(
        string id,
        ProblemListResponse problem,
        ProgrammingLanguageResponse programmingLanguage,
        string programmingLanguageVersionTag,
        DateTimeOffset submittedOn,
        DateTimeOffset? evaluatedOn,
        UserResponse user)
    {
        EvaluatedOn = evaluatedOn;
        Id = id;
        Problem = problem;
        ProgrammingLanguage = programmingLanguage;
        ProgrammingLanguageVersionTag = programmingLanguageVersionTag;
        SubmittedOn = submittedOn;
        User = user;
    }
}
