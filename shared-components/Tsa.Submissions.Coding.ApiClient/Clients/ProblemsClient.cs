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

    public async Task<ProblemResponse> GetAsync(string problemId, CancellationToken cancellationToken)
    {
        var request = new RestRequest($"/api/problems/{problemId}");

        var response = await _restClient.GetAsync<ProblemResponse>(request, cancellationToken);

        if (response == null)
        {
            throw new InvalidOperationException("Failed to deserialize problem response.");
        }

        return response;
    }
}
