using System.Text.Json.Serialization;

namespace Tsa.Submissions.Coding.Contracts.Problems;

public record ProblemListResponse
{
    [JsonPropertyName("id")]
    public string Id { get; init; }

    [JsonPropertyName("isActive")]
    public bool IsActive { get; init; }

    [JsonPropertyName("title")]
    public string Title { get; init; }

    public ProblemListResponse(
        string id,
        string title,
        bool isActive)
    {
        Id = id;
        Title = title;
        IsActive = isActive;
    }
}
