using RestSharp;
using Tsa.Submissions.Coding.Contracts.TestSets;

namespace Tsa.Submissions.Coding.ApiClient.Clients;

public class TestSetsClient : ITestSetsClient
{
    private readonly IRestClient _restClient;

    public TestSetsClient(IRestClient restClient)
    {
        _restClient = restClient;
    }

    public async Task<List<TestSetResponse>> GetAsync(CancellationToken cancellationToken)
    {
        var request = new RestRequest($"/api/testsets");

        var response = await _restClient.GetAsync<List<TestSetResponse>>(request, cancellationToken);

        if (response == null)
        {
            throw new InvalidOperationException("Failed to deserialize test set response.");
        }

        return response;
    }

    public async Task<TestSetResponse> GetAsync(string testSetId, CancellationToken cancellationToken)
    {
        var request = new RestRequest($"/api/testsets/{testSetId}");

        var response = await _restClient.GetAsync<TestSetResponse>(request, cancellationToken);

        if (response == null)
        {
            throw new InvalidOperationException("Failed to deserialize test set response.");
        }

        return response;
    }

    public async Task<List<TestSetResponse>> GetByProblemIdAsync(string problemId, CancellationToken cancellationToken)
    {
        var request = new RestRequest($"/api/problems/{problemId}/testsets");

        var response = await _restClient.GetAsync<List<TestSetResponse>>(request, cancellationToken);

        if (response == null)
        {
            throw new InvalidOperationException("Failed to deserialize test sets response.");
        }

        return response;
    }
}
