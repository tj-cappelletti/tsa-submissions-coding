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
///     Provides data access and caching operations for problem language variant entities.
///     All read operations are cached for 4 hours to optimize performance during competitions.
///     This service is expected to be hit heavily by the code executor during submission evaluation.
/// </summary>
/// <remarks>
///     This service overrides base <see cref="MongoDbService{T}" /> methods to implement caching.
///     Cache invalidation occurs automatically on create, update, and delete operations.
///     The 4-hour cache duration aligns with the typical competition time window.
/// </remarks>
public class ProblemLanguageVariantsService : MongoDbService<ProblemLanguageVariant>, IProblemLanguageVariantsService
{
    public const string MongoDbCollectionName = "problem_language_variants";

    /// <summary>
    ///     Gets the name of the MongoDB collection for problem language variants.
    /// </summary>
    public string CollectionName => MongoDbCollectionName;

    /// <summary>
    ///     Gets the service name for diagnostic and logging purposes.
    /// </summary>
    public string ServiceName => "ProblemLanguageVariants";

    /// <summary>
    ///     Initializes a new instance of the <see cref="ProblemLanguageVariantsService" /> class.
    /// </summary>
    /// <param name="cacheService">The cache service for storing and retrieving cached data</param>
    /// <param name="mongoClient">The MongoDB client instance</param>
    /// <param name="options">The database configuration options</param>
    /// <param name="logger">The logger instance</param>
    public ProblemLanguageVariantsService(
        ICacheService cacheService,
        IMongoClient mongoClient,
        IOptions<SubmissionsDatabase> options,
        ILogger<ProblemLanguageVariantsService> logger)
        : base(
            cacheService,
            mongoClient,
            options.Value.Name!,
            MongoDbCollectionName,
            logger) { }

    /// <summary>
    ///     Creates a new problem language variant in the database and populates the cache.
    /// </summary>
    /// <param name="entity">The variant entity to create</param>
    /// <param name="cancellationToken">The cancellation token</param>
    public override async Task CreateAsync(ProblemLanguageVariant entity, CancellationToken cancellationToken = default)
    {
        await base.CreateAsync(entity, cancellationToken);

        await InvalidateVariantsCacheAsync(cancellationToken);
        await InvalidateVariantsByProblemCacheAsync(entity, cancellationToken);

        await SetCacheAsync(ProblemLanguageVariantCacheKeys.ForEntity(entity), entity, cancellationToken);
    }

    /// <summary>
    ///     Checks if a variant exists for a specific problem, programming language, and version combination.
    /// </summary>
    /// <param name="problemId">The unique identifier of the problem</param>
    /// <param name="programmingLanguageId">The unique identifier of the programming language</param>
    /// <param name="versionTag">The programming language version tag</param>
    /// <param name="cancellationToken">The cancellation token</param>
    /// <returns>True if a variant exists for the combination; otherwise, false</returns>
    public async Task<bool> ExistsForProblemLanguageAndVersionAsync(string problemId, string programmingLanguageId, string versionTag,
        CancellationToken cancellationToken = default)
    {
        var variant = await GetByProblemLanguageAndVersionAsync(problemId, programmingLanguageId, versionTag, cancellationToken);

        return variant != null;
    }

    /// <summary>
    ///     Gets all problem language variants from the database. Results are cached for 4 hours.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token</param>
    /// <returns>A list of all problem language variants</returns>
    public override async Task<List<ProblemLanguageVariant>> GetAsync(CancellationToken cancellationToken = default)
    {
        return await GetOrSetCacheAsync(
            ProblemLanguageVariantCacheKeys.AllVariants(),
            async ct => await base.GetAsync(ct),
            cancellationToken
        );
    }

    /// <summary>
    ///     Gets a problem language variant by its unique identifier. Results are cached for 4 hours.
    /// </summary>
    /// <param name="id">The variant's unique identifier</param>
    /// <param name="cancellationToken">The cancellation token</param>
    /// <returns>The variant if found; otherwise, null</returns>
    public override async Task<ProblemLanguageVariant?> GetAsync(string id, CancellationToken cancellationToken = default)
    {
        return await GetOrSetCacheAsync(
            ProblemLanguageVariantCacheKeys.ById(id),
            async ct => await base.GetAsync(id, ct),
            cancellationToken
        );
    }

    /// <summary>
    ///     Retrieves all variants for a specific problem. Results are cached for 4 hours.
    /// </summary>
    /// <param name="problem">The problem entity</param>
    /// <param name="cancellationToken">The cancellation token</param>
    /// <returns>A list of variants for the problem</returns>
    public async Task<List<ProblemLanguageVariant>> GetByProblemAsync(Problem problem, CancellationToken cancellationToken = default)
    {
        return await GetOrSetCacheAsync(
            ProblemLanguageVariantCacheKeys.ByProblemId(problem.Id!),
            async ct =>
            {
                var filterDefinition = Builders<ProblemLanguageVariant>.Filter.Eq(
                    variant => variant.ProblemId, problem.Id);

                var cursor = await EntityCollection.FindAsync(filterDefinition, null, ct);

                return await cursor.ToListAsync(ct);
            },
            cancellationToken
        );
    }

