using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading;
using Tsa.Submissions.Coding.WebApi.Entities;

namespace Tsa.Submissions.Coding.WebApi.Services;

/// <summary>
/// Defines the contract for MongoDB entity data access operations.
/// Implementations may provide additional features such as caching or validation.
/// </summary>
/// <typeparam name="T">The entity type that implements <see cref="IMongoDbEntity"/></typeparam>
public interface IMongoEntityService<T> where T : IMongoDbEntity, new()
{
    /// <summary>
    /// Gets the name of the MongoDB collection for this entity type.
    /// </summary>
    string CollectionName { get; }

    /// <summary>
    /// Creates a new entity in the database.
    /// </summary>
    /// <param name="entity">The entity to create</param>
    /// <param name="cancellationToken">The cancellation token</param>
    /// <returns>A task representing the asynchronous operation</returns>
    Task CreateAsync(T entity, CancellationToken cancellationToken = default);

    /// <summary>
    /// Checks whether an entity with the specified ID exists in the database.
    /// </summary>
    /// <param name="id">The unique identifier to check</param>
    /// <param name="cancellationToken">The cancellation token</param>
    /// <returns>True if the entity exists; otherwise, false</returns>
    Task<bool> ExistsAsync(string id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all entities from the database.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token</param>
    /// <returns>A list of all entities</returns>
    Task<List<T>> GetAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets multiple entities by their unique identifiers.
    /// </summary>
    /// <param name="ids">The collection of unique identifiers</param>
    /// <param name="cancellationToken">The cancellation token</param>
    /// <returns>A list of entities matching the provided IDs</returns>
    Task<List<T>> GetAsync(IEnumerable<string> ids, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a single entity by its unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier</param>
    /// <param name="cancellationToken">The cancellation token</param>
    /// <returns>The entity if found; otherwise, null</returns>
    Task<T?> GetAsync(string id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Removes an entity from the database.
    /// </summary>
    /// <param name="entity">The entity to remove</param>
    /// <param name="cancellationToken">The cancellation token</param>
    /// <returns>A task representing the asynchronous operation</returns>
    Task RemoveAsync(T entity, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates an existing entity in the database.
    /// </summary>
    /// <param name="entity">The entity with updated values</param>
    /// <param name="cancellationToken">The cancellation token</param>
    /// <returns>A task representing the asynchronous operation</returns>
    Task UpdateAsync(T entity, CancellationToken cancellationToken = default);
}

