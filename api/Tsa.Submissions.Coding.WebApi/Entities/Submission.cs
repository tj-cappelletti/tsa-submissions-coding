using System;
using System.Collections.Generic;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;
using Swashbuckle.AspNetCore.Annotations;

namespace Tsa.Submissions.Coding.WebApi.Entities;

public class Submission : IMongoDbEntity
{
    public DateTimeOffset? EvaluatedOn { get; set; }

    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    [SwaggerSchema(ReadOnly = true)]
    public string? Id { get; set; }

    public ProgrammingLanguage? Language { get; set; }

    public MongoDBRef? Problem { get; set; }

    public string? Solution { get; set; }

    public DateTimeOffset? SubmittedOn { get; set; }

    public IList<TestSetResult>? TestSetResults { get; set; }

    public MongoDBRef? User { get; set; }
}
