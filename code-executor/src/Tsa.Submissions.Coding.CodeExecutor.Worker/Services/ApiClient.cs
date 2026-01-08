using System.Net.Http.Json;
using System.Text.Json;
using Tsa.Submissions.Coding.CodeExecutor.Shared.Models;

namespace Tsa.Submissions.Coding.CodeExecutor.Worker.Services;

/// <summary>
/// Client for interacting with the TSA Submissions API
/// </summary>
public class ApiClient
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<ApiClient> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="ApiClient"/> class
    /// </summary>
    public ApiClient(HttpClient httpClient, ILogger<ApiClient> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    /// <summary>
    /// Gets submission details and test cases from the API
    /// </summary>
    /// <param name="submissionId">The submission identifier</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Execution payload with submission details and test cases</returns>
    public async Task<ExecutionPayload?> GetSubmissionAsync(string submissionId, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Fetching submission {SubmissionId} from API", submissionId);
            
            // This is a placeholder - actual API endpoints would be defined in the API project
            var response = await _httpClient.GetAsync($"/api/submissions/{submissionId}/execution", cancellationToken);
            
            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError("Failed to fetch submission {SubmissionId}: {StatusCode}", submissionId, response.StatusCode);
                return null;
            }

            var payload = await response.Content.ReadFromJsonAsync<ExecutionPayload>(cancellationToken);
            return payload;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching submission {SubmissionId}", submissionId);
            return null;
        }
    }

    /// <summary>
    /// Updates the API with test execution results
    /// </summary>
    /// <param name="result">The execution result</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>True if update was successful</returns>
    public async Task<bool> UpdateSubmissionResultsAsync(ExecutionResult result, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Updating submission {SubmissionId} with results", result.SubmissionId);
            
            // This is a placeholder - actual API endpoints would be defined in the API project
            var response = await _httpClient.PostAsJsonAsync($"/api/submissions/{result.SubmissionId}/results", result, cancellationToken);
            
            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError("Failed to update submission {SubmissionId}: {StatusCode}", result.SubmissionId, response.StatusCode);
                return false;
            }

            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating submission {SubmissionId}", result.SubmissionId);
            return false;
        }
    }
}