    /// <summary>
    ///     Retrieves a variant for a specific problem, programming language, and version combination.
    ///     Results are cached for 4 hours. This is the primary lookup used by the code executor.
    /// </summary>
    /// <param name="problemId">The unique identifier of the problem</param>
    /// <param name="programmingLanguageId">The unique identifier of the programming language</param>
    /// <param name="versionTag">The programming language version tag</param>
    /// <param name="cancellationToken">The cancellation token</param>
    /// <returns>The variant if found; otherwise, null</returns>
    public async Task<ProblemLanguageVariant?> GetByProblemLanguageAndVersionAsync(string problemId, string programmingLanguageId, string versionTag,
        CancellationToken cancellationToken = default)
    {
        return await GetOrSetCacheAsync(
            ProblemLanguageVariantCacheKeys.ByProblemLanguageAndVersion(problemId, programmingLanguageId, versionTag),
            async ct =>
            {
                var filterDefinition = Builders<ProblemLanguageVariant>.Filter.And(
                    Builders<ProblemLanguageVariant>.Filter.Eq(variant => variant.ProblemId, problemId),
                    Builders<ProblemLanguageVariant>.Filter.Eq(variant => variant.ProgrammingLanguageId, programmingLanguageId),
                    Builders<ProblemLanguageVariant>.Filter.Eq(variant => variant.ProgrammingLanguageVersionTag, versionTag));

                var cursor = await EntityCollection.FindAsync(filterDefinition, null, ct);

                return await cursor.FirstOrDefaultAsync(ct);
            },
            cancellationToken
        );
    }

    /// <summary>
    ///     Invalidates all cache entries for a specific variant and related collections.
    /// </summary>
    /// <param name="variant">The variant whose cache entries should be invalidated</param>
    /// <param name="cancellationToken">The cancellation token</param>
    private async Task InvalidateVariantCacheAsync(ProblemLanguageVariant variant, CancellationToken cancellationToken = default)
    {
        await CacheService.RemoveAsync(ProblemLanguageVariantCacheKeys.ForEntity(variant), cancellationToken);
        await CacheService.RemoveAsync(ProblemLanguageVariantCacheKeys.ForEntityProblemLanguageAndVersion(variant), cancellationToken);

        await InvalidateVariantsByProblemCacheAsync(variant, cancellationToken);
        await InvalidateVariantsCacheAsync(cancellationToken);
    }

    /// <summary>
    ///     Invalidates the cached collection of variants for a specific problem.
    /// </summary>
    /// <param name="variant">The variant whose problem's cache should be invalidated</param>
    /// <param name="cancellationToken">The cancellation token</param>
    private async Task InvalidateVariantsByProblemCacheAsync(ProblemLanguageVariant variant, CancellationToken cancellationToken = default)
    {
        await CacheService.RemoveAsync(ProblemLanguageVariantCacheKeys.ForEntityProblem(variant), cancellationToken);
    }

    /// <summary>
    ///     Invalidates the cached collection of all problem language variants.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token</param>
    private async Task InvalidateVariantsCacheAsync(CancellationToken cancellationToken = default)
    {
        await CacheService.RemoveAsync(ProblemLanguageVariantCacheKeys.AllVariants(), cancellationToken);
    }

    /// <summary>
    ///     Removes a variant from the database and invalidates all related cache entries.
    /// </summary>
    /// <param name="entity">The variant entity to remove</param>
    /// <param name="cancellationToken">The cancellation token</param>
    public override async Task RemoveAsync(ProblemLanguageVariant entity, CancellationToken cancellationToken = default)
    {
        await base.RemoveAsync(entity, cancellationToken);

        await InvalidateVariantCacheAsync(entity, cancellationToken);
    }

    /// <summary>
    ///     Updates a variant in the database and refreshes its cache entry.
    /// </summary>
    /// <param name="entity">The variant entity with updated values</param>
    /// <param name="cancellationToken">The cancellation token</param>
    /// <remarks>
    ///     This method updates the individual variant cache with the new data
    ///     and invalidates the collection caches to ensure consistency.
    /// </remarks>
    public override async Task UpdateAsync(ProblemLanguageVariant entity, CancellationToken cancellationToken = default)
    {
        await base.UpdateAsync(entity, cancellationToken);

        await InvalidateVariantCacheAsync(entity, cancellationToken);

        await SetCacheAsync(ProblemLanguageVariantCacheKeys.ForEntity(entity), entity, cancellationToken);
    }
}
