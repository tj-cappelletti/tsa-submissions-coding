using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Tsa.Submissions.Coding.Contracts.Submissions;

namespace Tsa.Submissions.Coding.UnitTests.Helpers;

[ExcludeFromCodeCoverage]
internal class TestSetResultModelEqualityComparer : IEqualityComparer<TestSetResultResponse?>, IEqualityComparer<IList<TestSetResultResponse>?>
{
    public bool Equals(TestSetResultResponse? x, TestSetResultResponse? y)
    {
        if (ReferenceEquals(x, y)) return true;
        if (x is null) return false;
        if (y is null) return false;
        if (x.GetType() != y.GetType()) return false;

        return
            x.Passed == y.Passed &&
            x.RunDuration == y.RunDuration &&
            x.TestSetId == y.TestSetId;
    }

    public bool Equals(IList<TestSetResultResponse>? x, IList<TestSetResultResponse>? y)
    {
        if (ReferenceEquals(x, y)) return true;
        if (x is null) return false;
        if (y is null) return false;
        if (x.Count != y.Count) return false;

        foreach (var leftTestInputModel in x)
        {
            var rightTestSetInputModel = y.SingleOrDefault(_ => _.TestSetId == leftTestInputModel.TestSetId);

            if (!Equals(leftTestInputModel, rightTestSetInputModel)) return false;
        }

        return true;
    }

    public int GetHashCode(TestSetResultResponse obj)
    {
        return HashCode.Combine(obj.Passed, obj.RunDuration, obj.TestSetId);
    }

    public int GetHashCode(IList<TestSetResultResponse>? obj)
    {
        return obj == null ? 0 : obj.GetHashCode();
    }
}
