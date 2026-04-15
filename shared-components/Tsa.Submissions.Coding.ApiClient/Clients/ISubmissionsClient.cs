using Tsa.Submissions.Coding.Contracts.Submissions;
using Tsa.Submissions.Coding.Contracts.TestCases;

namespace Tsa.Submissions.Coding.ApiClient.Clients;

public interface ISubmissionsClient
{
    Task<SubmissionResponse> GetAsync(string id, CancellationToken cancellationToken);

    Task<SubmissionResponse> PostTestCaseResultsAsync(string id, IList<TestCaseResultRequest> testCaseResults, CancellationToken cancellationToken);
}
