using System.Text.Json.Serialization;

namespace Tsa.Submissions.Coding.Contracts.Problems;

public record ProblemRequest
{
    [JsonPropertyName("description")]
    public string Description { get; init; }

    [JsonPropertyName("isActive")]
    public bool IsActive { get; init; }

    [JsonPropertyName("title")]
    public string Title { get; init; }

    public ProblemRequest(
        string title,
        string description,
        bool isActive)
    {
        Description = description;
        IsActive = isActive;
        Title = title;
    }
}
