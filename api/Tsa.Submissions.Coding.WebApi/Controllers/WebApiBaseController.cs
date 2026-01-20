using System.Linq;
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
    /// <typeparam name="T"></typeparam>
    /// <param name="model"></param>
    /// <param name="validator"></param>
    /// <returns>The <see cref="ValidatedResult" /> representing the outcome of the validation.</returns>
    protected async Task<ValidatedResult> ValidateAsync<T>(T model, IValidator<T> validator)
    {
        var result = await validator.ValidateAsync(model);

        if (result.IsValid)
        {
            return ValidatedResult.Success();
        }

        return ValidatedResult.Failure(BadRequest(new
        {
            errors = result.Errors.Select(selector: e => new
            {
                field = e.PropertyName,
                message = e.ErrorMessage
            })
        }));
    }
}
