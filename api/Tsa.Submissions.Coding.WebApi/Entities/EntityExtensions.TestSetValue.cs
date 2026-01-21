using System;
using System.Collections.Generic;
using System.Linq;
using Tsa.Submissions.Coding.Contracts.TestSets;

namespace Tsa.Submissions.Coding.WebApi.Entities;

public static partial class EntityExtensions
{
    public static TestSetValueResponse ToResponse(this TestSetValue testSetValue)
    {
        if (string.IsNullOrWhiteSpace(testSetValue.DataType)) throw new InvalidOperationException("Test Set Value Data Type is required.");

        if (testSetValue.Index == null) throw new InvalidOperationException("Test Set Value Index is required.");

        if (string.IsNullOrWhiteSpace(testSetValue.ValueAsJson)) throw new InvalidOperationException("Test Set Value JSON is required.");

        return new TestSetValueResponse(
            testSetValue.DataType,
            testSetValue.Index.Value,
            testSetValue.IsArray,
            testSetValue.ValueAsJson);
    }

    public static List<TestSetValueResponse> ToResponses(this IEnumerable<TestSetValue> testSetValues)
    {
        return testSetValues.Select(testSetValue => testSetValue.ToResponse()).ToList();
    }
}
