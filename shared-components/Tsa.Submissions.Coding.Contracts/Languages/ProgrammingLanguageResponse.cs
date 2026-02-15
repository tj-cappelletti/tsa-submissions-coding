using System.Text.Json.Serialization;

namespace Tsa.Submissions.Coding.Contracts.Languages;

public record ProgrammingLanguageResponse
{
    [JsonPropertyName("fileExtension")]
    public string FileExtension { get; set; }

    [JsonPropertyName("id")]
    public string Id { get; set; }

    [JsonPropertyName("identifier")]
    public string Identifier { get; set; }

    [JsonPropertyName("isEnabled")]
    public bool IsEnabled { get; set; } = true;

    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("versions")]
    public List<ProgrammingLanguageVersionResponse> Versions { get; set; } = [];

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
