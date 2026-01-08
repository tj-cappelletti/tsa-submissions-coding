using System.Collections.Generic;
using System.Linq;
using Tsa.Submissions.Coding.WebApi.Models;

namespace Tsa.Submissions.Coding.WebApi.Entities;

public static partial class EntityExtensions
{
    private static List<TestSetValueModel> TestSetValuesToTestSetValueModels(this IEnumerable<TestSetValue> testSetValues)
    {
        return testSetValues.Select(testSetValue => testSetValue.ToModel()).ToList();
    }

    public static TestSetValueModel ToModel(this TestSetValue testSetValue)
    {
        return new TestSetValueModel
        {
            DataType = testSetValue.DataType,
            Index = testSetValue.Index,
            IsArray = testSetValue.IsArray,
            ValueAsJson = testSetValue.ValueAsJson
        };
    }

    public static List<TestSetValueModel> ToModels(this IList<TestSetValue> testSetInputs)
    {
        return TestSetValuesToTestSetValueModels(testSetInputs);
    }

    public static List<TestSetValueModel> ToModels(this IEnumerable<TestSetValue> testSetInputs)
    {
        return TestSetValuesToTestSetValueModels(testSetInputs);
    }
}
