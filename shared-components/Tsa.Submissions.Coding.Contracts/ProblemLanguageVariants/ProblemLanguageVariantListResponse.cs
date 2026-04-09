using System.Text.Json.Serialization;

namespace Tsa.Submissions.Coding.Contracts.ProblemLanguageVariants;

// TODO: Determine what needs to be displayed in a list view for this entity and update accordingly.
// We know we need this for listing the variants for a problem (specifically for judges), but we don't have a UI to model this off of right now
// We may need to include a lightweight version of the problem and programming language (maybe their list responses?)
public record ProblemLanguageVariantListResponse
{
    [JsonPropertyName("id")]
    public string Id { get; init; }

    [JsonPropertyName("isActive")]
    public bool IsActive { get; init; }

    [JsonPropertyName("problemId")]
    public string ProblemId { get; init; }

    [JsonPropertyName("programmingLanguageId")]
    public string ProgrammingLanguageId { get; init; }

    [JsonPropertyName("programmingLanguageVersionTag")]
    public string ProgrammingLanguageVersionTag { get; init; }

    public ProblemLanguageVariantListResponse(
        string id,
        string problemId,
        string programmingLanguageId,
        string programmingLanguageVersionTag,
        bool isActive)
    {
        Id = id;
        IsActive = isActive;
        ProblemId = problemId;
        ProgrammingLanguageId = programmingLanguageId;
        ProgrammingLanguageVersionTag = programmingLanguageVersionTag;
    }
}
