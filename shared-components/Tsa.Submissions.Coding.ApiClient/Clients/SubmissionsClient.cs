using RestSharp;
using Tsa.Submissions.Coding.Contracts.Submissions;

namespace Tsa.Submissions.Coding.ApiClient.Clients;

public class SubmissionsClient : ISubmissionsClient
{
    private readonly IRestClient _restClient;

    public SubmissionsClient(IRestClient restClient)
    {
        _restClient = restClient;
    }

    public async Task<SubmissionResponse> GetAsync(string id, CancellationToken cancellationToken)
    {
        var request = new RestRequest($"/api/submissions/{id}");

        var response = await _restClient.GetAsync<SubmissionResponse>(request, cancellationToken);

        if (response == null)
        {
            throw new InvalidOperationException("Failed to deserialize submission response");
        }

        return response;
    }
}
