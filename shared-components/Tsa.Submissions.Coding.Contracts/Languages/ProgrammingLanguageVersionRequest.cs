using System.Text.Json.Serialization;

namespace Tsa.Submissions.Coding.Contracts.Languages;

public record ProgrammingLanguageVersionRequest
{
    [JsonPropertyName("displayName")]
    public string DisplayName { get; set; }

    [JsonPropertyName("isDefault")]
    public bool IsDefault { get; set; }

    [JsonPropertyName("versionTag")]
    public string VersionTag { get; set; }

    public ProgrammingLanguageVersionRequest(string displayName, bool isDefault, string versionTag)
    {
        DisplayName = displayName;
        IsDefault = isDefault;
        VersionTag = versionTag;
    }
}
