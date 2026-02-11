using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using Tsa.Submissions.Coding.WebApi.Configuration;
using Tsa.Submissions.Coding.WebApi.Entities;

namespace Tsa.Submissions.Coding.WebApi.Services;

/// <summary>
///     Provides data access and caching operations for user entities.
///     All read operations are cached for 2 hours to optimize performance during competitions.
/// </summary>
/// <remarks>
///     This service overrides base <see cref="MongoDbService{T}" /> methods to implement caching.
///     Cache invalidation occurs automatically on create, update, and delete operations.
///     The 2-hour cache duration aligns with the typical competition time window.
/// </remarks>
public sealed class UsersService : MongoDbService<User>, IUsersService
{
    public const string MongoDbCollectionName = "users";

    private const string UserIdCacheKey = "user_id";
    private const string UserNameCacheKey = "user_name";
    private const string UsersCacheKey = "users";

    public string CollectionName => MongoDbCollectionName;

    public string ServiceName => "Users";

    public UsersService(
        ICacheService cacheService,
        IMongoClient mongoClient,
        IOptions<SubmissionsDatabase> options,
        ILogger<UsersService> logger) : base(
        cacheService,
        mongoClient,
        options.Value.Name!,
        MongoDbCollectionName,
        logger) { }

    /// <summary>
    ///     Creates a new user in the database and populates the cache.
    /// </summary>
    /// <param name="entity">The user entity to create</param>
    /// <param name="cancellationToken">The cancellation token</param>
    public override async Task CreateAsync(User entity, CancellationToken cancellationToken = default)
    {
        await base.CreateAsync(entity, cancellationToken);

        await SetCacheAsync($"{UserIdCacheKey}:{entity.Id}", entity, cancellationToken);
        await SetCacheAsync($"{UserNameCacheKey}:{entity.UserName}", entity, cancellationToken);

        await InvalidateUsersCacheAsync(cancellationToken);
    }

    /// <summary>
    ///     Gets all users from the database. Results are cached for 2 hours.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token</param>
    /// <returns>A list of all users</returns>
    public override async Task<List<User>> GetAsync(CancellationToken cancellationToken = default)
    {
        return await GetOrSetCacheAsync(
            $"{UsersCacheKey}",
            async ct => await base.GetAsync(ct),
            cancellationToken
        );
    }

    /// <summary>
    ///     Gets a user by their unique identifier. Results are cached for 2 hours.
    /// </summary>
    /// <param name="id">The user's unique identifier</param>
    /// <param name="cancellationToken">The cancellation token</param>
    /// <returns>The user if found; otherwise, null</returns>
    public override async Task<User?> GetAsync(string id, CancellationToken cancellationToken = default)
    {
        return await GetOrSetCacheAsync(
            $"{UserIdCacheKey}:{id}",
            async ct => await base.GetAsync(id, ct),
            cancellationToken
        );
    }

    /// <summary>
    ///     Gets a user by their username. Results are cached for 2 hours.
    /// </summary>
    /// <param name="userName">The username to search for</param>
    /// <param name="cancellationToken">The cancellation token</param>
    /// <returns>The user if found; otherwise, null</returns>
    public async Task<User?> GetByUserNameAsync(string? userName, CancellationToken cancellationToken = default)
    {
        return await GetOrSetCacheAsync(
            $"{UserNameCacheKey}:{userName}",
            async ct => await GetByUserNameFromDatabaseAsync(userName, ct),
            cancellationToken
        );
    }

    private async Task<User?> GetByUserNameFromDatabaseAsync(string? userName, CancellationToken cancellationToken = default)
    {
        var filterDefinition = Builders<User>.Filter.Eq(user => user.UserName, userName);

        var cursor = await EntityCollection.FindAsync(filterDefinition, cancellationToken: cancellationToken);

        var result = await cursor.SingleOrDefaultAsync(cancellationToken);

        return result;
    }

    private async Task InvalidateUserCacheAsync(User entity, CancellationToken cancellationToken)
    {
        await CacheService.RemoveAsync($"{UserIdCacheKey}:{entity.Id}", cancellationToken);
        await CacheService.RemoveAsync($"{UserNameCacheKey}:{entity.UserName}", cancellationToken);

        await InvalidateUsersCacheAsync(cancellationToken);
    }

    private async Task InvalidateUsersCacheAsync(CancellationToken cancellationToken)
    {
        await CacheService.RemoveAsync(UsersCacheKey, cancellationToken);
    }

    /// <summary>
    ///     Removes a user from the database and invalidates all related cache entries.
    /// </summary>
    /// <param name="entity">The user entity to remove</param>
    /// <param name="cancellationToken">The cancellation token</param>
    public override async Task RemoveAsync(User entity, CancellationToken cancellationToken = default)
    {
        await base.RemoveAsync(entity, cancellationToken);

        await InvalidateUserCacheAsync(entity, cancellationToken);
    }

    /// <summary>
    ///     Updates a user in the database and invalidates all related cache entries.
    /// </summary>
    /// <param name="entity">The user entity with updated values</param>
    /// <param name="cancellationToken">The cancellation token</param>
    public override async Task UpdateAsync(User entity, CancellationToken cancellationToken = default)
    {
        await base.UpdateAsync(entity, cancellationToken);

        await InvalidateUserCacheAsync(entity, cancellationToken);
    }
}
