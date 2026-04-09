using System.Collections.Generic;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Tsa.Submissions.Coding.WebApi.Entities;

public class ProblemLanguageVariant : IMongoDbEntity
{
    public CodeMetrics? BaselineMetrics { get; set; }

    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; }

    public bool IsActive { get; set; }

    [BsonRepresentation(BsonType.ObjectId)]
    public string? ProblemId { get; set; }

    [BsonRepresentation(BsonType.ObjectId)]
    public string? ProgrammingLanguageId { get; set; }

    public string? ProgrammingLanguageVersionTag { get; set; }

    public string? ReferenceSolution { get; set; }

    public string? StarterCode { get; set; }

    public string? TestHarnessCode { get; set; }

    public List<WorkspaceFile> WorkspaceFiles { get; set; } = [];
}
