using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
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

    protected override bool EqualsCore(IList<TestCaseResponse> x, IList<TestCaseResponse> y)
    {
        foreach (var leftTestCaseResponse in x)
        {
            var rightTestCaseResponse = y.SingleOrDefault(testCase => testCase.Signature == leftTestCaseResponse.Signature);

            if (!Equals(leftTestCaseResponse, rightTestCaseResponse)) return false;
        }

        return true;
    }

    public override int GetHashCode(TestCaseResponse? obj)
    {
        throw new NotImplementedException();
    }

    protected override object GetOrderByKey(TestCaseResponse item)
    {
        return item.Id;
    }
}
