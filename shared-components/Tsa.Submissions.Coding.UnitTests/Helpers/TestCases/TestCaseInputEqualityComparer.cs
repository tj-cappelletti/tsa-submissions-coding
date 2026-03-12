using System;
using System.Diagnostics.CodeAnalysis;
using Tsa.Submissions.Coding.WebApi.Entities;

namespace Tsa.Submissions.Coding.UnitTests.Helpers.TestCases;

[ExcludeFromCodeCoverage]
internal class TestCaseInputEqualityComparer : EqualityComparerBase<TestCaseInput>
{
    protected override bool EqualsCore(TestCaseInput x, TestCaseInput y)
    {
        var dataTypesMatch = x.DataType == y.DataType;
        var indicesMatch = x.Index == y.Index;
        var isArraysMatch = x.IsArray == y.IsArray;
        var valuesMatch = x.Value == y.Value;

        return dataTypesMatch && indicesMatch && isArraysMatch && valuesMatch;
    }

    public override int GetHashCode(TestCaseInput? obj)
    {
        return obj == null ? 0 : HashCode.Combine(obj.Index, obj.DataType, obj.IsArray, obj.Value);
    }

    protected override Func<TestCaseInput, bool> GetItemPredicate(TestCaseInput item)
    {
        return testCaseInput => testCaseInput.Index == item.Index;
    }

    protected override object GetOrderByKey(TestCaseInput item)
    {
        return item.Index;
    }
}
