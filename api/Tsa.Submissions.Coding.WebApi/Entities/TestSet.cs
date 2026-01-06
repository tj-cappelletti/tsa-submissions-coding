using System.Collections.Generic;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;
using Swashbuckle.AspNetCore.Annotations;

namespace Tsa.Submissions.Coding.WebApi.Entities;

public class TestSet : IMongoDbEntity
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    [SwaggerSchema(ReadOnly = true)]
    public string? Id { get; set; }

    public IList<TestSetValue>? Inputs { get; set; }

    public bool IsPublic { get; set; }

    public string? Name { get; set; }

    public MongoDBRef? Problem { get; set; }
}
