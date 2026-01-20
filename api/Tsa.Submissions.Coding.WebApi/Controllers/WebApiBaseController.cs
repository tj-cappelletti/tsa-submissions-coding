using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Tsa.Submissions.Coding.WebApi.Validators;

namespace Tsa.Submissions.Coding.WebApi.Controllers;

public abstract class WebApiBaseController : ControllerBase
{
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
