using System;
using System.Collections.Generic;
using System.Linq;
using Tsa.Submissions.Coding.Contracts.Problems;
using Tsa.Submissions.Coding.Contracts.TestSets;
using Tsa.Submissions.Coding.WebApi.Models;

namespace Tsa.Submissions.Coding.WebApi.Entities;

public static partial class EntityExtensions
{
    public static ProblemResponse ToResponse(this Problem problem, IList<TestSetResponse>? testSetResponses = null)
    {
        if (string.IsNullOrWhiteSpace(problem.Description)) throw new InvalidOperationException("Problem description is required.");

        if (string.IsNullOrWhiteSpace(problem.Id)) throw new InvalidOperationException("Problem ID is required.");

        if (string.IsNullOrWhiteSpace(problem.Title)) throw new InvalidOperationException("Problem title is required.");

        return new ProblemResponse(
            problem.Id,
            problem.Title,
            problem.Description,
            problem.IsActive,
            testSetResponses ?? []);
    }

    public static IEnumerable<ProblemResponse> ToResponses(this IEnumerable<Problem> problems)
    {
        return problems.Select(p => p.ToResponse()).ToList();
    }
}
