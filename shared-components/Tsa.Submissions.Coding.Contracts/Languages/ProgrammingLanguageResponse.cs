using System.Text.Json.Serialization;

namespace Tsa.Submissions.Coding.Contracts.Languages;

public record ProgrammingLanguageResponse
{
    [JsonPropertyName("fileExtension")]
    public string FileExtension { get; init; }

    [JsonPropertyName("id")]
    public string Id { get; init; }

    [JsonPropertyName("identifier")]
    public string Identifier { get; init; }

    [JsonPropertyName("isEnabled")]
    public bool IsEnabled { get; init; } = true;

    [JsonPropertyName("name")]
    public string Name { get; init; } = string.Empty;

    [JsonPropertyName("versions")]
    public List<ProgrammingLanguageVersionResponse> Versions { get; init; } = [];

    public ProgrammingLanguageResponse(
        string id,
        string identifier,
        string name,
        string fileExtension,
        bool isEnabled,
        IEnumerable<ProgrammingLanguageVersionResponse> versions)
    {
        FileExtension = fileExtension;
        Id = id;
        Identifier = identifier;
        IsEnabled = isEnabled;
        Name = name;
        Versions = versions.ToList();
    }
}
