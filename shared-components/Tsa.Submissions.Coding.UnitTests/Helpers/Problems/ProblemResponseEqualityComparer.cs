using System;
using System.Diagnostics.CodeAnalysis;
using Tsa.Submissions.Coding.Contracts.Problems;
using Tsa.Submissions.Coding.UnitTests.Helpers.TestCases;

namespace Tsa.Submissions.Coding.UnitTests.Helpers.Problems;

//TODO: Turn into code generator
[ExcludeFromCodeCoverage]
internal class ProblemResponseEqualityComparer : EqualityComparerBase<ProblemResponse>
{
    protected override bool EqualsCore(ProblemResponse x, ProblemResponse y)
    {
        var descriptionsMatch = x.Description == y.Description;
        var idsMatch = x.Id == y.Id;
        var isActiveMatch = x.IsActive == y.IsActive;
        var testCasesMatch = new TestCaseResponseEqualityComparer().Equals(x.TestCases, y.TestCases);
        var titlesMatch = x.Title == y.Title;

        return descriptionsMatch &&
               idsMatch &&
               isActiveMatch &&
               testCasesMatch &&
               titlesMatch;
    }

    public override int GetHashCode(ProblemResponse? obj)
    {
        return obj == null ? 0 : HashCode.Combine(obj.Description, obj.Id, obj.IsActive, obj.TestCases, obj.Title);
    }

    protected override Func<ProblemResponse, bool> GetItemPredicate(ProblemResponse item)
    {
        return problemResponse => problemResponse.Id == item.Id;
    }

    protected override object GetOrderByKey(ProblemResponse item)
    {
        return item.Id;
    }
}
