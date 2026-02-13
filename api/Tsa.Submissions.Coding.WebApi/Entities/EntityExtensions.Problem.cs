using System;
using System.Collections.Generic;
using System.Linq;
using Tsa.Submissions.Coding.Contracts.Problems;

namespace Tsa.Submissions.Coding.WebApi.Entities;

/// <summary>
///     Provides extension methods for converting <see cref="Problem" /> entities to response models.
/// </summary>
public static partial class EntityExtensions
{
    /// <summary>
    ///     Converts a <see cref="Problem" /> entity to a <see cref="ProblemResponse" />.
    /// </summary>
    /// <param name="problem">The problem entity to convert</param>
    /// <param name="testCases">Optional collection of test cases to include in the response</param>
    /// <param name="includeDescription">
    ///     If true, includes the problem description in the response; otherwise, the description is set to null.
    ///     Defaults to true.
    /// </param>
    /// <returns>A <see cref="ProblemResponse" /> containing the problem data</returns>
    /// <exception cref="InvalidOperationException">
    ///     Thrown when required fields (ID, title, or description when includeDescription is true) are missing
    /// </exception>
    /// <remarks>
    ///     When includeDescription is false, the description is excluded to reduce response size for list operations.
    ///     Test cases are only included in the response if explicitly provided.
    /// </remarks>
    public static ProblemResponse ToResponse(this Problem problem, IEnumerable<TestCase>? testCases = null, bool includeDescription = true)
    {
        if (includeDescription && string.IsNullOrWhiteSpace(problem.Description)) throw new InvalidOperationException("Problem description is required.");

        var description = includeDescription
            ? problem.Description
            : null;

        if (string.IsNullOrWhiteSpace(problem.Id)) throw new InvalidOperationException("Problem ID is required.");

        if (string.IsNullOrWhiteSpace(problem.Title)) throw new InvalidOperationException("Problem title is required.");

        return testCases == null
            ? new ProblemResponse(problem.Id, problem.Title, description, problem.IsActive)
            : new ProblemResponse(problem.Id, problem.Title, description, problem.IsActive, testCases.ToResponses());
    }

    /// <summary>
    ///     Converts a collection of <see cref="Problem" /> entities to <see cref="ProblemResponse" /> objects.
    /// </summary>
    /// <param name="problems">The collection of problem entities to convert</param>
    /// <param name="includeDescription">
    ///     If true, includes the problem descriptions in the responses; otherwise, descriptions are excluded.
    ///     Defaults to false to keep list responses lightweight.
    /// </param>
    /// <returns>A collection of <see cref="ProblemResponse" /> objects</returns>
    /// <remarks>
    ///     By default, this method excludes problem descriptions to minimize response payload size
    ///     for list operations. This is particularly important for API performance when returning
    ///     multiple problems. To include descriptions, explicitly set includeDescription to true.
    /// </remarks>
    public static IEnumerable<ProblemResponse> ToResponses(this IEnumerable<Problem> problems, bool includeDescription = false)
    {
        return problems.Select(p => p.ToResponse(includeDescription: includeDescription));
    }
}
