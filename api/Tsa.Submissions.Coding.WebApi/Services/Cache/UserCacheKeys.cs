using Tsa.Submissions.Coding.WebApi.Entities;

namespace Tsa.Submissions.Coding.WebApi.Services.Cache;

/// <summary>
///     Provides methods for building consistent cache keys for user operations.
/// </summary>
public sealed class UserCacheKeys : CacheKeyBuilder
{
    private const string UserIdPrefix = "user_id";
    private const string UserNamePrefix = "user_name";
    private const string UsersCollectionKey = "users";

    /// <summary>
    ///     Gets the cache key for the collection of all users.
    /// </summary>
    public static string AllUsers()
    {
        return UsersCollectionKey;
    }

    /// <summary>
    ///     Builds a cache key for a specific user by their ID.
    /// </summary>
    public static string ById(string userId)
    {
        return BuildKey(UserIdPrefix, userId);
    }

    /// <summary>
    ///     Builds a cache key for a user by their username.
    /// </summary>
    public static string ByUserName(string userName)
    {
        return BuildKey(UserNamePrefix, userName);
    }

    /// <summary>
    ///     Builds a cache key for a user entity by ID.
    /// </summary>
    public static string ForEntity(User user)
    {
        return ById(user.Id!);
    }

    /// <summary>
    ///     Builds a cache key for a user entity by username.
    /// </summary>
    public static string ForEntityUserName(User user)
    {
        return ByUserName(user.UserName!);
    }
}
