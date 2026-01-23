using System;
using System.Collections.Generic;
using System.Linq;
using Tsa.Submissions.Coding.Contracts.Problems;

namespace Tsa.Submissions.Coding.WebApi.Entities;

public static partial class EntityExtensions
{
    public static ProblemResponse ToResponse(this Problem problem, bool includeTestCases = false)
    {
        if (string.IsNullOrWhiteSpace(problem.Description)) throw new InvalidOperationException("Problem description is required.");

        if (string.IsNullOrWhiteSpace(problem.Id)) throw new InvalidOperationException("Problem ID is required.");

        if (string.IsNullOrWhiteSpace(problem.Title)) throw new InvalidOperationException("Problem title is required.");

        var testCases = includeTestCases ? problem.TestCases : [];

        return new ProblemResponse(
            problem.Id,
            problem.Title,
            problem.Description,
            problem.IsActive,
            testCases
        );
    }

    public static IEnumerable<ProblemResponse> ToResponses(this IEnumerable<Problem> problems, bool includeTestCases = false)
    {
        return problems.Select(p => p.ToResponse(includeTestCases));
    }
}
