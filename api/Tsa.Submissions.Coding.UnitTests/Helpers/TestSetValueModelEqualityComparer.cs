using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Tsa.Submissions.Coding.WebApi.Models;

namespace Tsa.Submissions.Coding.UnitTests.Helpers;

//TODO: Turn into code generator
[ExcludeFromCodeCoverage]
internal class TestSetValueModelEqualityComparer : IEqualityComparer<TestSetValueModel?>, IEqualityComparer<IList<TestSetValueModel>?>
{
    public bool Equals(TestSetValueModel? x, TestSetValueModel? y)
    {
        if (ReferenceEquals(x, y)) return true;
        if (x is null) return false;
        if (y is null) return false;
        if (x.GetType() != y.GetType()) return false;

        return
            x.DataType == y.DataType &&
            x.Index == y.Index &&
            x.IsArray == y.IsArray &&
            x.ValueAsJson == y.ValueAsJson;
    }

    public bool Equals(IList<TestSetValueModel>? x, IList<TestSetValueModel>? y)
    {
        if (ReferenceEquals(x, y)) return true;
        if (x is null) return false;
        if (y is null) return false;
        if (x.Count != y.Count) return false;

        foreach (var leftTestInputModel in x)
        {
            var rightTestSetInputModel = y.SingleOrDefault(testSetValueModel => testSetValueModel.Index == leftTestInputModel.Index);

            if (!Equals(leftTestInputModel, rightTestSetInputModel)) return false;
        }

        return true;
    }

    public int GetHashCode(TestSetValueModel obj)
    {
        return HashCode.Combine(obj.DataType, obj.Index, obj.IsArray, obj.ValueAsJson);
    }

    public int GetHashCode(IList<TestSetValueModel>? obj)
    {
        return obj == null ? 0 : obj.GetHashCode();
    }
}
