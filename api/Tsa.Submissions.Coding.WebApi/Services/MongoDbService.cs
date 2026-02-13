using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using MongoDB.Driver;
using Tsa.Submissions.Coding.WebApi.Entities;

namespace Tsa.Submissions.Coding.WebApi.Services;

/// <summary>
///     Provides base MongoDB data access functionality for entities.
///     This class can be extended to add features such as caching, validation, or auditing.
///     Methods are virtual to allow derived classes to override behavior while maintaining the base implementation.
/// </summary>
/// <typeparam name="T">The entity type that implements <see cref="IMongoDbEntity" /></typeparam>
public abstract class MongoDbService<T> where T : IMongoDbEntity
{
    private readonly TimeSpan _cacheExpiration = TimeSpan.FromHours(4);
    private readonly string _collectionName;
    private readonly string _databaseName;
    protected readonly ICacheService CacheService;
    protected readonly IMongoCollection<T> EntityCollection;
    protected readonly ILogger<MongoDbService<T>> Logger;
    protected readonly IMongoDatabase MongoDatabase;

    /// <summary>
    ///     Initializes a new instance of the <see cref="MongoDbService{T}" /> class.
    /// </summary>
    /// <param name="cacheService">The cache service for derived classes that implement caching</param>
    /// <param name="mongoClient">The MongoDB client instance</param>
    /// <param name="databaseName">The name of the database</param>
    /// <param name="collectionName">The name of the collection</param>
    /// <param name="logger">The logger instance</param>
    protected MongoDbService(ICacheService cacheService, IMongoClient mongoClient, string databaseName,
        string collectionName, ILogger<MongoDbService<T>> logger)
    {
        _collectionName = collectionName;
        _databaseName = databaseName;

        CacheService = cacheService;
        MongoDatabase = mongoClient.GetDatabase(databaseName);

        EntityCollection = MongoDatabase.GetCollection<T>(collectionName);

        Logger = logger;
    }

    /// <summary>
    ///     Creates a new entity in the database.
    ///     This method is virtual and can be overridden by derived classes to add additional behavior such as cache
    ///     population.
    /// </summary>
    /// <param name="entity">The entity to create</param>
    /// <param name="cancellationToken">The cancellation token</param>
    public virtual async Task CreateAsync(T entity, CancellationToken cancellationToken = default)
    {
        await EntityCollection.InsertOneAsync(entity, null, cancellationToken);
    }

    /// <summary>
    ///     Checks if an entity exists in the database.
    /// </summary>
    /// <param name="id">The id of the entity</param>
    /// <param name="cancellationToken">The cancellation token</param>
    /// <returns>True if the entity exists, otherwise false</returns>
    public virtual async Task<bool> ExistsAsync(string id, CancellationToken cancellationToken = default)
    {
        var filterDefinition = Builders<T>.Filter.Eq(mongoDbEntity => mongoDbEntity.Id, id);

        var cursor = await EntityCollection.FindAsync(filterDefinition, null, cancellationToken);

        return await cursor.AnyAsync(cancellationToken);
    }

    /// <summary>
    ///     Retrieves all entities from the database.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token</param>
    /// <returns>A list of entities</returns>
    public virtual async Task<List<T>> GetAsync(CancellationToken cancellationToken = default)
    {
        var cursor = await EntityCollection.FindAsync(Builders<T>.Filter.Empty, null, cancellationToken);

        var result = await cursor.ToListAsync(cancellationToken);

        return result;
    }

    /// <summary>
    ///     Retrieves multiple entities by their ids.
    /// </summary>
    /// <param name="ids">The ids of the entities</param>
    /// <param name="cancellationToken">The cancellation token</param>
    /// <returns>A list of entities</returns>
    public virtual async Task<List<T>> GetAsync(IEnumerable<string> ids, CancellationToken cancellationToken = default)
    {
        var filterDefinition = Builders<T>.Filter.In(mongoDbEntity => mongoDbEntity.Id, ids);

        var cursor = await EntityCollection.FindAsync(filterDefinition, null, cancellationToken);

        var result = await cursor.ToListAsync(cancellationToken);

        return result;
    }

