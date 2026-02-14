using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Tsa.Submissions.Coding.Contracts.Problems;
using Tsa.Submissions.Coding.UnitTests.Helpers.TestCases;

namespace Tsa.Submissions.Coding.UnitTests.Helpers;

//TODO: Turn into code generator
[ExcludeFromCodeCoverage]
internal class ProblemModelEqualityComparer : EqualityComparerBase<ProblemResponse>
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

    protected override bool EqualsCore(IList<ProblemResponse> x, IList<ProblemResponse> y)
    {
        foreach (var leftProblemResponse in x)
        {
            var rightProblemResponse = y.SingleOrDefault(problemResponse => problemResponse.Id == leftProblemResponse.Id);

            if (!Equals(leftProblemResponse, rightProblemResponse)) return false;
        }

        return true;
    }

    public override int GetHashCode(ProblemResponse? obj)
    {
        return obj == null ? 0 : HashCode.Combine(obj.Description, obj.Id, obj.IsActive, obj.TestCases, obj.Title);
    }

    public override int GetHashCode(IList<ProblemResponse>? obj)
    {
        if (obj == null) return 0;

        var hash = new HashCode();

        foreach (var item in obj.OrderBy(i => i.Id))
        {
            hash.Add(GetHashCode(item));
        }

        return hash.ToHashCode();
    }
}
