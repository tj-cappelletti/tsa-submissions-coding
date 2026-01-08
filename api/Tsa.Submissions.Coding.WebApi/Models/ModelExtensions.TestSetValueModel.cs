using System.Collections.Generic;
using System.Linq;
using Tsa.Submissions.Coding.WebApi.Entities;

namespace Tsa.Submissions.Coding.WebApi.Models;

public static partial class ModelExtensions
{
    public static IList<TestSetValue>? ToEntities(this IList<TestSetValueModel>? testSetInputModels)
    {
        return testSetInputModels?.Select(testSetInputModel => testSetInputModel.ToEntity()).ToList();
    }

    public static TestSetValue ToEntity(this TestSetValueModel testSetValueModel)
    {
        return new TestSetValue
        {
            DataType = testSetValueModel.DataType,
            Index = testSetValueModel.Index,
            IsArray = testSetValueModel.IsArray,
            ValueAsJson = testSetValueModel.ValueAsJson
        };
    }
}
