using System;
using System.Collections.Generic;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;
using Swashbuckle.AspNetCore.Annotations;
using Tsa.Submissions.Coding.Contracts.TestCases;

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

    public List<TestCaseResult> TestCaseResults { get; set; } = [];

    public MongoDBRef? User { get; set; }
}
