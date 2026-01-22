using System.Net.Http.Headers;
using RestSharp.Interceptors;
using Tsa.Submissions.Coding.Contracts.Authentication;

namespace Tsa.Submissions.Coding.ApiClient.Interceptors;

public class AuthenticationInterceptor : Interceptor
{
    private readonly Func<Task<AuthenticationResponse>> _authenticateFunc;
    private readonly string _loginEndpoint;
    private readonly SemaphoreSlim _loginLock = new(1, 1);
    private string? _accessToken;
    private DateTimeOffset _tokenExpiration;

    private bool IsLoggedIn => !string.IsNullOrEmpty(_accessToken) && DateTimeOffset.UtcNow < _tokenExpiration;

    public AuthenticationInterceptor(Func<Task<AuthenticationResponse>> authenticateFunc, string loginEndpoint)
    {
        _authenticateFunc = authenticateFunc;
        _loginEndpoint = loginEndpoint;
    }

    public override async ValueTask BeforeHttpRequest(HttpRequestMessage requestMessage, CancellationToken cancellationToken)
    {
        // Don't add auth header to login requests
        if (requestMessage.RequestUri?.AbsolutePath.Contains(_loginEndpoint) == true)
        {
            return;
        }

        await EnsureValidTokenAsync(cancellationToken);

        // Add the bearer token to the request
        requestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _accessToken);
    }

    private async Task EnsureValidTokenAsync(CancellationToken cancellationToken)
    {
        if (IsLoggedIn) return;

        // Acquire the lock to prevent multiple simultaneous logins
        await _loginLock.WaitAsync(cancellationToken);

        try
        {
            // Race condition check
            if (IsLoggedIn) return;

            var authenticationResponse = await _authenticateFunc();
            _accessToken = authenticationResponse.Token;
            _tokenExpiration = authenticationResponse.Expiration;
        }
        finally
        {
            _loginLock.Release();
        }
    }
}
