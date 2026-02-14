using Tsa.Submissions.Coding.WebApi.Entities;

namespace Tsa.Submissions.Coding.WebApi.Services.Cache;

/// <summary>
///     Provides methods for building consistent cache keys for problem operations.
/// </summary>
public sealed class ProblemCacheKeys : CacheKeyBuilder
{
    private const string ProblemIdPrefix = "problem_id";
    private const string ProblemsCollectionKey = "problems";

    /// <summary>
    ///     Gets the cache key for the collection of all problems.
    /// </summary>
    public static string AllProblems()
    {
        return ProblemsCollectionKey;
    }

    /// <summary>
    ///     Builds a cache key for a specific problem by its ID.
    /// </summary>
    public static string ById(string problemId)
    {
        return BuildKey(ProblemIdPrefix, problemId);
    }

    /// <summary>
    ///     Builds a cache key for a problem entity.
    /// </summary>
    public static string ForEntity(Problem problem)
    {
        return ById(problem.Id!);
    }
}
