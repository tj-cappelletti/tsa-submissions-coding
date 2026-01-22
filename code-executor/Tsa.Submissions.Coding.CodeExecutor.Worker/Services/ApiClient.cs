using Microsoft.Extensions.Logging;
using System.Net.Http.Json;
using Tsa.Submissions.Coding.Contracts.Submissions;

namespace Tsa.Submissions.Coding.CodeExecutor.Worker.Services;

public class ApiClient
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<ApiClient> _logger;

    public ApiClient(HttpClient httpClient, ILogger<ApiClient> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task<SubmissionResponse> GetSubmissionAsync(string submissionId, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Fetching submission {SubmissionId} from API", submissionId);

        var response = await _httpClient.GetAsync($"/api/submissions/{submissionId}", cancellationToken);

        response.EnsureSuccessStatusCode();

        var submissionModel = await response.Content.ReadFromJsonAsync<SubmissionResponse>(cancellationToken);

        if (submissionModel is not null) return submissionModel;

        var content = await response.Content.ReadAsStringAsync(cancellationToken);
        _logger.LogError("Failed to deserialize submission {SubmissionId} from API response: {Content}", submissionId, content);

        throw new InvalidOperationException($"Failed to deserialize submission {submissionId}");
    }
}
