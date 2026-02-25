using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using FluentValidation;
using FluentValidation.Results;
using Moq;

namespace Tsa.Submissions.Coding.UnitTests.Helpers.Mocks;

/// <summary>
/// Abstract base class for creating fluent builders that configure mocked FluentValidation validators.
/// Provides methods to set up validation success and failure scenarios for testing.
/// </summary>
/// <typeparam name="TModel">The type of model being validated.</typeparam>
[ExcludeFromCodeCoverage]
internal abstract class MockedValidatorBuilder<TModel>
{
    private readonly Mock<IValidator<TModel>> _mock = new();

    /// <summary>
    /// Builds and returns the configured mock validator instance.
    /// </summary>
    /// <returns>The configured mock validator.</returns>
    public Mock<IValidator<TModel>> Build()
    {
        return _mock;
    }

    /// <summary>
    /// Builds and returns the configured mock validator's object.
    /// </summary>
    /// <returns>The mocked validator object.</returns>
    public IValidator<TModel> BuildObject()
    {
        return _mock.Object;
    }

    /// <summary>
    /// Configures the validator to return a failed validation result with a single validation error.
    /// This abstract method must be implemented by derived classes to provide model-specific equality comparison.
    /// </summary>
    /// <param name="expectedModel">The model instance that the validator should match.</param>
    /// <param name="property">The name of the property that failed validation.</param>
    /// <param name="message">The validation error message.</param>
    /// <param name="times">The expected number of validator invocations. Defaults to Times.Once if null.</param>
    /// <returns>The builder instance for method chaining.</returns>
    public abstract MockedValidatorBuilder<TModel> WithFailedValidationResult(
        TModel expectedModel,
        string property,
        string message,
        Times? times = null);

    /// <summary>
    /// Configures the validator to return a failed validation result with a single validation error using a custom equality comparer.
    /// </summary>
    /// <param name="expectedModel">The model instance that the validator should match.</param>
    /// <param name="equalityComparer">The equality comparer used to match the model instance.</param>
    /// <param name="property">The name of the property that failed validation.</param>
    /// <param name="message">The validation error message.</param>
    /// <param name="times">The expected number of validator invocations. Defaults to Times.Once if null.</param>
    /// <returns>The builder instance for method chaining.</returns>
    protected MockedValidatorBuilder<TModel> WithFailedValidationResult(
        TModel expectedModel,
        IEqualityComparer<TModel> equalityComparer,
        string property,
        string message,
        Times? times = null)
    {
        var validationFailure = new ValidationFailure(property, message);

        _mock
            .Setup(validator => validator.ValidateAsync(It.Is(expectedModel, equalityComparer), default))
            .ReturnsAsync(new ValidationResult(new[] { validationFailure }))
            .Verifiable(times ?? Times.Once());

        return this;
    }

    /// <summary>
    /// Configures the validator to return a failed validation result with multiple validation errors.
    /// This abstract method must be implemented by derived classes to provide model-specific equality comparison.
    /// </summary>
    /// <param name="expectedModel">The model instance that the validator should match.</param>
    /// <param name="failures">A dictionary mapping property names to their corresponding validation error messages.</param>
    /// <param name="times">The expected number of validator invocations. Defaults to Times.Once if null.</param>
    /// <returns>The builder instance for method chaining.</returns>
    public abstract MockedValidatorBuilder<TModel> WithFailedValidationResult(
        TModel expectedModel,
        Dictionary<string, string> failures,
        Times? times = null);

    /// <summary>
    /// Configures the validator to return a failed validation result with multiple validation errors using a custom equality comparer.
    /// </summary>
    /// <param name="expectedModel">The model instance that the validator should match.</param>
    /// <param name="equalityComparer">The equality comparer used to match the model instance.</param>
    /// <param name="failures">A dictionary mapping property names to their corresponding validation error messages.</param>
    /// <param name="times">The expected number of validator invocations. Defaults to Times.Once if null.</param>
    /// <returns>The builder instance for method chaining.</returns>
    protected MockedValidatorBuilder<TModel> WithFailedValidationResult(
        TModel expectedModel,
        IEqualityComparer<TModel> equalityComparer,
        Dictionary<string, string> failures,
        Times? times = null)
    {
        var validationFailures = failures
            .Select(keyValuePair => new ValidationFailure(keyValuePair.Key, keyValuePair.Value))
            .ToList();

        _mock
            .Setup(validator => validator.ValidateAsync(It.Is(expectedModel, equalityComparer), default))
            .ReturnsAsync(new ValidationResult(validationFailures))
            .Verifiable(times ?? Times.Once());
        return this;
    }

    /// <summary>
    /// Configures the validator to return a successful validation result with no errors.
    /// This abstract method must be implemented by derived classes to provide model-specific equality comparison.
    /// </summary>
    /// <param name="expectedModel">The model instance that the validator should match.</param>
    /// <param name="times">The expected number of validator invocations. Defaults to Times.Once if null.</param>
    /// <returns>The builder instance for method chaining.</returns>
    public abstract MockedValidatorBuilder<TModel> WithSuccessfulValidationResult(
        TModel expectedModel,
        Times? times = null);

    /// <summary>
    /// Configures the validator to return a successful validation result with no errors using a custom equality comparer.
    /// </summary>
    /// <param name="expectedModel">The model instance that the validator should match.</param>
    /// <param name="equalityComparer">The equality comparer used to match the model instance.</param>
    /// <param name="times">The expected number of validator invocations. Defaults to Times.Once if null.</param>
    /// <returns>The builder instance for method chaining.</returns>
    protected MockedValidatorBuilder<TModel> WithSuccessfulValidationResult(
        TModel expectedModel,
        IEqualityComparer<TModel> equalityComparer,
        Times? times = null)
    {
        _mock
            .Setup(validator => validator.ValidateAsync(It.Is(expectedModel, equalityComparer), default))
            .ReturnsAsync(new ValidationResult())
            .Verifiable(times ?? Times.Once());

        return this;
    }
}
