namespace Tsa.Submissions.Coding.WebApi.Services.Cache;

/// <summary>
///     Provides methods for building consistent cache keys for event operations.
/// </summary>
public sealed class EventCacheKeys : CacheKeyBuilder
{
    private const string EventKey = "event:current";

    /// <summary>
    ///     Gets the cache key for the current event.
    /// </summary>
    public static string CurrentEvent()
    {
        return EventKey;
    }
}
