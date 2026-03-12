using System;
using System.Diagnostics.CodeAnalysis;
using Tsa.Submissions.Coding.Contracts.TestCases;

namespace Tsa.Submissions.Coding.UnitTests.Helpers.TestCases;

[ExcludeFromCodeCoverage]
internal class TestCaseResponseEqualityComparer : EqualityComparerBase<TestCaseResponse>
{
    protected override bool EqualsCore(TestCaseResponse x, TestCaseResponse y)
    {
        var expectedOutputsMatch = x.ExpectedOutput == y.ExpectedOutput;
        var idsMatch = x.Id == y.Id;
        var inputsMatch = new TestCaseInputResponseEqualityComparer().Equals(x.Inputs, y.Inputs);
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

    public override int GetHashCode(TestCaseResponse? obj)
    {
        if (obj == null) return 0;

        var hashCode = new HashCode();
        hashCode.Add(obj.ExpectedOutput);
        hashCode.Add(obj.Id);
        hashCode.Add(obj.Inputs);
        hashCode.Add(obj.IsActive);
        hashCode.Add(obj.IsPublic);
        hashCode.Add(obj.Name);
        hashCode.Add(obj.OutputDataType);
        hashCode.Add(obj.OutputIsArray);
        hashCode.Add(obj.ProblemId);
        hashCode.Add(obj.Signature);

        return hashCode.ToHashCode();
    }

    protected override Func<TestCaseResponse, bool> GetItemPredicate(TestCaseResponse item)
    {
        return testCase => testCase.Signature == item.Signature;
    }

    protected override object GetOrderByKey(TestCaseResponse item)
    {
        return item.Id;
    }
}
