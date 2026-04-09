using System.Text.Json.Serialization;

namespace Tsa.Submissions.Coding.Contracts.ProblemLanguageVariants;

public record WorkspaceFileRequest
{
    [JsonPropertyName("contents")]
    public string? Contents { get; init; }

    [JsonPropertyName("isTemplate")]
    public bool? IsTemplate { get; init; }

    [JsonPropertyName("path")]
    public string Path { get; init; }

    [JsonPropertyName("source")]
    public string? Source { get; init; }

    [JsonPropertyName("type")]
    public string Type { get; init; }

    public WorkspaceFileRequest(string type, string path, string? contents = null, string? source = null, bool? isTemplate = null)
    {
        Contents = contents;
        IsTemplate = isTemplate;
        Path = path;
        Source = source;
        Type = type;
    }
}
