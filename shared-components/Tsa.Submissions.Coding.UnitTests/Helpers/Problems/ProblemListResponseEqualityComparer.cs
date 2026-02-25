using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Tsa.Submissions.Coding.Contracts.Problems;

namespace Tsa.Submissions.Coding.UnitTests.Helpers.Problems;

[ExcludeFromCodeCoverage]
internal class ProblemListResponseEqualityComparer : EqualityComparerBase<ProblemListResponse>
{
    protected override bool EqualsCore(ProblemListResponse x, ProblemListResponse y)
    {
        var idsMatch = x.Id == y.Id;
        var isActiveMatch = x.IsActive == y.IsActive;
        var titlesMatch = x.Title == y.Title;

        return idsMatch &&
               isActiveMatch &&
               titlesMatch;
    }

    protected override bool EqualsCore(IList<ProblemListResponse> x, IList<ProblemListResponse> y)
    {
        foreach (var leftProblemResponse in x)
        {
            var rightProblemResponse = y.SingleOrDefault(problemResponse => problemResponse.Id == leftProblemResponse.Id);

            if (!Equals(leftProblemResponse, rightProblemResponse)) return false;
        }

        return true;
    }

    public override int GetHashCode(ProblemListResponse? obj)
    {
        return obj == null ? 0 : HashCode.Combine(obj.Id, obj.IsActive, obj.Title);
    }

    /// <summary>
    ///     Provides the ordering key for list hash code computation.
    ///     Orders by Id to ensure consistent hash codes.
    /// </summary>
    protected override object GetOrderByKey(ProblemListResponse item)
    {
        return item.Id;
    }
}