    /// <summary>
    ///     Retrieves an entity by its id.
    /// </summary>
    /// <param name="id">The id of the entity</param>
    /// <param name="cancellationToken">The cancellation token</param>
    /// <returns>The entity, or null if not found</returns>
    public virtual async Task<T?> GetAsync(string id, CancellationToken cancellationToken = default)
    {
        var filterDefinition = Builders<T>.Filter.Eq(mongoDbEntity => mongoDbEntity.Id, id);

        var cursor = await EntityCollection.FindAsync(filterDefinition, null, cancellationToken);

        var result = await cursor.FirstOrDefaultAsync(cancellationToken);

        return result;
    }

    /// <summary>
    ///     Retrieves data from cache or fetches from the database and caches the result.
    ///     This helper method implements the cache-aside pattern for derived classes.
    /// </summary>
    /// <typeparam name="TResult">The type of data being cached</typeparam>
    /// <param name="cacheKey">The cache key to use</param>
    /// <param name="fetchFromService">The function to fetch data if not in cache</param>
    /// <param name="cancellationToken">The cancellation token</param>
    /// <returns>The cached or freshly fetched data</returns>
    protected async Task<TResult> GetOrSetCacheAsync<TResult>(string cacheKey,
        Func<CancellationToken, Task<TResult>> fetchFromService, CancellationToken cancellationToken)
    {
        var cachedData = await CacheService.GetAsync<TResult>(cacheKey, cancellationToken);

        if (cachedData != null) return cachedData;

        var data = await fetchFromService(cancellationToken);

        if (data != null)
        {
            await CacheService.SetAsync(cacheKey, data, _cacheExpiration, cancellationToken);
        }

        return data;
    }

    /// <summary>
    ///     Pings the MongoDB collection to check connectivity.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token</param>
    /// <returns>True if the ping was successful, otherwise false</returns>
    public virtual async Task<bool> PingAsync(CancellationToken cancellationToken = default)
    {
        var successful = true;

        try
        {
            await EntityCollection.Database.RunCommandAsync((Command<BsonDocument>)"{ping:1}", cancellationToken: cancellationToken);
        }
        catch (Exception exception)
        {
            Logger.LogError(exception, "Error pinging the MongoDb collection {Collection} in the database {DatabaseName}",
                _collectionName, _databaseName);
            successful = false;
        }

        //TODO: Create a PingResult class that contains more information about the ping result such as the time taken to ping the database, and any error messages if the ping fails.
        return successful;
    }

    /// <summary>
    ///     Removes an entity from the database.
    ///     This method is virtual and can be overridden by derived classes to add additional behavior such as cache removal.
    /// </summary>
    /// <param name="entity">The entity to remove</param>
    /// <param name="cancellationToken">The cancellation token</param>
    public virtual async Task RemoveAsync(T entity, CancellationToken cancellationToken = default)
    {
        var filterDefinition = Builders<T>.Filter.Eq(mongoDbEntity => mongoDbEntity.Id, entity.Id);

        await EntityCollection.DeleteOneAsync(filterDefinition, null, cancellationToken);
    }

    /// <summary>
    ///     Stores data in the cache with the configured expiration time.
    /// </summary>
    /// <param name="cacheKey">The cache key</param>
    /// <param name="data">The data to cache</param>
    /// <param name="cancellationToken">The cancellation token</param>
    protected async Task SetCacheAsync(string cacheKey, T data, CancellationToken cancellationToken = default)
    {
        await CacheService.SetAsync(cacheKey, data, _cacheExpiration, cancellationToken);
    }

    /// <summary>
    ///     Updates an existing entity in the database.
    ///     This method is virtual and can be overridden by derived classes to add additional behavior such as cache updates.
    /// </summary>
    /// <param name="entity">The entity to update</param>
    /// <param name="cancellationToken">The cancellation token</param>
    public virtual async Task UpdateAsync(T entity, CancellationToken cancellationToken = default)
    {
        var filterDefinition = Builders<T>.Filter.Eq(mongoDbEntity => mongoDbEntity.Id, entity.Id);

        var replaceOptions = new ReplaceOptions
        {
            IsUpsert = false
        };

        await EntityCollection.ReplaceOneAsync(filterDefinition, entity, replaceOptions, cancellationToken);
    }
}
