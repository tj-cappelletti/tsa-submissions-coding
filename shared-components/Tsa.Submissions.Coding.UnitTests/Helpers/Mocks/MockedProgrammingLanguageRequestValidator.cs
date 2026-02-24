using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Moq;
using Tsa.Submissions.Coding.Contracts.Languages;

namespace Tsa.Submissions.Coding.UnitTests.Helpers.Mocks;

/// <summary>
/// Fluent builder for creating mocked validators for <see cref="ProgrammingLanguageRequest"/> instances.
/// Provides convenient methods to configure validation success and failure scenarios for testing.
/// </summary>
[ExcludeFromCodeCoverage]
internal class MockedProgrammingLanguageRequestValidator : MockedValidatorBuilder<ProgrammingLanguageRequest>
{
    /// <summary>
    /// Configures the validator to return a failed validation result with a single validation error.
    /// </summary>
    /// <param name="expectedModel">The <see cref="ProgrammingLanguageRequest"/> instance that the validator should match.</param>
    /// <param name="property">The name of the property that failed validation.</param>
    /// <param name="message">The validation error message.</param>
    /// <param name="times">The expected number of validator invocations. Defaults to Times.Once if null.</param>
    /// <returns>The builder instance for method chaining.</returns>
    public override MockedValidatorBuilder<ProgrammingLanguageRequest> WithFailedValidationResult(
        ProgrammingLanguageRequest expectedModel,
        string property,
        string message,
        Times? times = null)
    {
        WithFailedValidationResult(
            expectedModel,
            EqualityComparer<ProgrammingLanguageRequest>.Default,
            property,
            message,
            times);

        return this;
    }

    /// <summary>
    /// Configures the validator to return a failed validation result with multiple validation errors.
    /// </summary>
    /// <param name="expectedModel">The <see cref="ProgrammingLanguageRequest"/> instance that the validator should match.</param>
    /// <param name="failures">A dictionary mapping property names to their corresponding validation error messages.</param>
    /// <param name="times">The expected number of validator invocations. Defaults to Times.Once if null.</param>
    /// <returns>The builder instance for method chaining.</returns>
    public override MockedValidatorBuilder<ProgrammingLanguageRequest> WithFailedValidationResult(
        ProgrammingLanguageRequest expectedModel,
        Dictionary<string, string> failures,
        Times? times = null)
    {
        WithFailedValidationResult(
            expectedModel,
            EqualityComparer<ProgrammingLanguageRequest>.Default,
            failures,
            times);

        return this;
    }

    /// <summary>
    /// Configures the validator to return a successful validation result with no errors.
    /// </summary>
    /// <param name="expectedModel">The <see cref="ProgrammingLanguageRequest"/> instance that the validator should match.</param>
    /// <param name="times">The expected number of validator invocations. Defaults to Times.Once if null.</param>
    /// <returns>The builder instance for method chaining.</returns>
    public override MockedValidatorBuilder<ProgrammingLanguageRequest> WithSuccessfulValidationResult(
        ProgrammingLanguageRequest expectedModel,
        Times? times = null)
    {
        WithSuccessfulValidationResult(
            expectedModel,
            EqualityComparer<ProgrammingLanguageRequest>.Default,
            times);

        return this;
    }
}
