using System.Threading;
using System.Threading.Tasks;
using Tsa.Submissions.Coding.WebApi.Entities;

namespace Tsa.Submissions.Coding.WebApi.Services;

/// <summary>
///     Defines the contract for event data access and state management operations.
/// </summary>
public interface IEventService : IMongoEntityService<Event>
{
    /// <summary>
    ///     Gets the current event document, if one exists.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token</param>
    /// <returns>The current event, or null if no event has been created</returns>
    Task<Event?> GetCurrentAsync(CancellationToken cancellationToken = default);

    /// <summary>
    ///     Starts the event by creating or replacing the event document with the given start and end times.
    /// </summary>
    /// <param name="durationInMinutes">The duration of the event in minutes</param>
    /// <param name="cancellationToken">The cancellation token</param>
    /// <returns>The started event</returns>
    Task<Event> StartAsync(int durationInMinutes, CancellationToken cancellationToken = default);

    /// <summary>
    ///     Stops the event by setting IsActive to false.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token</param>
    /// <returns>The stopped event, or null if no event exists</returns>
    Task<Event?> StopAsync(CancellationToken cancellationToken = default);

    /// <summary>
    ///     Sets a per-user end time override for the current event.
    /// </summary>
    /// <param name="userId">The ID of the user whose end time should be overridden</param>
    /// <param name="endTime">The new end time for the user</param>
    /// <param name="cancellationToken">The cancellation token</param>
    /// <returns>The updated event, or null if no event exists</returns>
    Task<Event?> SetUserEndTimeOverrideAsync(string userId, System.DateTimeOffset endTime, CancellationToken cancellationToken = default);
}
