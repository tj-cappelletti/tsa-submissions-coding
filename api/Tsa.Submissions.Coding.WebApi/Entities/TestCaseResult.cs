using System;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Tsa.Submissions.Coding.WebApi.Entities;

public class TestCaseResult
{
    public string? ActualOutput { get; set; }

    public TimeSpan ExecutionTime { get; set; } = TimeSpan.Zero;

    public string? Message { get; set; }

    public bool Passed { get; set; }

    [BsonRepresentation(BsonType.ObjectId)]
    public string? TestCaseId { get; set; }

    public bool TimedOut { get; set; }
}
