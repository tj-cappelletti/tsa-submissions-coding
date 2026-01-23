using Tsa.Submissions.Coding.ApiClient.Clients;

namespace Tsa.Submissions.Coding.ApiClient;

public interface ICodingApiClient
{
    IProblemsClient Problems { get; }

    ISubmissionsClient Submissions { get; }

    ITestSetsClient TestSets { get; }
}
