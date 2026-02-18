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
///     Provides data access and caching operations for programming language entities.
///     All read operations are cached for 4 hours to optimize performance during competitions.
/// </summary>
/// <remarks>
///     This service overrides base <see cref="MongoDbService{T}" /> methods to implement caching.
///     Cache invalidation occurs automatically on create, update, and delete operations.
///     The 4-hour cache duration aligns with the typical competition time window.
/// </remarks>
public class ProgrammingLanguagesService : MongoDbService<ProgrammingLanguage>, IProgrammingLanguagesService
{
    public const string MongoDbCollectionName = "programming_languages";

    /// <summary>
    ///     Gets the name of the MongoDB collection for programming languages.
    /// </summary>
    public string CollectionName => MongoDbCollectionName;

    /// <summary>
    ///     Gets the service name for diagnostic and logging purposes.
    /// </summary>
    public string ServiceName => "ProgrammingLanguages";

    /// <summary>
    ///     Initializes a new instance of the <see cref="ProgrammingLanguagesService" /> class.
    /// </summary>
    /// <param name="cacheService">The cache service for storing and retrieving cached data</param>
    /// <param name="mongoClient">The MongoDB client instance</param>
    /// <param name="options">The database configuration options</param>
    /// <param name="logger">The logger instance</param>
    public ProgrammingLanguagesService(
        ICacheService cacheService,
        IMongoClient mongoClient,
        IOptions<SubmissionsDatabase> options,
        ILogger<ProgrammingLanguagesService> logger)
        : base(
            cacheService,
            mongoClient,
            options.Value.Name!,
            MongoDbCollectionName,
            logger)
    { }

    /// <summary>
    ///     Creates a new programming language in the database and populates the cache.
    /// </summary>
    /// <param name="entity">The programming language entity to create</param>
    /// <param name="cancellationToken">The cancellation token</param>
    /// <returns>A task representing the asynchronous operation</returns>
    /// <remarks>
    ///     This method invalidates the collection cache and then populates the individual cache entry
    ///     for the newly created programming language.
    /// </remarks>
    public override async Task CreateAsync(ProgrammingLanguage entity, CancellationToken cancellationToken = default)
    {
        await base.CreateAsync(entity, cancellationToken);

        await InvalidateProgrammingLanguagesAsync(cancellationToken);

        await SetCacheAsync(ProgrammingLanguageCacheKeys.ForEntity(entity), entity, cancellationToken);
    }

    /// <summary>
    ///     Gets all programming languages from the database. Results are cached for 4 hours.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token</param>
    /// <returns>A list of all programming languages</returns>
    public override async Task<List<ProgrammingLanguage>> GetAsync(CancellationToken cancellationToken = default)
    {
        return await GetOrSetCacheAsync(
            ProgrammingLanguageCacheKeys.AllProgrammingLanguages(),
            async ct => await base.GetAsync(ct),
            cancellationToken
        );
    }

    /// <summary>
    ///     Gets a programming language by its unique identifier. Results are cached for 4 hours.
    /// </summary>
    /// <param name="id">The programming language's unique identifier</param>
    /// <param name="cancellationToken">The cancellation token</param>
    /// <returns>The programming language if found; otherwise, null</returns>
    public override async Task<ProgrammingLanguage?> GetAsync(string id, CancellationToken cancellationToken = default)
    {
        return await GetOrSetCacheAsync(
            ProgrammingLanguageCacheKeys.ById(id),
            async ct => await base.GetAsync(id, ct),
            cancellationToken
        );
    }

    /// <summary>
    ///     Invalidates the cached collection of all programming languages.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token</param>
    /// <returns>A task representing the asynchronous operation</returns>
    private async Task InvalidateProgrammingLanguagesAsync(CancellationToken cancellationToken = default)
    {
        await CacheService.RemoveAsync(ProgrammingLanguageCacheKeys.AllProgrammingLanguages(), cancellationToken);
    }

    /// <summary>
    ///     Invalidates all cache entries for a specific programming language and the collection cache.
    /// </summary>
    /// <param name="programmingLanguage">The programming language whose cache entries should be invalidated</param>
    /// <param name="cancellationToken">The cancellation token</param>
    /// <returns>A task representing the asynchronous operation</returns>
    private async Task InvalidateProgrammingLanguageCacheAsync(ProgrammingLanguage programmingLanguage, CancellationToken cancellationToken = default)
    {
        await CacheService.RemoveAsync(ProgrammingLanguageCacheKeys.ForEntity(programmingLanguage), cancellationToken);

        await InvalidateProgrammingLanguagesAsync(cancellationToken);
    }

    /// <summary>
    ///     Removes a programming language from the database and invalidates all related cache entries.
    /// </summary>
    /// <param name="entity">The programming language entity to remove</param>
    /// <param name="cancellationToken">The cancellation token</param>
    /// <returns>A task representing the asynchronous operation</returns>
    public override async Task RemoveAsync(ProgrammingLanguage entity, CancellationToken cancellationToken = default)
    {
        await base.RemoveAsync(entity, cancellationToken);

        await InvalidateProgrammingLanguageCacheAsync(entity, cancellationToken);
    }

    /// <summary>
    ///     Updates a programming language in the database and invalidates all related cache entries.
    /// </summary>
    /// <param name="entity">The programming language entity with updated values</param>
    /// <param name="cancellationToken">The cancellation token</param>
    /// <returns>A task representing the asynchronous operation</returns>
    /// <remarks>
    ///     This method invalidates cache entries rather than refreshing them to ensure
    ///     the next read fetches the most current data from the database, including
    ///     any changes to nested version collections.
    /// </remarks>
    public override async Task UpdateAsync(ProgrammingLanguage entity, CancellationToken cancellationToken = default)
    {
        await base.UpdateAsync(entity, cancellationToken);

        await InvalidateProgrammingLanguageCacheAsync(entity, cancellationToken);
    }
}
