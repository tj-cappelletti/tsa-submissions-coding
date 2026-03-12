using System;
using System.Diagnostics.CodeAnalysis;
using Tsa.Submissions.Coding.WebApi.Entities;

namespace Tsa.Submissions.Coding.UnitTests.Helpers.TestCases;

[ExcludeFromCodeCoverage]
internal class TestCaseEqualityComparer : EqualityComparerBase<TestCase>
{
    protected override bool EqualsCore(TestCase x, TestCase y)
    {
        var expectedOutputsMatch = x.ExpectedOutput == y.ExpectedOutput;
        var idsMatch = x.Id == y.Id;
        var inputsMatch = new TestCaseInputEqualityComparer().Equals(x.Inputs, y.Inputs);
        var isActiveMatch = x.IsActive == y.IsActive;
        var isPublicMatch = x.IsPublic == y.IsPublic;
        var namesMatch = x.Name == y.Name;
        var outputDataTypesMatch = x.OutputDataType == y.OutputDataType;
        var outputIsArraysMatch = x.OutputIsArray == y.OutputIsArray;
        var problemIdsMatch = x.ProblemId == y.ProblemId;
        var signaturesMatch = x.Signature == y.Signature;

        return expectedOutputsMatch &&
               idsMatch &&
               inputsMatch &&
               isActiveMatch &&
               isPublicMatch &&
               namesMatch &&
               outputDataTypesMatch &&
               outputIsArraysMatch &&
               problemIdsMatch &&
               signaturesMatch;
    }

    public override int GetHashCode(TestCase? obj)
    {
        if (obj == null) return 0;

        var hash = new HashCode();
        hash.Add(obj.ExpectedOutput);
        hash.Add(obj.Id);
        hash.Add(new TestCaseInputEqualityComparer().GetHashCode(obj.Inputs));
        hash.Add(obj.IsActive);
        hash.Add(obj.IsPublic);
        hash.Add(obj.Name);
        hash.Add(obj.OutputDataType);
        hash.Add(obj.OutputIsArray);
        hash.Add(obj.ProblemId);
        hash.Add(obj.Signature);

        return hash.ToHashCode();
    }

    protected override Func<TestCase, bool> GetItemPredicate(TestCase item)
    {
        return testCase => testCase.Signature == item.Signature;
    }

    protected override object GetOrderByKey(TestCase item)
    {
        return item.Signature!;
    }
}
