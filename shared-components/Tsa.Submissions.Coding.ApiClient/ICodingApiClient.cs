using Tsa.Submissions.Coding.ApiClient.Clients;

namespace Tsa.Submissions.Coding.ApiClient;

public interface ICodingApiClient
{
    ISubmissionsClient Submissions { get; }
}
