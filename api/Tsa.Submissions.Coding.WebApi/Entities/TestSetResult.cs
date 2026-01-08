using System;
using MongoDB.Driver;

namespace Tsa.Submissions.Coding.WebApi.Entities;

public class TestSetResult
{
    public bool Passed { get; set; }

    public TimeSpan? RunDuration { get; set; }

    public MongoDBRef? TestSet { get; set; }
}
