using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Tsa.Submissions.Coding.WebApi.Models;

namespace Tsa.Submissions.Coding.UnitTests.Helpers;

[ExcludeFromCodeCoverage]
internal class TestSetResultModelEqualityComparer : IEqualityComparer<TestSetResultModel?>, IEqualityComparer<IList<TestSetResultModel>?>
{
    public bool Equals(TestSetResultModel? x, TestSetResultModel? y)
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

    public bool Equals(IList<TestSetResultModel>? x, IList<TestSetResultModel>? y)
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

    public int GetHashCode(TestSetResultModel obj)
    {
        return HashCode.Combine(obj.Passed, obj.RunDuration, obj.TestSetId);
    }

    public int GetHashCode(IList<TestSetResultModel>? obj)
    {
        return obj == null ? 0 : obj.GetHashCode();
    }
}
