using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Tsa.Submissions.Coding.Contracts.Problems;

namespace Tsa.Submissions.Coding.UnitTests.Helpers.Problems;

[ExcludeFromCodeCoverage]
internal class ProblemRequestEqualityComparer : EqualityComparerBase<ProblemRequest>
{
    protected override bool EqualsCore(ProblemRequest x, ProblemRequest y)
    {
        var descriptionsMatch = x.Description == y.Description;
        var isActiveMatch = x.IsActive == y.IsActive;
        var titlesMatch = x.Title == y.Title;

        return descriptionsMatch && isActiveMatch && titlesMatch;
    }

    protected override bool EqualsCore(IList<ProblemRequest> x, IList<ProblemRequest> y)
    {
        foreach (var leftProblemRequest in x)
        {
            // The title is the unique identifier for a problem request, so we can use it to find the corresponding problem request in the right list.
            var rightProblemRequest = y.SingleOrDefault(problemRequest => problemRequest.Title == leftProblemRequest.Title);

            if (!Equals(leftProblemRequest, rightProblemRequest)) return false;
        }

        return true;
    }

    public override int GetHashCode(ProblemRequest? obj)
    {
        return obj == null ? 0 : HashCode.Combine(obj.Description, obj.IsActive, obj.Title);
    }

    public override int GetHashCode(IList<ProblemRequest>? obj)
    {
        if (obj == null) return 0;

        var hash = new HashCode();

        foreach (var item in obj.OrderBy(i => i.Title))
        {
            hash.Add(GetHashCode(item));
        }

        return hash.ToHashCode();
    }
}
