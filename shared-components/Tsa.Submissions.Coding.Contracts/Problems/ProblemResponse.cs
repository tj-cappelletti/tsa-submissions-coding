using System.Text.Json.Serialization;
using Tsa.Submissions.Coding.Contracts.TestCases;

namespace Tsa.Submissions.Coding.Contracts.Problems;

public record ProblemResponse
{
    [JsonPropertyName("description")]
    public string Description { get; init; }

    [JsonPropertyName("id")]
    public string Id { get; init; }

    [JsonPropertyName("isActive")]
    public bool IsActive { get; init; }

    [JsonPropertyName("testCases")]
    public List<TestCase> TestCases { get; init; }

    [JsonPropertyName("title")]
    public string Title { get; init; }

    public ProblemResponse(string id, string title, string description, bool isActive) :
        this(id, title, description, isActive, []) { }

    [JsonConstructor]
    public ProblemResponse(string id, string title, string description, bool isActive, List<TestCase> testCases)
    {
        Id = id;
        Title = title;
        Description = description;
        IsActive = isActive;
        TestCases = testCases;
    }
}
