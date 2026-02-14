using System.Threading;
using System.Threading.Tasks;
using Tsa.Submissions.Coding.WebApi.Entities;

namespace Tsa.Submissions.Coding.WebApi.Services;

/// <summary>
///     Defines the contract for user data access operations with caching.
///     All read operations are automatically cached for 4 hours to optimize performance during competitions.
///     Cache entries are automatically invalidated when users are created, updated, or deleted.
/// </summary>
/// <remarks>
///     This service overrides base <see cref="IMongoEntityService{T}" /> methods to provide caching behavior.
///     When using this interface, all operations will use the cached implementations.
/// </remarks>
public interface IUsersService : IMongoEntityService<User>, IPingableService
{
    /// <summary>
    ///     Gets a user by their username. Results are cached for 4 hours.
    /// </summary>
    /// <param name="userName">The username to search for</param>
    /// <param name="cancellationToken">The cancellation token</param>
    /// <returns>The user if found; otherwise, null</returns>
    Task<User?> GetByUserNameAsync(string userName, CancellationToken cancellationToken = default);
}
