using System.Text.Json.Serialization;

namespace Tsa.Submissions.Coding.Contracts.ProblemLanguageVariants;

public record ProblemLanguageVariantResponse
{
    [JsonPropertyName("baselineMetrics")]
    public CodeMetricsResponse? BaselineMetrics { get; init; }

    [JsonPropertyName("id")]
    public string Id { get; init; }

    [JsonPropertyName("isActive")]
    public bool IsActive { get; init; }

    [JsonPropertyName("problemId")]
    public string ProblemId { get; init; }

    [JsonPropertyName("programmingLanguageId")]
    public string ProgrammingLanguageId { get; init; }

    [JsonPropertyName("programmingLanguageVersionTag")]
    public string ProgrammingLanguageVersionTag { get; init; }

    [JsonPropertyName("referenceSolution")]
    public string ReferenceSolution { get; init; }

    [JsonPropertyName("starterCode")]
    public string StarterCode { get; init; }

    [JsonPropertyName("testHarnessCode")]
    public string TestHarnessCode { get; init; }

    [JsonPropertyName("workspaceFiles")]
    public List<WorkspaceFileResponse> WorkspaceFiles { get; init; } = [];

    public ProblemLanguageVariantResponse(
        string id,
        string problemId,
        string programmingLanguageId,
        string programmingLanguageVersionTag,
        string referenceSolution,
        string starterCode,
        string testHarnessCode,
        List<WorkspaceFileResponse> workspaceFiles,
        bool isActive,
        CodeMetricsResponse? baselineMetrics = null)
    {
        BaselineMetrics = baselineMetrics;
        Id = id;
        IsActive = isActive;
        ProblemId = problemId;
        ProgrammingLanguageId = programmingLanguageId;
        ProgrammingLanguageVersionTag = programmingLanguageVersionTag;
        ReferenceSolution = referenceSolution;
        StarterCode = starterCode;
        TestHarnessCode = testHarnessCode;
        WorkspaceFiles = workspaceFiles;
    }
}
