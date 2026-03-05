using System;
using System.Collections.Generic;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Tsa.Submissions.Coding.Contracts.TestCases;

namespace Tsa.Submissions.Coding.WebApi.Entities;

public class Submission : IMongoDbEntity
{
    public DateTimeOffset? EvaluatedOn { get; set; }

    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; }

    [BsonRepresentation(BsonType.ObjectId)]
    public string? ProblemId { get; set; }

    [BsonRepresentation(BsonType.ObjectId)]
    public string? ProgrammingLanguageId { get; set; }

    public string? ProgrammingLanguageVersionTag { get; set; }

    public SubmissionScorecard? Scorecard { get; set; }

    public string? Solution { get; set; }

    public DateTimeOffset? SubmittedOn { get; set; }

    public List<TestCaseResult> TestCaseResults { get; set; } = [];

    [BsonRepresentation(BsonType.ObjectId)]
    public string? UserId { get; set; }
}
