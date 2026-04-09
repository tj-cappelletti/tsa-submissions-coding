using System.Text.Json.Serialization;

namespace Tsa.Submissions.Coding.Contracts.ProblemLanguageVariants;

public record ProblemLanguageVariantRequest
{
    [JsonPropertyName("baselineMetrics")]
    public CodeMetricsRequest? BaselineMetrics { get; init; }

    [JsonPropertyName("isActive")]
    public bool IsActive { get; init; }

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
    public List<WorkspaceFileRequest> WorkspaceFiles { get; init; } = [];

    public ProblemLanguageVariantRequest(
        string programmingLanguageId,
        string programmingLanguageVersionTag,
        string referenceSolution,
        string starterCode,
        string testHarnessCode,
        List<WorkspaceFileRequest> workspaceFiles,
        bool isActive,
        CodeMetricsRequest? baselineMetrics = null)
    {
        BaselineMetrics = baselineMetrics;
        IsActive = isActive;
        ProgrammingLanguageId = programmingLanguageId;
        ProgrammingLanguageVersionTag = programmingLanguageVersionTag;
        ReferenceSolution = referenceSolution;
        StarterCode = starterCode;
        TestHarnessCode = testHarnessCode;
        WorkspaceFiles = workspaceFiles;
    }
}
