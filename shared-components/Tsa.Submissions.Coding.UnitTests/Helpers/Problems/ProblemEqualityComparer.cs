using System;
using System.Diagnostics.CodeAnalysis;
using Tsa.Submissions.Coding.WebApi.Entities;

namespace Tsa.Submissions.Coding.UnitTests.Helpers.Problems;

[ExcludeFromCodeCoverage]
internal class ProblemEqualityComparer : EqualityComparerBase<Problem>
{
    protected override bool EqualsCore(Problem x, Problem y)
    {
        var descriptionsMatch = x.Description == y.Description;
        var idsMatch = x.Id == y.Id;
        var isActiveMatch = x.IsActive == y.IsActive;
        var titlesMatch = x.Title == y.Title;

        return descriptionsMatch && idsMatch && isActiveMatch && titlesMatch;
    }

    public override int GetHashCode(Problem? obj)
    {
        return obj == null ? 0 : HashCode.Combine(obj.Description, obj.Id, obj.IsActive, obj.Title);
    }

    protected override Func<Problem, bool> GetItemPredicate(Problem item)
    {
        return problem => problem.Id == item.Id;
    }

    protected override object GetOrderByKey(Problem item)
    {
        return item.Id ?? string.Empty;
    }
}
