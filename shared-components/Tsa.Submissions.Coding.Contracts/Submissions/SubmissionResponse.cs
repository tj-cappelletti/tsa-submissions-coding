using System.Text.Json.Serialization;
using Tsa.Submissions.Coding.Contracts.Languages;
using Tsa.Submissions.Coding.Contracts.Problems;
using Tsa.Submissions.Coding.Contracts.TestCases;
using Tsa.Submissions.Coding.Contracts.Users;

namespace Tsa.Submissions.Coding.Contracts.Submissions;

public record SubmissionResponse
{
    [JsonPropertyName("evaluatedOn")]
    public DateTimeOffset? EvaluatedOn { get; set; }

    [JsonPropertyName("id")]
    public string Id { get; init; }

    [JsonPropertyName("programmingLanguage")]
    public ProgrammingLanguageResponse ProgrammingLanguage { get; init; }

    [JsonPropertyName("programmingLanguageVersionTag")]
    public string ProgrammingLanguageVersionTag { get; init; }

    [JsonPropertyName("problem")]
    public ProblemListResponse Problem { get; set; }

    [JsonPropertyName("solution")]
    public string Solution { get; init; }

    [JsonPropertyName("submittedOn")]
    public DateTimeOffset SubmittedOn { get; init; }

    [JsonPropertyName("testCaseResults")]
    public List<TestCaseResultResponse>? TestCaseResults { get; init; }

    [JsonPropertyName("user")]
    public UserResponse User { get; set; }

    public SubmissionResponse(
        string id,
        UserResponse user,
        ProgrammingLanguageResponse programmingLanguage,
        string programmingLanguageVersionTag,
        ProblemListResponse problem,
        string solution,
        DateTimeOffset submittedOn,
        DateTimeOffset? evaluatedOn,
        List<TestCaseResultResponse>? testCaseResults = null
    )
    {
        EvaluatedOn = evaluatedOn;
        Id = id;
        ProgrammingLanguage = programmingLanguage;
        ProgrammingLanguageVersionTag = programmingLanguageVersionTag;
        Problem = problem;
        Solution = solution;
        SubmittedOn = submittedOn;
        TestCaseResults = testCaseResults;
        User = user;
    }
}
