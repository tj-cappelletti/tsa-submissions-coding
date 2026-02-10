using System;
using System.Collections.Generic;
using System.Linq;
using Tsa.Submissions.Coding.Contracts.TestCases;

namespace Tsa.Submissions.Coding.WebApi.Entities;

public static partial class EntityExtensions
{
    public static IEnumerable<TestCaseInputResponse> ToResponse(this IEnumerable<TestCaseInput> testCaseInputs)
    {
        return testCaseInputs.Select(testCaseInput => testCaseInput.ToResponse());
    }

    public static TestCaseInputResponse ToResponse(this TestCaseInput testCaseInput)
    {
        if (string.IsNullOrWhiteSpace(testCaseInput.DataType)) throw new InvalidOperationException("Test case input data type is required.");

        if (string.IsNullOrWhiteSpace(testCaseInput.Value)) throw new InvalidOperationException("Test case input value is required.");

        return new TestCaseInputResponse(
            testCaseInput.DataType,
            testCaseInput.IsArray,
            testCaseInput.Value,
            testCaseInput.Index);
    }
}
