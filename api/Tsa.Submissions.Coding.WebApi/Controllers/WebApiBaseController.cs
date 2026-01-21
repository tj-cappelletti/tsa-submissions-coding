using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Tsa.Submissions.Coding.Contracts;
using Tsa.Submissions.Coding.WebApi.Exceptions;
using Tsa.Submissions.Coding.WebApi.Validators;

namespace Tsa.Submissions.Coding.WebApi.Controllers;

public abstract class WebApiBaseController : ControllerBase
{
    protected static ApiErrorResponse ApiErrorEntityAlreadyExists(string entityName, string lookupKey)
    {
        var data = new Dictionary<string, string>
        {
            { "entityName", entityName },
            { "lookupKey", lookupKey }
        };

        return new ApiErrorResponse(data, (int)ErrorCodes.EntityAlreadyExists, "The resource requested to create already exists.");
    }

    protected static ApiErrorResponse ApiErrorEntityNotFound(string entityName, string lookupKey)
    {
        var data = new Dictionary<string, string>
        {
            { "entityName", entityName },
            { "lookupKey", lookupKey }
        };

        return new ApiErrorResponse(data, (int)ErrorCodes.EntityNotFound, "The requested resource could not be found.");
    }

    protected static ApiErrorResponse ApiErrorInvalidId()
    {
        return new ApiErrorResponse((int)ErrorCodes.InvalidId, "The ID provided is not valid.");
    }

    protected ApiErrorResponse ApiErrorSubmissionAlreadyEvaluated()
    {
        return new ApiErrorResponse((int)ErrorCodes.SubmissionAlreadyEvaluated, "The submission has already been evaluated and cannot be modified.");
    }

    protected static ApiErrorResponse ApiErrorUnauthorized()
    {
        return new ApiErrorResponse((int)ErrorCodes.Unauthorized, "Client is unauthorized");
    }

    protected static ApiErrorResponse ApiErrorUnexpectedMissingResource()
    {
        return new ApiErrorResponse((int)ErrorCodes.UnexpectedMissingResource, "A dependent resource could not be loaded while making this call.");
    }

    protected static ApiErrorResponse ApiErrorUnexpectedNullValue()
    {
        return new ApiErrorResponse((int)ErrorCodes.UnexpectedNullValue, "A required value was unexpectedly missing.");
    }

    /// <summary>
    ///     A generic method to validate a model using a specified FluentValidation validator.
    /// </summary>
    /// <typeparam name="T">The type of the model being validated.</typeparam>
    /// <param name="model">The instance of the model to validate.</param>
    /// <param name="validator">The <see cref="IValidator{T}" /> instance used for validation.</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken" /> to observe while waiting for the task to complete.</param>
    /// <returns>The <see cref="ValidatedResult" /> representing the outcome of the validation.</returns>
    protected async Task<ValidatedResult> ValidateAsync<T>(T model, IValidator<T> validator, CancellationToken cancellationToken)
    {
        var result = await validator.ValidateAsync(model, cancellationToken);

        if (result.IsValid)
        {
            return ValidatedResult.Success();
        }

        return ValidatedResult.Failure(BadRequest(new
        {
            errors = result.Errors.Select(e => new
            {
                field = e.PropertyName,
                message = e.ErrorMessage
            })
        }));
    }
}
