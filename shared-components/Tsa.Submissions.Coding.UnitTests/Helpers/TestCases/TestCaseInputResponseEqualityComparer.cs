using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
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

    protected override bool EqualsCore(IList<TestCaseInputResponse> x, IList<TestCaseInputResponse> y)
    {
        foreach (var leftTestCaseInput in x)
        {
            // Find the corresponding TestCaseInput in the right list by Index
            // Index is assumed to be unique within each list
            var rightTestCaseInput = y.SingleOrDefault(testCaseInput => testCaseInput.Index == leftTestCaseInput.Index);

            if (!Equals(leftTestCaseInput, rightTestCaseInput)) return false;
        }

        return true;
    }

    public override int GetHashCode(TestCaseInputResponse? obj)
    {
        return obj == null ? 0 : HashCode.Combine(obj.Index, obj.DataType, obj.IsArray, obj.Value);
    }

    public override int GetHashCode(IList<TestCaseInputResponse>? obj)
    {
        if (obj == null) return 0;

        var hash = new HashCode();

        foreach (var item in obj.OrderBy(i => i.Index))
        {
            hash.Add(GetHashCode(item));
        }

        return hash.ToHashCode();
    }
}
