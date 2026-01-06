using System;
using System.Net;
using Tsa.Submissions.Coding.WebApi.Models;

namespace Tsa.Submissions.Coding.WebApi.Exceptions;

public class RequiredEntityNotFoundException : Exception, IWebApiException
{
    public string EntityName { get; }

    public HttpStatusCode HttpStatusCode { get; }

    public RequiredEntityNotFoundException(string entityName) : base($"Could not locate required resource for the `{entityName}` entity.")
    {
        EntityName = entityName;
        HttpStatusCode = HttpStatusCode.NotFound;
    }

    public ApiErrorResponseModel ToApiErrorResponseModel()
    {
        return new ApiErrorResponseModel
        {
            ErrorCode = (int)ErrorCodes.RequiredEntityNotFound,
            Message = Message
        };
    }
}
