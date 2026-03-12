using System;
using System.Diagnostics.CodeAnalysis;
using Tsa.Submissions.Coding.Contracts.Problems;

namespace Tsa.Submissions.Coding.UnitTests.Helpers.Problems;

[ExcludeFromCodeCoverage]
internal class ProblemRequestEqualityComparer : EqualityComparerBase<ProblemRequest>
{
    protected override bool EqualsCore(ProblemRequest x, ProblemRequest y)
    {
        return x.Description == y.Description &&
               x.IsActive == y.IsActive &&
               x.Title == y.Title;
    }

    public override int GetHashCode(ProblemRequest? obj)
    {
        if (obj is null) return 0;

        return HashCode.Combine(obj.Description, obj.IsActive, obj.Title);
    }

    protected override Func<ProblemRequest, bool> GetItemPredicate(ProblemRequest item)
    {
        return problemRequest => problemRequest.Title == item.Title;
    }

    protected override object GetOrderByKey(ProblemRequest item)
    {
        return item.Title;
    }
}
