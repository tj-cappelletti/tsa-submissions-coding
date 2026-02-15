using System;
using System.Collections.Generic;
using System.Linq;
using Tsa.Submissions.Coding.Contracts.Problems;

namespace Tsa.Submissions.Coding.WebApi.Entities;

public static partial class EntityExtensions
{
    /// <summary>
    ///     Converts a <see cref="Problem" /> entity to a lightweight <see cref="ProblemListResponse" />.
    /// </summary>
    /// <param name="problem">The problem entity to convert</param>
    /// <returns>A <see cref="ProblemListResponse" /> containing basic problem information</returns>
    /// <exception cref="InvalidOperationException">
    ///     Thrown when required fields (ID or title) are missing
    /// </exception>
    /// <remarks>
    ///     This method produces a lightweight response intended for list operations.
    ///     It excludes the description and test cases to minimize payload size.
    /// </remarks>
    private static ProblemListResponse ToProblemListResponse(this Problem problem)
    {
        if (string.IsNullOrWhiteSpace(problem.Id)) throw new InvalidOperationException("Problem ID is required.");

        if (string.IsNullOrWhiteSpace(problem.Title)) throw new InvalidOperationException("Problem title is required.");

        return new ProblemListResponse(problem.Id, problem.Title, problem.IsActive);
    }

    /// <summary>
    ///     Converts a <see cref="Problem" /> entity to a complete <see cref="ProblemResponse" />.
    /// </summary>
    /// <param name="problem">The problem entity to convert</param>
    /// <param name="testCases">Optional collection of test cases to include in the response</param>
    /// <returns>A <see cref="ProblemResponse" /> containing complete problem data including description</returns>
    /// <exception cref="InvalidOperationException">
    ///     Thrown when required fields (ID, title, or description) are missing
    /// </exception>
    /// <remarks>
    ///     This method produces a complete response intended for single problem retrieval.
    ///     It includes the full description and optionally includes test cases if provided.
    ///     Use this for detailed problem views where complete information is needed.
    /// </remarks>
    public static ProblemResponse ToResponse(this Problem problem, IEnumerable<TestCase>? testCases = null)
    {
        if (string.IsNullOrWhiteSpace(problem.Id)) throw new InvalidOperationException("Problem ID is required.");

        if (string.IsNullOrWhiteSpace(problem.Description)) throw new InvalidOperationException("Problem description is required.");

        if (string.IsNullOrWhiteSpace(problem.Title)) throw new InvalidOperationException("Problem title is required.");

        return testCases == null
            ? new ProblemResponse(problem.Id, problem.Title, problem.Description, problem.IsActive)
            : new ProblemResponse(problem.Id, problem.Title, problem.Description, problem.IsActive, testCases.ToResponses());
    }

    /// <summary>
    ///     Converts a collection of <see cref="Problem" /> entities to lightweight <see cref="ProblemListResponse" /> objects.
    /// </summary>
    /// <param name="problems">The collection of problem entities to convert</param>
    /// <returns>A collection of <see cref="ProblemListResponse" /> objects</returns>
    /// <exception cref="InvalidOperationException">
    ///     Thrown when required fields (ID or title) are missing from any problem
    /// </exception>
    /// <remarks>
    ///     This method produces lightweight responses optimized for list operations.
    ///     Each response excludes the problem description and test cases to minimize payload size
    ///     and improve API performance when returning multiple problems.
    ///     Use <see cref="ToResponse(Problem, IEnumerable{TestCase}?)" /> for individual problem details.
    /// </remarks>
    public static IEnumerable<ProblemListResponse> ToResponses(this IEnumerable<Problem> problems)
    {
        return problems.Select(problem => problem.ToProblemListResponse());
    }
}
