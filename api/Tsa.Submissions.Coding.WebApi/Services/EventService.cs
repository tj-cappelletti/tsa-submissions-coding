using System;
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
///     Provides data access and state management for the global event.
///     The event is stored as a single document in MongoDB and cached briefly to reduce database load.
/// </summary>
public sealed class EventService : MongoDbService<Event>, IEventService
{
    private static readonly TimeSpan EventCacheExpiration = TimeSpan.FromSeconds(30);

    public const string MongoDbCollectionName = "event";

    /// <summary>
    ///     Gets the name of the MongoDB collection used for the event entity.
    /// </summary>
    public string CollectionName => MongoDbCollectionName;

    /// <summary>
    ///     Gets the service name for diagnostic and logging purposes.
    /// </summary>
    public string ServiceName => "Event";

    public EventService(
        ICacheService cacheService,
        IMongoClient mongoClient,
        IOptions<SubmissionsDatabase> options,
        ILogger<EventService> logger)
        : base(
            cacheService,
            mongoClient,
            options.Value.Name!,
            MongoDbCollectionName,
            logger)
    { }

    /// <inheritdoc />
    public async Task<Event?> GetCurrentAsync(CancellationToken cancellationToken = default)
    {
        var cached = await CacheService.GetAsync<Event>(EventCacheKeys.CurrentEvent(), cancellationToken);

        if (cached != null) return cached;

        var cursor = await EntityCollection.FindAsync(Builders<Event>.Filter.Empty, null, cancellationToken);
        var current = await cursor.FirstOrDefaultAsync(cancellationToken);

        if (current != null)
        {
            await CacheService.SetAsync(EventCacheKeys.CurrentEvent(), current, EventCacheExpiration, cancellationToken);
        }

        return current;
    }

    /// <inheritdoc />
    public async Task<Event> StartAsync(int durationInMinutes, CancellationToken cancellationToken = default)
    {
        var now = DateTimeOffset.UtcNow;

        var existing = await GetCurrentAsync(cancellationToken);

        Event @event;

        if (existing == null)
        {
            @event = new Event
            {
                IsActive = true,
                StartTime = now,
                EndTime = now.AddMinutes(durationInMinutes)
            };

            await CreateAsync(@event, cancellationToken);
        }
        else
        {
            existing.IsActive = true;
            existing.StartTime = now;
            existing.EndTime = now.AddMinutes(durationInMinutes);
            existing.UserOverrides.Clear();

            await UpdateAsync(existing, cancellationToken);

            @event = existing;
        }

        await CacheService.SetAsync(EventCacheKeys.CurrentEvent(), @event, EventCacheExpiration, cancellationToken);

        return @event;
    }

    /// <inheritdoc />
    public async Task<Event?> StopAsync(CancellationToken cancellationToken = default)
    {
        var existing = await GetCurrentAsync(cancellationToken);

        if (existing == null) return null;

        existing.IsActive = false;

        await UpdateAsync(existing, cancellationToken);

        await CacheService.SetAsync(EventCacheKeys.CurrentEvent(), existing, EventCacheExpiration, cancellationToken);

        return existing;
    }

    /// <inheritdoc />
    public async Task<Event?> SetUserEndTimeOverrideAsync(string userId, DateTimeOffset endTime, CancellationToken cancellationToken = default)
    {
        var existing = await GetCurrentAsync(cancellationToken);

        if (existing == null) return null;

        var override_ = existing.UserOverrides.Find(o => o.UserId == userId);

        if (override_ == null)
        {
            existing.UserOverrides.Add(new EventUserOverride { UserId = userId, EndTime = endTime });
        }
        else
        {
            override_.EndTime = endTime;
        }

        await UpdateAsync(existing, cancellationToken);

        await CacheService.SetAsync(EventCacheKeys.CurrentEvent(), existing, EventCacheExpiration, cancellationToken);

        return existing;
    }
}
