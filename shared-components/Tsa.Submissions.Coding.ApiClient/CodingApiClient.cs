using RestSharp;
using Tsa.Submissions.Coding.ApiClient.Clients;
using Tsa.Submissions.Coding.ApiClient.Interceptors;
using Tsa.Submissions.Coding.Contracts.Authentication;

namespace Tsa.Submissions.Coding.ApiClient;

public class CodingApiClient : ICodingApiClient
{
    private readonly string _loginEndpoint;
    private readonly string _password;
    private readonly RestClient _restClient;
    private readonly string _username;

    public IProblemsClient Problems { get; }

    public ISubmissionsClient Submissions { get; }

    public ITestSetsClient TestSets { get; }

    public CodingApiClient(Uri baseUri, string username, string password, string loginEndpoint = "api/auth/login")
    {
        _loginEndpoint = loginEndpoint;
        _password = password;
        _username = username;

        var authInterceptor = new AuthenticationInterceptor(PerformLoginAsync, _loginEndpoint);

        var options = new RestClientOptions(baseUri)
        {
            Interceptors = [authInterceptor]
        };

        _restClient = new RestClient(options);

        Problems = new ProblemsClient(_restClient);
        Submissions = new SubmissionsClient(_restClient);
        TestSets = new TestSetsClient(_restClient);
    }

    private async Task<AuthenticationResponse> PerformLoginAsync()
    {
        var request = new RestRequest(_loginEndpoint, Method.Post);

        var authenticationRequest = new AuthenticationRequest(_password, _username);
        request.AddJsonBody(authenticationRequest);

        var response = await _restClient.PostAsync<AuthenticationResponse>(request);

        if (response == null) throw new InvalidOperationException("Failed to deserialize authentication response.");

        return response;
    }
}
