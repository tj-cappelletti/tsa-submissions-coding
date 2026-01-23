using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Tsa.Submissions.Coding.Contracts.TestCases;

namespace Tsa.Submissions.Coding.UnitTests.Helpers;

[ExcludeFromCodeCoverage]
internal class TestCaseEqualityComparer : IEqualityComparer<TestCase?>, IEqualityComparer<IList<TestCase>?>
{
    public bool Equals(TestCase? x, TestCase? y)
    {
        if (ReferenceEquals(x, y)) return true;
        if (x is null) return false;
        if (y is null) return false;
        if (x.GetType() != y.GetType()) return false;

        return x.Input == y.Input && x.ExpectedOutput == y.ExpectedOutput && x.IsActive == y.IsActive;
    }

    public bool Equals(IList<TestCase>? x, IList<TestCase>? y)
    {
        if (ReferenceEquals(x, y)) return true;
        if (x is null) return false;
        if (y is null) return false;
        if (x.Count != y.Count) return false;

        foreach (var leftTestCase in x)
        {
            var rightTestCase = y.SingleOrDefault(testCase => testCase.Id == leftTestCase.Id);

            if (!Equals(leftTestCase, rightTestCase)) return false;
        }

        return true;
    }

    public int GetHashCode(TestCase? obj)
    {
        return HashCode.Combine(obj?.Input, obj?.ExpectedOutput, obj?.IsActive);
    }

    public int GetHashCode(IList<TestCase>? obj)
    {
        return obj == null ? 0 : obj.GetHashCode();
    }
}
