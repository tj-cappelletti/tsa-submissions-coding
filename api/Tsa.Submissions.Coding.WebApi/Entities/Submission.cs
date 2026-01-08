using System;
using System.Collections.Generic;
using MongoDB.Driver;

namespace Tsa.Submissions.Coding.WebApi.Entities;

public class Submission : IMongoDbEntity
{
    public string? Id { get; set; }

    public bool IsFinalSubmission { get; set; }

    public string? Language { get; set; }

    public MongoDBRef? Problem { get; set; }

    public string? Solution { get; set; }

    public DateTime? SubmittedOn { get; set; }

    public MongoDBRef? User { get; set; }

    public IList<TestSetResult>? TestSetResults { get; set; }
}
