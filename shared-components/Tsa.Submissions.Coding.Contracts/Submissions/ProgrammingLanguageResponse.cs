using System.Text.Json.Serialization;

namespace Tsa.Submissions.Coding.Contracts.Submissions;

public record ProgrammingLanguageResponse
{
    [JsonPropertyName("name")]
    public string Name { get; init; }

    [JsonPropertyName("version")]
    public string Version { get; init; }

    [JsonConstructor]
    public ProgrammingLanguageResponse(string name, string version)
    {
        Name = name;
        Version = version;
    }
}
