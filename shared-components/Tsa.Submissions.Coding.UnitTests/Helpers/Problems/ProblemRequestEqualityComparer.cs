using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Tsa.Submissions.Coding.Contracts.Problems;

namespace Tsa.Submissions.Coding.UnitTests.Helpers.Problems;

/// <summary>
///     Provides equality comparison for <see cref="ProblemRequest" /> objects and collections.
/// </summary>
[ExcludeFromCodeCoverage]
internal class ProblemRequestEqualityComparer : EqualityComparerBase<ProblemRequest>
{
    /// <summary>
    ///     Performs the actual equality comparison for two <see cref="ProblemRequest" /> objects.
    /// </summary>
    protected override bool EqualsCore(ProblemRequest x, ProblemRequest y)
    {
        return x.Description == y.Description &&
               x.IsActive == y.IsActive &&
               x.Title == y.Title;
    }

    /// <summary>
    ///     Performs the actual equality comparison for two lists of <see cref="ProblemRequest" /> objects.
    /// </summary>
    protected override bool EqualsCore(IList<ProblemRequest> x, IList<ProblemRequest> y)
    {
        foreach (var leftProblemRequest in x)
        {
            // The title is the unique identifier for a problem request
            var rightProblemRequest = y.SingleOrDefault(problemRequest => problemRequest.Title == leftProblemRequest.Title);

            if (!Equals(leftProblemRequest, rightProblemRequest)) return false;
        }

        return true;
    }

    /// <summary>
    ///     Returns a hash code for the specified <see cref="ProblemRequest" />.
    /// </summary>
    public override int GetHashCode(ProblemRequest? obj)
    {
        if (obj is null) return 0;

        return HashCode.Combine(obj.Description, obj.IsActive, obj.Title);
    }

    /// <summary>
    ///     Provides the ordering key for list hash code computation.
    ///     Orders by Title to ensure consistent hash codes.
    /// </summary>
    protected override object GetOrderByKey(ProblemRequest item) => item.Title;
}
