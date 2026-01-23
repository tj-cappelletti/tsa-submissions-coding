using System.Collections.Generic;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Swashbuckle.AspNetCore.Annotations;
using Tsa.Submissions.Coding.Contracts.TestCases;

namespace Tsa.Submissions.Coding.WebApi.Entities;

public class Problem : IMongoDbEntity
{
    public string? Description { get; set; }

    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    [SwaggerSchema(ReadOnly = true)]
    public string? Id { get; set; }

    public bool IsActive { get; set; }

    public string? Title { get; set; }

    public List<TestCase> TestCases { get; set; } = [];
}
