using Tsa.Submissions.Coding.Contracts.TestSets;

namespace Tsa.Submissions.Coding.ApiClient.Clients;

public interface ITestSetsClient
{
    Task<List<TestSetResponse>> GetAsync(CancellationToken cancellationToken);

    Task<TestSetResponse> GetAsync(string testSetId, CancellationToken cancellationToken);

    Task<List<TestSetResponse>> GetByProblemIdAsync(string problemId, CancellationToken cancellationToken);
}
