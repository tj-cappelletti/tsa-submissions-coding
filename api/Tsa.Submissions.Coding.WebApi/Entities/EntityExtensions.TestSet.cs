using System;
using System.Collections.Generic;
using System.Linq;
using Tsa.Submissions.Coding.Contracts.TestSets;

namespace Tsa.Submissions.Coding.WebApi.Entities;

public static partial class EntityExtensions
{
    private static IEnumerable<TestSetResponse> TestSetsToTestSetResponses(IEnumerable<TestSet> testSetValues)
    {
        return testSetValues.Select(testSetValue => testSetValue.ToResponse());
    }

    public static TestSetResponse ToResponse(this TestSet testSet)
    {
        if (string.IsNullOrWhiteSpace(testSet.Id)) throw new ArgumentException("Test Set ID is required.", nameof(testSet));

        if (testSet.Problem == null) throw new ArgumentException("Test Set Problem is required.", nameof(testSet));

        return new TestSetResponse(
            testSet.Id,
            testSet.Inputs?.ToResponses() ?? [],
            testSet.IsPublic,
            testSet.Name,
            testSet.Problem.Id.AsString);
    }

    public static IEnumerable<TestSetResponse> ToResponses(this IEnumerable<TestSet> testSets)
    {
        return TestSetsToTestSetResponses(testSets);
    }
}
