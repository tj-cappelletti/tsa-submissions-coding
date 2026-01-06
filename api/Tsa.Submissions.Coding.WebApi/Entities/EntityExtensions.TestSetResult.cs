using System.Collections.Generic;
using System.Linq;
using Tsa.Submissions.Coding.WebApi.Models;

namespace Tsa.Submissions.Coding.WebApi.Entities;

public static partial class EntityExtensions
{
    private static List<TestSetResultModel> TestSetsToTestSetModels(IEnumerable<TestSetResult> testSetResults)
    {
        return testSetResults.Select(testSetResult => testSetResult.ToModel()).ToList();
    }

    public static TestSetResultModel ToModel(this TestSetResult testSetResult)
    {
        return new TestSetResultModel
        {
            Passed = testSetResult.Passed,
            RunDuration = testSetResult.RunDuration,
            // TestSet is required, if null, we are in a bad state
            TestSetId = testSetResult.TestSet!.Id.AsString
        };
    }

    public static List<TestSetResultModel> ToModels(this IEnumerable<TestSetResult> testSetResults)
    {
        return TestSetsToTestSetModels(testSetResults);
    }

    public static List<TestSetResultModel> ToModels(this IList<TestSetResult> testSetResults)
    {
        return TestSetsToTestSetModels(testSetResults);
    }
}
