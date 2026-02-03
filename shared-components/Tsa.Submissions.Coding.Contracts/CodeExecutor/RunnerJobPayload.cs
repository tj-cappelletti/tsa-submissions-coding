using System.Text.Json.Serialization;
using Tsa.Submissions.Coding.Contracts.TestCases;

namespace Tsa.Submissions.Coding.Contracts.CodeExecutor;

public record RunnerJobPayload
{
    [JsonPropertyName("language")]
    public string Language { get; init; }

    [JsonPropertyName("languageFixture")]
    public string LanguageFixture { get; init; }

    [JsonPropertyName("languageVersion")]
    public string LanguageVersion { get; init; }

    [JsonPropertyName("problemId")]
    public string ProblemId { get; init; }

    [JsonPropertyName("solution")]
    public string Solution { get; init; }

    [JsonPropertyName("submissionId")]
    public string SubmissionId { get; init; }

    [JsonPropertyName("testCases")]
    public List<TestCase> TestCases { get; init; }

    public RunnerJobPayload(
        string language,
        string languageFixture,
        string languageVersion,
        string problemId,
        string solution,
        string submissionId,
        List<TestCase> testCases)
    {
        Language = language;
        LanguageFixture = languageFixture;
        LanguageVersion = languageVersion;
        ProblemId = problemId;
        Solution = solution;
        SubmissionId = submissionId;
        TestCases = testCases;
    }
}
