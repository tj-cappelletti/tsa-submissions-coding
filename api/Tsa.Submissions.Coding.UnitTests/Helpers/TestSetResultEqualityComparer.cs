using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Tsa.Submissions.Coding.WebApi.Entities;

namespace Tsa.Submissions.Coding.UnitTests.Helpers;

//TODO: Turn into code generator
[ExcludeFromCodeCoverage]
internal class TestSetResultEqualityComparer : IEqualityComparer<TestSetResult?>, IEqualityComparer<IList<TestSetResult>?>
{
    public bool Equals(TestSetResult? x, TestSetResult? y)
    {
        if (ReferenceEquals(x, y)) return true;
        if (x is null) return false;
        if (y is null) return false;
        if (x.GetType() != y.GetType()) return false;

        return
            x.Passed == y.Passed &&
            x.RunDuration == y.RunDuration &&
            x.TestSet?.Id.AsString == y.TestSet?.Id.AsString;
    }

    public bool Equals(IList<TestSetResult>? x, IList<TestSetResult>? y)
    {
        if (ReferenceEquals(x, y)) return true;
        if (x is null) return false;
        if (y is null) return false;
        if (x.Count != y.Count) return false;

        foreach (var leftTestInputModel in x)
        {
            var rightTestSetInputModel = y.SingleOrDefault(_ => _.TestSet?.Id.AsString == leftTestInputModel.TestSet?.Id.AsString);

            if (!Equals(leftTestInputModel, rightTestSetInputModel)) return false;
        }

        return true;
    }

    public int GetHashCode(TestSetResult obj)
    {
        return HashCode.Combine(obj.Passed, obj.RunDuration, obj.TestSet);
    }

    public int GetHashCode(IList<TestSetResult>? obj)
    {
        return obj == null ? 0 : obj.GetHashCode();
    }
}
