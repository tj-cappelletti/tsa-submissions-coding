using System.Text.Json;
using RestSharp;
using Tsa.Submissions.Coding.Contracts.Problems;

namespace Tsa.Submissions.Coding.ApiClient.Clients;

public class ProblemsClient : IProblemsClient
{
    private readonly IRestClient _restClient;

    public ProblemsClient(IRestClient restClient)
    {
        _restClient = restClient;
    }

    public async Task<ProblemResponse> GetAsync(string problemId, bool includeTestCases = false, CancellationToken cancellationToken = default)
    {
        var request = new RestRequest($"/api/problems/{problemId}");

        if (includeTestCases)
        {
            request.AddParameter("expandTestCases", true);
        }

        var response = await _restClient.GetAsync(request, cancellationToken);

        if (response == null)
        {
            throw new InvalidOperationException("Failed to fetch the problem.");
        }

        var problemResponse = response.Content != null
            ? JsonSerializer.Deserialize<ProblemResponse>(response.Content)
            : null;

        if (problemResponse == null)
        {
            throw new InvalidOperationException("Failed to deserialize problem response.");
        }

        return problemResponse;
    }
}
