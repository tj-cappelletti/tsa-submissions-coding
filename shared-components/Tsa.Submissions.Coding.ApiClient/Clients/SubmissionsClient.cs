using System.Text.Json;
using RestSharp;
using Tsa.Submissions.Coding.ApiClient.Guards;
using Tsa.Submissions.Coding.Contracts.Submissions;
using Tsa.Submissions.Coding.Contracts.TestCases;

namespace Tsa.Submissions.Coding.ApiClient.Clients;

public class SubmissionsClient : ISubmissionsClient
{
    private readonly IRestClient _restClient;

    //TODO: Add logging and telemetry during logging feature development
    public SubmissionsClient(IRestClient restClient)
    {
        _restClient = restClient;
    }

    //TODO: Consider moving this to a shared utility class if deserialization logic is needed elsewhere
    private static SubmissionResponse DeserializeSubmissionResponse(string? content)
    {
        try
        {
            var submissionResponse = content != null
                ? JsonSerializer.Deserialize<SubmissionResponse>(content)
                : null;

            if (submissionResponse == null)
            {
                throw new InvalidOperationException("Failed to deserialize submission response");
            }

            return submissionResponse;
        }
        catch (JsonException ex)
        {
            throw new InvalidOperationException("Failed to deserialize submission response", ex);
        }
    }

    public async Task<SubmissionResponse> GetAsync(string id, CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(id);

        var request = new RestRequest($"/api/submissions/{id}");

        var response = await _restClient.GetAsync(request, cancellationToken);

        ApiResponseGuard.EnsureSuccessfulStatusCode(response, "Submission", id);

        return DeserializeSubmissionResponse(response.Content);
    }

    public async Task<SubmissionResponse> PostTestCaseResultsAsync(
        string id,
        IList<TestCaseResultRequest> testCaseResults,
        CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(id);
        ArgumentNullException.ThrowIfNull(testCaseResults);

        if (testCaseResults.Count == 0)
        {
            throw new ArgumentException("Test case results cannot be empty", nameof(testCaseResults));
        }

        var request = new RestRequest($"/api/submissions/{id}/test-cases");

        request.AddJsonBody(testCaseResults);

        var response = await _restClient.PostAsync(request, cancellationToken);

        ApiResponseGuard.EnsureSuccessfulStatusCode(response, "Submission", id);

        return DeserializeSubmissionResponse(response.Content);
    }
}
