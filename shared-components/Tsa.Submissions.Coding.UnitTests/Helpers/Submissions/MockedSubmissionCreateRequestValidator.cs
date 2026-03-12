using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Moq;
using Tsa.Submissions.Coding.Contracts.Submissions;
using Tsa.Submissions.Coding.UnitTests.Helpers.Mocks;

namespace Tsa.Submissions.Coding.UnitTests.Helpers.Submissions;

[ExcludeFromCodeCoverage]
internal class MockedSubmissionCreateRequestValidator : MockedValidatorBuilder<SubmissionCreateRequest>
{
    public override MockedValidatorBuilder<SubmissionCreateRequest> WithFailedValidationResult(
        SubmissionCreateRequest expectedModel,
        string property,
        string message,
        Times? times = null)
    {
        WithFailedValidationResult(expectedModel, new SubmissionCreateRequestEqualityComparer(), property, message, times);

        return this;
    }

    public override MockedValidatorBuilder<SubmissionCreateRequest> WithFailedValidationResult(
        SubmissionCreateRequest expectedModel,
        Dictionary<string, string> failures,
        Times? times = null)
    {
        WithFailedValidationResult(expectedModel, new SubmissionCreateRequestEqualityComparer(), failures, times);

        return this;
    }

    public override MockedValidatorBuilder<SubmissionCreateRequest> WithSuccessfulValidationResult(
        SubmissionCreateRequest expectedModel,
        Times? times = null)
    {
        WithSuccessfulValidationResult(expectedModel, new SubmissionCreateRequestEqualityComparer(), times);

        return this;
    }
}
