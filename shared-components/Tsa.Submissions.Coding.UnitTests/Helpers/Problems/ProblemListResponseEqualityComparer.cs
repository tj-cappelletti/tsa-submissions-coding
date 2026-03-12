using System;
using System.Diagnostics.CodeAnalysis;
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

    public override int GetHashCode(ProblemListResponse? obj)
    {
        return obj == null ? 0 : HashCode.Combine(obj.Id, obj.IsActive, obj.Title);
    }

    protected override Func<ProblemListResponse, bool> GetItemPredicate(ProblemListResponse item)
    {
        return problemResponse => problemResponse.Id == item.Id;
    }

    protected override object GetOrderByKey(ProblemListResponse item)
    {
        return item.Id;
    }
}
