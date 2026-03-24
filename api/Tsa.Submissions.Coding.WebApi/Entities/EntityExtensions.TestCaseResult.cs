using System;
using Tsa.Submissions.Coding.Contracts.TestCases;

namespace Tsa.Submissions.Coding.WebApi.Entities;

public partial class EntityExtensions
{
    public static TestCaseResultResponse ToResponse(this TestCaseResult testCaseResult)
    {
        if (string.IsNullOrWhiteSpace(testCaseResult.TestCaseId))
        {
            throw new InvalidOperationException("The Test Case ID is required for a Test Case Result");
        }

        return new TestCaseResultResponse(
            testCaseResult.TestCaseId,
            testCaseResult.ActualOutput,
            testCaseResult.Message,
            testCaseResult.Passed,
            testCaseResult.TimedOut,
            testCaseResult.ExecutionTime);
    }
}
