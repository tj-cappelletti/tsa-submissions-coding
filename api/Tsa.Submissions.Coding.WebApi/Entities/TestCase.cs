using System.Collections.Generic;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Tsa.Submissions.Coding.WebApi.Entities;

public class TestCase : IMongoDbEntity
{
    public string? ExpectedOutput { get; set; }

    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; }

    public List<TestCaseInput> Inputs { get; set; } = [];

    public bool IsActive { get; set; }

    public bool IsPublic { get; set; }

    public string? Name { get; set; }

    public string? OutputDataType { get; set; }

    public bool OutputIsArray { get; set; }

    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? ProblemId { get; set; }

    public string? Signature { get; set; }
}
