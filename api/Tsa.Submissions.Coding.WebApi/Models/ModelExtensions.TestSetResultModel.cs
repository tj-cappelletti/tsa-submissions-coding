using System.Collections.Generic;
using System.Linq;
using MongoDB.Driver;
using Tsa.Submissions.Coding.WebApi.Entities;
using Tsa.Submissions.Coding.WebApi.Services;

namespace Tsa.Submissions.Coding.WebApi.Models;

public static partial class ModelExtensions
{
    public static IList<TestSetResult>? ToEntities(this IList<TestSetResultModel>? testSetResultModels)
    {
        return testSetResultModels?.Select(testSetResultModel => testSetResultModel.ToEntity()).ToList();
    }

    public static TestSetResult ToEntity(this TestSetResultModel testSetResultModel)
    {
        return new TestSetResult
        {
            Passed = testSetResultModel.Passed,
            RunDuration = testSetResultModel.RunDuration,
            TestSet = new MongoDBRef(TestSetsService.MongoDbCollectionName, testSetResultModel.TestSetId)
        };
    }
}
