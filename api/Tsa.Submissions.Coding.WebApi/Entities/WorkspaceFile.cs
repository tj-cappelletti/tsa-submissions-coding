using MongoDB.Bson.Serialization.Attributes;

namespace Tsa.Submissions.Coding.WebApi.Entities;

public class WorkspaceFile
{
    [BsonIgnoreIfNull]
    public string? Contents { get; set; }

    [BsonIgnoreIfNull]
    public bool? IsTemplate { get; set; }

    public string? Path { get; set; }

    [BsonIgnoreIfNull]
    public string? Source { get; set; }

    public string? Type { get; set; }
}
