using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Tsa.Submissions.Coding.WebApi.Models;

namespace Tsa.Submissions.Coding.UnitTests.Helpers;

//TODO: Turn into code generator
[ExcludeFromCodeCoverage]
internal class TestSetModelEqualityComparer : IEqualityComparer<TestSetModel?>, IEqualityComparer<IList<TestSetModel>?>
{
    private readonly TestSetValueModelEqualityComparer _testSetValueModelEqualityComparer = new();

    public bool Equals(TestSetModel? x, TestSetModel? y)
    {
        if (ReferenceEquals(x, y)) return true;
        if (x is null) return false;
        if (y is null) return false;
        if (x.GetType() != y.GetType()) return false;

        return
            x.Id == y.Id &&
            _testSetValueModelEqualityComparer.Equals(x.Inputs, y.Inputs) &&
            x.IsPublic == y.IsPublic &&
            x.Name == y.Name &&
            x.ProblemId == y.ProblemId;
    }

    public bool Equals(IList<TestSetModel>? x, IList<TestSetModel>? y)
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

    public int GetHashCode(TestSetModel obj)
    {
        return HashCode.Combine(obj.Id, obj.Inputs, obj.IsPublic, obj.Name, obj.ProblemId);
    }

    public int GetHashCode(IList<TestSetModel>? obj)
    {
        return obj == null ? 0 : obj.GetHashCode();
    }
}
