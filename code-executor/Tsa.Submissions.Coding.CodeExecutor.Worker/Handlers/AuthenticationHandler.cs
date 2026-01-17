using System.Net.Http.Headers;
using System.Net.Http.Json;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Tsa.Submissions.Coding.CodeExecutor.Shared.Models;
using Tsa.Submissions.Coding.CodeExecutor.Worker.Configuration;

namespace Tsa.Submissions.Coding.CodeExecutor.Worker.Handlers;

public class AuthenticationHandler : DelegatingHandler
{
    private readonly SubmissionsApiConfig _config;
    private readonly ILogger<AuthenticationHandler> _logger;
    private readonly SemaphoreSlim _semaphore = new(1, 1);
    private LoginResponseModel? _loginResponse;

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
            if (_loginResponse != null && _loginResponse.Expiration > DateTime.UtcNow.AddMinutes(5)) return; // Token still valid

            _logger.LogInformation("Authenticating API client with username {Username}", _config.Authentication.Username);

            var authModel = new AuthenticationModel
            {
                UserName = _config.Authentication.Username,
                Password = _config.Authentication.Password
            };

            var loginUri = new Uri(new Uri(_config.BaseUrl), "/api/auth/login");

            var loginRequest = new HttpRequestMessage(HttpMethod.Post, loginUri)
            {
                Content = JsonContent.Create(authModel)
            };

            var response = await base.SendAsync(loginRequest, cancellationToken);
            response.EnsureSuccessStatusCode();

            _loginResponse = await response.Content.ReadFromJsonAsync<LoginResponseModel>(cancellationToken);

            if (_loginResponse == null) throw new InvalidOperationException("Failed to deserialize login response");

            _logger.LogInformation("Successfully authenticated. Token expires at {Expiration}", _loginResponse.Expiration);
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
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _loginResponse!.Token);

        return await base.SendAsync(request, cancellationToken);
    }
}
