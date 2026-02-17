using System.Text.Json.Serialization;

namespace Tsa.Submissions.Coding.Contracts.Languages;

public record ProgrammingLanguageRequest
{
    [JsonPropertyName("fileExtension")]
    public string FileExtension { get; set; }

    [JsonPropertyName("identifier")]
    public string Identifier { get; set; }

    [JsonPropertyName("isEnabled")]
    public bool IsEnabled { get; set; } = true;

    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

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
