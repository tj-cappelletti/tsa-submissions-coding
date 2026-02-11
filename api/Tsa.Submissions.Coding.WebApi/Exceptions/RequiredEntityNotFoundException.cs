using System;
using System.Net;
using Tsa.Submissions.Coding.Contracts;
using Tsa.Submissions.Coding.WebApi.Models;

namespace Tsa.Submissions.Coding.WebApi.Exceptions;

//TODO: Evaluate if this exception is the right approach for handling missing required entities.
//Exceptions are generally used for unexpected situations, and missing entities might be better handled through validation or specific error responses.
public class RequiredEntityNotFoundException : Exception, IWebApiException
{
    public string EntityName { get; }

    public HttpStatusCode HttpStatusCode { get; }

    public RequiredEntityNotFoundException(string entityName) : base($"Could not locate required resource for the `{entityName}` entity.")
    {
        EntityName = entityName;
        HttpStatusCode = HttpStatusCode.NotFound;
    }

    public ApiErrorResponse ToApiErrorResponse()
    {
        return new ApiErrorResponse((int)ErrorCodes.RequiredEntityNotFound, Message);
    }
}
