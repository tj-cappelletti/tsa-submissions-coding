using System;
using System.Collections.Generic;
using System.Linq;
using Tsa.Submissions.Coding.Contracts.Problems;

namespace Tsa.Submissions.Coding.WebApi.Entities;

public static partial class EntityExtensions
{
    public static ProblemResponse ToResponse(this Problem problem, IEnumerable<TestCase>? testCases = null)
    {
        if (string.IsNullOrWhiteSpace(problem.Description)) throw new InvalidOperationException("Problem description is required.");

        if (string.IsNullOrWhiteSpace(problem.Id)) throw new InvalidOperationException("Problem ID is required.");

        if (string.IsNullOrWhiteSpace(problem.Title)) throw new InvalidOperationException("Problem title is required.");

        return testCases == null
            ? new ProblemResponse(problem.Id, problem.Title, problem.Description, problem.IsActive)
            : new ProblemResponse(problem.Id, problem.Title, problem.Description, problem.IsActive, testCases.ToResponses());
    }

    public static IEnumerable<ProblemResponse> ToResponses(this IEnumerable<Problem> problems)
    {
        return problems.Select(p => p.ToResponse());
    }
}
