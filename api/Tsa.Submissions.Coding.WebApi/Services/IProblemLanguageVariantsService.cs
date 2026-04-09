using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Tsa.Submissions.Coding.WebApi.Entities;

namespace Tsa.Submissions.Coding.WebApi.Services;

public interface IProblemLanguageVariantsService : IMongoEntityService<ProblemLanguageVariant>, IPingableService
{
    /// <summary>
    ///     Checks if a variant exists for a specific problem, programming language, and version combination.
    /// </summary>
    /// <param name="problemId">The unique identifier of the problem</param>
    /// <param name="programmingLanguageId">The unique identifier of the programming language</param>
    /// <param name="versionTag">The programming language version tag</param>
    /// <param name="cancellationToken">The cancellation token</param>
    /// <returns>True if a variant exists for the combination; otherwise, false</returns>
    Task<bool> ExistsForProblemLanguageAndVersionAsync(string problemId, string programmingLanguageId, string versionTag,
        CancellationToken cancellationToken = default);

    /// <summary>
    ///     Retrieves all variants for a specific problem.
    /// </summary>
    /// <param name="problem">The problem entity</param>
    /// <param name="cancellationToken">The cancellation token</param>
    /// <returns>A list of variants for the problem</returns>
    Task<List<ProblemLanguageVariant>> GetByProblemAsync(Problem problem, CancellationToken cancellationToken = default);

    /// <summary>
    ///     Retrieves a variant for a specific problem, programming language, and version combination.
    /// </summary>
    /// <param name="problemId">The unique identifier of the problem</param>
    /// <param name="programmingLanguageId">The unique identifier of the programming language</param>
    /// <param name="versionTag">The programming language version tag</param>
    /// <param name="cancellationToken">The cancellation token</param>
    /// <returns>The variant if found; otherwise, null</returns>
    Task<ProblemLanguageVariant?> GetByProblemLanguageAndVersionAsync(string problemId, string programmingLanguageId, string versionTag,
        CancellationToken cancellationToken = default);
}
