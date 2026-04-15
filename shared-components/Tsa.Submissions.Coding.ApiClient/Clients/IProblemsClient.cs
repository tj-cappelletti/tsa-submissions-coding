using Tsa.Submissions.Coding.Contracts.ProblemLanguageVariants;
using Tsa.Submissions.Coding.Contracts.Problems;

namespace Tsa.Submissions.Coding.ApiClient.Clients;

public interface IProblemsClient
{
    Task<ProblemResponse> GetAsync(string problemId, bool includeTestCases, CancellationToken cancellationToken);

    Task<ProblemLanguageVariantResponse> GetLanguageVariantAsync(
        string problemId,
        string languageId,
        string versionTag,
        CancellationToken cancellationToken);
}
