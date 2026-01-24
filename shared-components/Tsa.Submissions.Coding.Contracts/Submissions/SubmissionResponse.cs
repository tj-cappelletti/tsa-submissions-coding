using System.Text.Json.Serialization;
using Tsa.Submissions.Coding.Contracts.TestCases;

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
    public List<TestCaseResult> TestCaseResults { get; init; }

    [JsonPropertyName("userId")]
    public string UserId { get; set; }

    public SubmissionResponse(
        string id,
        ProgrammingLanguageResponse language,
        string problemId,
        string solution,
        DateTimeOffset submittedOn,
        string userId
    ) : this(id, language, problemId, solution, submittedOn, [], userId) { }

    [JsonConstructor]
    public SubmissionResponse(
        string id,
        ProgrammingLanguageResponse language,
        string problemId,
        string solution,
        DateTimeOffset submittedOn,
        List<TestCaseResult> testCaseResults,
        string userId)
    {
        Id = id;
        Language = language;
        ProblemId = problemId;
        Solution = solution;
        SubmittedOn = submittedOn;
        TestCaseResults = testCaseResults;
        UserId = userId;
    }
}
