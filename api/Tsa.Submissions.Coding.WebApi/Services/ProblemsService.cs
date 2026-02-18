using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using Tsa.Submissions.Coding.WebApi.Configuration;
using Tsa.Submissions.Coding.WebApi.Entities;
using Tsa.Submissions.Coding.WebApi.Services.Cache;

namespace Tsa.Submissions.Coding.WebApi.Services;

/// <summary>
///     Provides data access and caching operations for problem entities.
///     All read operations are cached for 4 hours to optimize performance during competitions.
/// </summary>
/// <remarks>
///     This service overrides base <see cref="MongoDbService{T}" /> methods to implement caching.
///     Cache invalidation occurs automatically on create, update, and delete operations.
///     The 4-hour cache duration aligns with the typical competition time window.
/// </remarks>
public class ProblemsService : MongoDbService<Problem>, IProblemsService
{
    public const string MongoDbCollectionName = "problems";

    /// <summary>
    ///     Gets the name of the MongoDB collection for problems.
    /// </summary>
    public string CollectionName => MongoDbCollectionName;

    /// <summary>
    ///     Gets the service name for diagnostic and logging purposes.
    /// </summary>
    public string ServiceName => "Problems";

    /// <summary>
    ///     Initializes a new instance of the <see cref="ProblemsService" /> class.
    /// </summary>
    /// <param name="cacheService">The cache service for storing and retrieving cached data</param>
    /// <param name="mongoClient">The MongoDB client instance</param>
    /// <param name="options">The database configuration options</param>
    /// <param name="logger">The logger instance</param>
    public ProblemsService(
        ICacheService cacheService,
        IMongoClient mongoClient,
        IOptions<SubmissionsDatabase> options,
        ILogger<ProblemsService> logger)
        : base(
            cacheService,
            mongoClient,
            options.Value.Name!,
            MongoDbCollectionName,
            logger)
    { }

    /// <summary>
    ///     Creates a new problem in the database and populates the cache.
    /// </summary>
    /// <param name="entity">The problem entity to create</param>
    /// <param name="cancellationToken">The cancellation token</param>
    /// <returns>A task representing the asynchronous operation</returns>
    public override async Task CreateAsync(Problem entity, CancellationToken cancellationToken = default)
    {
        await base.CreateAsync(entity, cancellationToken);

        await InvalidateProblemsCacheAsync(cancellationToken);

        await SetCacheAsync(ProblemCacheKeys.ForEntity(entity), entity, cancellationToken);
    }

    /// <summary>
    ///     Gets all problems from the database. Results are cached for 4 hours.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token</param>
    /// <returns>A list of all problems</returns>
    public override async Task<List<Problem>> GetAsync(CancellationToken cancellationToken = default)
    {
        return await GetOrSetCacheAsync(
            ProblemCacheKeys.AllProblems(),
            async ct => await base.GetAsync(ct),
            cancellationToken
        );
    }

    /// <summary>
    ///     Gets a problem by its unique identifier. Results are cached for 4 hours.
    /// </summary>
    /// <param name="id">The problem's unique identifier</param>
    /// <param name="cancellationToken">The cancellation token</param>
    /// <returns>The problem if found; otherwise, null</returns>
    public override async Task<Problem?> GetAsync(string id, CancellationToken cancellationToken = default)
    {
        return await GetOrSetCacheAsync(
            ProblemCacheKeys.ById(id),
            async ct => await base.GetAsync(id, ct),
            cancellationToken
        );
    }

    /// <summary>
    ///     Invalidates all cache entries for a specific problem and the problems collection.
    /// </summary>
    /// <param name="problem">The problem whose cache entries should be invalidated</param>
    /// <param name="cancellationToken">The cancellation token</param>
    /// <returns>A task representing the asynchronous operation</returns>
    private async Task InvalidateProblemCacheAsync(Problem problem, CancellationToken cancellationToken = default)
    {
        await CacheService.RemoveAsync(ProblemCacheKeys.ById(problem.Id!), cancellationToken);

        await InvalidateProblemsCacheAsync(cancellationToken);
    }

    /// <summary>
    ///     Invalidates the cached collection of all problems.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token</param>
    /// <returns>A task representing the asynchronous operation</returns>
    private async Task InvalidateProblemsCacheAsync(CancellationToken cancellationToken = default)
    {
        await CacheService.RemoveAsync(ProblemCacheKeys.AllProblems(), cancellationToken);
    }

    /// <summary>
    ///     Removes a problem from the database and invalidates all related cache entries.
    /// </summary>
    /// <param name="entity">The problem entity to remove</param>
    /// <param name="cancellationToken">The cancellation token</param>
    /// <returns>A task representing the asynchronous operation</returns>
    public override async Task RemoveAsync(Problem entity, CancellationToken cancellationToken = default)
    {
        await base.RemoveAsync(entity, cancellationToken);

        await InvalidateProblemCacheAsync(entity, cancellationToken);
    }

    /// <summary>
    ///     Updates a problem in the database and refreshes its cache entry.
    /// </summary>
    /// <param name="entity">The problem entity with updated values</param>
    /// <param name="cancellationToken">The cancellation token</param>
    /// <returns>A task representing the asynchronous operation</returns>
    /// <remarks>
    ///     This method updates the individual problem cache with the new data
    ///     and invalidates the collection cache to ensure consistency.
    /// </remarks>
    public override async Task UpdateAsync(Problem entity, CancellationToken cancellationToken = default)
    {
        await base.UpdateAsync(entity, cancellationToken);

        // Invalidate old cache entries and refresh cache after updating to ensure consistency
        await InvalidateProblemCacheAsync(entity, cancellationToken);

        await SetCacheAsync(ProblemCacheKeys.ForEntity(entity), entity, cancellationToken);
    }
}
