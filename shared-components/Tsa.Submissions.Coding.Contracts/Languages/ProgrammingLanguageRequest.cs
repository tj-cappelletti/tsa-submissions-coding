using System.Text.Json.Serialization;

namespace Tsa.Submissions.Coding.Contracts.Languages;

public record ProgrammingLanguageRequest
{
    [JsonPropertyName("fileExtension")]
    public string FileExtension { get; init; }

    [JsonPropertyName("identifier")]
    public string Identifier { get; init; }

    [JsonPropertyName("isEnabled")]
    public bool IsEnabled { get; init; } = true;

    [JsonPropertyName("name")]
    public string Name { get; init; } = string.Empty;

    public ProgrammingLanguageRequest(
        string identifier,
        string name,
        string fileExtension,
        bool isEnabled)
    {
        FileExtension = fileExtension;
        Identifier = identifier;
        IsEnabled = isEnabled;
        Name = name;
    }
}
