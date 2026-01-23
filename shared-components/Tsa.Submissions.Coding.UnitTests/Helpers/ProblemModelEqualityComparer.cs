using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Tsa.Submissions.Coding.Contracts.Problems;

namespace Tsa.Submissions.Coding.UnitTests.Helpers;

//TODO: Turn into code generator
[ExcludeFromCodeCoverage]
internal class ProblemModelEqualityComparer : IEqualityComparer<ProblemResponse?>, IEqualityComparer<IList<ProblemResponse>?>
{
    private readonly TestCaseEqualityComparer _testSetModelEqualityComparer = new();

    public bool Equals(ProblemResponse? x, ProblemResponse? y)
    {
        if (ReferenceEquals(x, y)) return true;
        if (x is null) return false;
        if (y is null) return false;
        if (x.GetType() != y.GetType()) return false;

        return
            x.Description == y.Description &&
            x.Id == y.Id &&
            x.IsActive == y.IsActive &&
            _testSetModelEqualityComparer.Equals(x.TestCases, y.TestCases) &&
            x.Title == y.Title;
    }

    public bool Equals(IList<ProblemResponse>? x, IList<ProblemResponse>? y)
    {
        if (ReferenceEquals(x, y)) return true;
        if (x is null) return false;
        if (y is null) return false;
        if (x.Count != y.Count) return false;

        foreach (var leftProblemResponse in x)
        {
            var rightProblemResponse = y.SingleOrDefault(_ => _.Id == leftProblemResponse.Id);

            if (!Equals(leftProblemResponse, rightProblemResponse)) return false;
        }

        return true;
    }

    public int GetHashCode(ProblemResponse obj)
    {
        return HashCode.Combine(obj.Description, obj.Id, obj.IsActive, obj.TestCases, obj.Title);
    }

    public int GetHashCode(IList<ProblemResponse>? obj)
    {
        return obj == null ? 0 : obj.GetHashCode();
    }
}
