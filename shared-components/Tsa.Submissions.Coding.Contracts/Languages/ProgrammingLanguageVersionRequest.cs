using System.Text.Json.Serialization;

namespace Tsa.Submissions.Coding.Contracts.Languages;

public record ProgrammingLanguageVersionRequest
{
    [JsonPropertyName("displayName")]
    public string DisplayName { get; init; }

    [JsonPropertyName("isDefault")]
    public bool IsDefault { get; init; }

    [JsonPropertyName("versionTag")]
    public string VersionTag { get; init; }

    public ProgrammingLanguageVersionRequest(string displayName, bool isDefault, string versionTag)
    {
        DisplayName = displayName;
        IsDefault = isDefault;
        VersionTag = versionTag;
    }
}
