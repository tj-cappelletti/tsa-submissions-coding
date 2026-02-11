using System;
using System.Collections.Generic;
using System.Linq;
using Tsa.Submissions.Coding.Contracts.TestCases;

namespace Tsa.Submissions.Coding.WebApi.Entities;

public static partial class EntityExtensions
{
    public static TestCaseResponse ToResponse(this TestCase testCase)
    {
        if (string.IsNullOrWhiteSpace(testCase.ExpectedOutput)) throw new InvalidOperationException("Expected output is required for a Test Case.");

        if (string.IsNullOrWhiteSpace(testCase.Id)) throw new InvalidOperationException("Test Case ID is required.");

        if (testCase.Inputs == null || testCase.Inputs.Count == 0) throw new InvalidOperationException("At least one input is required for a Test Case.");

        if (string.IsNullOrWhiteSpace(testCase.Name)) throw new InvalidOperationException("The name of the Test Case is required.");

        if (string.IsNullOrWhiteSpace(testCase.OutputDataType)) throw new InvalidOperationException("Test Case output data type is required.");

        if (string.IsNullOrWhiteSpace(testCase.ProblemId)) throw new InvalidOperationException("A Problem ID is required for a Test Case.");

        if (string.IsNullOrWhiteSpace(testCase.Signature)) throw new InvalidOperationException("A signature is required for a Test Case.");

        return new TestCaseResponse(
            testCase.Id,
            testCase.ProblemId,
            testCase.Name,
            testCase.Inputs.ToResponse().ToList(),
            testCase.ExpectedOutput,
            testCase.OutputDataType,
            testCase.OutputIsArray,
            testCase.Signature,
            testCase.IsActive,
            testCase.IsPublic);
    }

    public static IEnumerable<TestCaseResponse> ToResponses(this IEnumerable<TestCase> testCases)
    {
        return testCases.Select(tc => tc.ToResponse());
    }
}
