using MongoDB.Driver;
using Tsa.Submissions.Coding.WebApi.Entities;
using Tsa.Submissions.Coding.WebApi.Services;

namespace Tsa.Submissions.Coding.WebApi.Models;

public static partial class ModelExtensions
{
    public static TestSet ToEntity(this TestSetModel testSetModel)
    {
        return new TestSet
        {
            Id = testSetModel.Id,
            Inputs = testSetModel.Inputs.ToEntities(),
            IsPublic = testSetModel.IsPublic,
            Name = testSetModel.Name,
            Problem = new MongoDBRef(ProblemsService.MongoDbCollectionName, testSetModel.ProblemId)
        };
    }
}
