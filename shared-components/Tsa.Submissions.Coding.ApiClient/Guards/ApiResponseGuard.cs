using System.Net;
using RestSharp;

namespace Tsa.Submissions.Coding.ApiClient.Guards;

internal static class ApiResponseGuard
{
    public static void EnsureSuccessfulStatusCode(
        RestResponse response,
        string? resourceType = null,
        string? resourceId = null)
    {
        if (response.IsSuccessful) return;

        var errorMessage = response.StatusCode switch
        {
            HttpStatusCode.NotFound when resourceType != null && resourceId != null
                => $"{resourceType} with ID {resourceId} not found.",
            HttpStatusCode.NotFound
                => "Resource not found.",
            HttpStatusCode.BadRequest
                => $"Bad request: {response.Content ?? "No details provided"}",
            HttpStatusCode.Unauthorized
                => "Unauthorized. Please check your authentication credentials.",
            HttpStatusCode.Forbidden
                => "Forbidden. You do not have permission to access this resource.",
            HttpStatusCode.Conflict
                => $"Conflict: {response.Content ?? "Resource conflict occurred"}",
            >= HttpStatusCode.InternalServerError
                => $"Server error ({(int)response.StatusCode}): {response.ErrorMessage ?? "Unknown server error"}",
            _
                => $"Request failed with status code {(int)response.StatusCode}: {response.ErrorMessage ?? "Unknown error"}"
        };

        throw response.StatusCode switch
        {
            HttpStatusCode.NotFound => new KeyNotFoundException(errorMessage),
            HttpStatusCode.BadRequest => new ArgumentException(errorMessage),
            HttpStatusCode.Unauthorized or HttpStatusCode.Forbidden => new UnauthorizedAccessException(errorMessage),
            HttpStatusCode.Conflict => new InvalidOperationException(errorMessage),
            _ => new HttpRequestException(errorMessage)
        };
    }
}
