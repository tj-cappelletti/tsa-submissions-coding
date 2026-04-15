using Microsoft.Extensions.Logging;
using Tsa.Submissions.Coding.ApiClient;
using Tsa.Submissions.Coding.CodeExecutor.Worker.Handlers;
using Tsa.Submissions.Coding.Contracts.CodeExecutor;

namespace Tsa.Submissions.Coding.CodeExecutor.Worker.Handlers;

public class TestCaseResultHandler : IResultHandler<RunnerResult>
{
    private readonly ICodingApiClient _apiClient;
    private readonly ILogger<TestCaseResultHandler> _logger;

    public TestCaseResultHandler(ICodingApiClient apiClient, ILogger<TestCaseResultHandler> logger)
    {
        _apiClient = apiClient;
        _logger = logger;
    }

    public async Task HandleAsync(
        RunnerJobPayload payload,
        RunnerResult result,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Updating test results for submission {SubmissionId}", payload.SubmissionId);

        try
        {
            // Update submission with test results
            await _apiClient.Submissions.PostTestCaseResultsAsync(
                payload.SubmissionId,
                result.TestCaseResults.ToList(),
                cancellationToken);

            _logger.LogInformation("Successfully updated test results for submission {SubmissionId}", payload.SubmissionId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to update test results for submission {SubmissionId}", payload.SubmissionId);
            throw;
        }
    }
}
