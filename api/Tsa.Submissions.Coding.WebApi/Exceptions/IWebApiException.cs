using System.Net;
using Tsa.Submissions.Coding.Contracts;

namespace Tsa.Submissions.Coding.WebApi.Exceptions;

public interface IWebApiException
{
    HttpStatusCode HttpStatusCode { get; }

    ApiErrorResponse ToApiErrorResponse();
}
