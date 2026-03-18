using System.Text.Json.Serialization;
using Tsa.Submissions.Coding.Contracts.Languages;
using Tsa.Submissions.Coding.Contracts.TestCases;
using Tsa.Submissions.Coding.Contracts.Users;

namespace Tsa.Submissions.Coding.Contracts.Submissions;

public record SubmissionResponse
{
    [JsonPropertyName("id")]
    public string Id { get; init; }

    [JsonPropertyName("language")]
    public ProgrammingLanguageResponse Language { get; init; }

    [JsonPropertyName("problemId")]
    public string ProblemId { get; init; }

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
        ProgrammingLanguageResponse language,
        string problemId,
        string solution,
        DateTimeOffset submittedOn,
        List<TestCaseResultResponse>? testCaseResults = null
    )
    {
        Id = id;
        Language = language;
        ProblemId = problemId;
        Solution = solution;
        SubmittedOn = submittedOn;
        TestCaseResults = testCaseResults;
        User = user;
    }
}
