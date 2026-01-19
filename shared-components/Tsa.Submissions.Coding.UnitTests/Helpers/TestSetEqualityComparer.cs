using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Tsa.Submissions.Coding.WebApi.Entities;

namespace Tsa.Submissions.Coding.UnitTests.Helpers;

//TODO: Turn into code generator
[ExcludeFromCodeCoverage]
internal class TestSetEqualityComparer : IEqualityComparer<TestSet?>, IEqualityComparer<IList<TestSet>?>
{
    private readonly TestSetValueEqualityComparer _testSetValueEqualityComparer = new();

    public bool Equals(TestSet? x, TestSet? y)
    {
        if (ReferenceEquals(x, y)) return true;
        if (x is null) return false;
        if (y is null) return false;
        if (x.GetType() != y.GetType()) return false;

        var idsMatch = x.Id == y.Id;
        var inputsMatch = _testSetValueEqualityComparer.Equals(x.Inputs, y.Inputs);
        var isPublicMatch = x.IsPublic == y.IsPublic;
        var nameMatch = x.Name == y.Name;
        var problemMatch = x.Problem?.Id.AsString == y.Problem?.Id.AsString;

        return idsMatch && inputsMatch && isPublicMatch && nameMatch && problemMatch;
    }

    public bool Equals(IList<TestSet>? x, IList<TestSet>? y)
    {
        if (ReferenceEquals(x, y)) return true;
        if (x is null) return false;
        if (y is null) return false;
        if (x.Count != y.Count) return false;

        foreach (var leftTestInputModel in x)
        {
            var rightTestSetInputModel = y.SingleOrDefault(testSetModel => testSetModel.Id == leftTestInputModel.Id);

            if (!Equals(leftTestInputModel, rightTestSetInputModel)) return false;
        }

        return true;
    }

    public int GetHashCode(TestSet obj)
    {
        return HashCode.Combine(obj.Id, obj.Inputs, obj.IsPublic, obj.Name, obj.Problem);
    }

    public int GetHashCode(IList<TestSet>? obj)
    {
        return obj == null ? 0 : obj.GetHashCode();
    }
}
