using System;
using Microsoft.AspNetCore.Mvc;

namespace Tsa.Submissions.Coding.WebApi.Validators;

public class ValidatedResult
{
    /// <summary>
    ///     The error result associated with a failed validation.
    /// </summary>
    public IActionResult? ErrorResult { get; init; }

    /// <summary>
    ///     Flag indicating whether the validation failed.
    /// </summary>
    public bool IsInvalid => !IsValid;

    /// <summary>
    ///     Flag indicating whether the validation was successful.
    /// </summary>
    public bool IsValid { get; init; }

    /// <summary>
    ///     Creates a <see cref="ValidatedResult" /> that represents a failed validation with the specified error result.
    /// </summary>
    /// <param name="error">
    ///     The <see cref="IActionResult" /> that describes the error to associate with the failed validation. Cannot be
    ///     <see
    ///         langword="null" />
    ///     .
    /// </param>
    /// <returns>
    ///     A <see cref="ValidatedResult" /> instance with <c>IsValid</c> set to <see langword="false" /> and the specified
    ///     error result.
    /// </returns>
    public static ValidatedResult Failure(IActionResult error)
    {
        return new ValidatedResult { IsValid = false, ErrorResult = error };
    }

    /// <summary>
    ///     Returns the error result associated with a failed validation.
    /// </summary>
    /// <remarks>
    ///     This method should only be called when the validation has failed. Calling this method when
    ///     the validation is successful will result in an exception.
    /// </remarks>
    /// <returns>The <see cref="IActionResult" /> representing the error details for the failed validation.</returns>
    /// <exception cref="InvalidOperationException">Thrown if the validation is successful and there is no error to return.</exception>
    public IActionResult GetError()
    {
        if (IsValid)
        {
            throw new InvalidOperationException("Cannot get error from a successful validation result.");
        }

        return ErrorResult!;
    }

    /// <summary>
    ///     Creates a successful validation result.
    /// </summary>
    /// <returns>The <see cref="ValidatedResult" /> representing the successful validation.</returns>
    public static ValidatedResult Success()
    {
        return new ValidatedResult { IsValid = true };
    }
}
