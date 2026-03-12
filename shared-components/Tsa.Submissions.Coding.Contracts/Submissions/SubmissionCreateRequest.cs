using System.Text.Json.Serialization;

namespace Tsa.Submissions.Coding.Contracts.Submissions;

public record SubmissionCreateRequest
{
    [JsonPropertyName("problemId")]
    public string ProblemId { get; init; }

    [JsonPropertyName("programmingLanguageId")]
    public string ProgrammingLanguageId { get; init; }

    public string ProgrammingLanguageVersionTag { get; init; }

    [JsonPropertyName("solution")]
    public string Solution { get; init; }

    public SubmissionCreateRequest(string problemId, string programmingLanguageId, string programmingLanguageVersionTag, string solution)
    {
        ProblemId = problemId;
        ProgrammingLanguageId = programmingLanguageId;
        ProgrammingLanguageVersionTag = programmingLanguageVersionTag;
        Solution = solution;
    }
}
