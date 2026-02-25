using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Moq;
using Tsa.Submissions.Coding.Contracts.Languages;
using Tsa.Submissions.Coding.UnitTests.Helpers.Languages;

namespace Tsa.Submissions.Coding.UnitTests.Helpers.Mocks;

/// <summary>
///     Fluent builder for creating mocked validators for <see cref="ProgrammingLanguageVersionRequest" /> instances.
///     Provides convenient methods to configure validation success and failure scenarios for testing.
/// </summary>
[ExcludeFromCodeCoverage]
internal class MockedProgrammingLanguageVersionRequestValidator : MockedValidatorBuilder<ProgrammingLanguageVersionRequest>
{
    /// <summary>
    ///     Configures the validator to return a failed validation result with a single validation error.
    /// </summary>
    public override MockedValidatorBuilder<ProgrammingLanguageVersionRequest> WithFailedValidationResult(
        ProgrammingLanguageVersionRequest expectedModel,
        string property,
        string message,
        Times? times = null)
    {
        WithFailedValidationResult(
            expectedModel,
            new ProgrammingLanguageVersionRequestEqualityComparer(),
            property,
            message,
            times);

        return this;
    }

    /// <summary>
    ///     Configures the validator to return a failed validation result with multiple validation errors.
    /// </summary>
    public override MockedValidatorBuilder<ProgrammingLanguageVersionRequest> WithFailedValidationResult(
        ProgrammingLanguageVersionRequest expectedModel,
        Dictionary<string, string> failures,
        Times? times = null)
    {
        WithFailedValidationResult(
            expectedModel,
            new ProgrammingLanguageVersionRequestEqualityComparer(),
            failures,
            times);

        return this;
    }

    /// <summary>
    ///     Configures the validator to return a successful validation result with no errors.
    /// </summary>
    public override MockedValidatorBuilder<ProgrammingLanguageVersionRequest> WithSuccessfulValidationResult(
        ProgrammingLanguageVersionRequest expectedModel,
        Times? times = null)
    {
        WithSuccessfulValidationResult(
            expectedModel,
            new ProgrammingLanguageVersionRequestEqualityComparer(),
            times);

        return this;
    }
}
