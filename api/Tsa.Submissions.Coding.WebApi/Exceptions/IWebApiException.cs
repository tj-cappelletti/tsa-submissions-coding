using System.Net;
using Tsa.Submissions.Coding.WebApi.Models;

namespace Tsa.Submissions.Coding.WebApi.Exceptions;

public interface IWebApiException
{
    HttpStatusCode HttpStatusCode { get; }

    ApiErrorResponseModel ToApiErrorResponseModel();
}