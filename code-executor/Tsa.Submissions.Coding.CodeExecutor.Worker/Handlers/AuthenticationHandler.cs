using System.Net.Http.Headers;
using System.Net.Http.Json;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Tsa.Submissions.Coding.CodeExecutor.Worker.Configuration;
using Tsa.Submissions.Coding.Contracts.Authentication;

namespace Tsa.Submissions.Coding.CodeExecutor.Worker.Handlers;

public class AuthenticationHandler : DelegatingHandler
{
    private readonly SubmissionsApiConfig _config;
    private readonly ILogger<AuthenticationHandler> _logger;
    private readonly SemaphoreSlim _semaphore = new(1, 1);
    private AuthenticationResponse? _authenticationResponse;

    public AuthenticationHandler(ILogger<AuthenticationHandler> logger, IOptions<SubmissionsApiConfig> config)
    {
        _logger = logger;
        _config = config.Value;
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing) _semaphore.Dispose();

        base.Dispose(disposing);
    }

    private async Task EnsureTokenAsync(CancellationToken cancellationToken)
    {
        // Use semaphore to prevent multiple concurrent login attempts
        await _semaphore.WaitAsync(cancellationToken);

        try
        {
            // Check if token is still valid (with 5-minute buffer)
            if (_authenticationResponse != null && _authenticationResponse.Expiration > DateTime.UtcNow.AddMinutes(5)) return; // Token still valid

            _logger.LogInformation("Authenticating API client with username {Username}", _config.Authentication.Username);

            var authModel = new AuthenticationRequest(_config.Authentication.Password, _config.Authentication.Username);

            var loginUri = new Uri(new Uri(_config.BaseUrl), "/api/auth/login");

            var loginRequest = new HttpRequestMessage(HttpMethod.Post, loginUri)
            {
                Content = JsonContent.Create(authModel)
            };

            var response = await base.SendAsync(loginRequest, cancellationToken);
            response.EnsureSuccessStatusCode();

            _authenticationResponse = await response.Content.ReadFromJsonAsync<AuthenticationResponse>(cancellationToken);

            if (_authenticationResponse == null) throw new InvalidOperationException("Failed to deserialize login response");

            _logger.LogInformation("Successfully authenticated. Token expires at {Expiration}", _authenticationResponse.Expiration);
        }
        finally
        {
            _semaphore.Release();
        }
    }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        // Skip auth for login endpoint
        if (request.RequestUri?.PathAndQuery.Contains("/api/auth/login") == true) return await base.SendAsync(request, cancellationToken);

        // Ensure we have a valid token
        await EnsureTokenAsync(cancellationToken);

        // Add bearer token to request
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _authenticationResponse!.Token);

        return await base.SendAsync(request, cancellationToken);
    }
}
