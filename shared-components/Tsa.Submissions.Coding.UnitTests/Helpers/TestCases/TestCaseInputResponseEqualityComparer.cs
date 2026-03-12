using System;
using System.Diagnostics.CodeAnalysis;
using Tsa.Submissions.Coding.Contracts.TestCases;

namespace Tsa.Submissions.Coding.UnitTests.Helpers.TestCases;

[ExcludeFromCodeCoverage]
internal class TestCaseInputResponseEqualityComparer : EqualityComparerBase<TestCaseInputResponse>
{
    protected override bool EqualsCore(TestCaseInputResponse x, TestCaseInputResponse y)
    {
        var dataTypesMatch = x.DataType == y.DataType;
        var indicesMatch = x.Index == y.Index;
        var isArraysMatch = x.IsArray == y.IsArray;
        var valuesMatch = x.Value == y.Value;

        return dataTypesMatch && indicesMatch && isArraysMatch && valuesMatch;
    }

    public override int GetHashCode(TestCaseInputResponse? obj)
    {
        return obj == null ? 0 : HashCode.Combine(obj.Index, obj.DataType, obj.IsArray, obj.Value);
    }

    protected override Func<TestCaseInputResponse, bool> GetItemPredicate(TestCaseInputResponse item)
    {
        return testCaseInput => testCaseInput.Index == item.Index;
    }

    protected override object GetOrderByKey(TestCaseInputResponse item)
    {
        return item.Index;
    }
}
