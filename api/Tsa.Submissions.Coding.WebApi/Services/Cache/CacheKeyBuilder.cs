using System;

namespace Tsa.Submissions.Coding.WebApi.Services.Cache;

/// <summary>
///     Provides a base class for building consistent cache keys with validation.
/// </summary>
public abstract class CacheKeyBuilder
{
    /// <summary>
    ///     Builds a cache key from the specified segments, ensuring none are null or whitespace.
    /// </summary>
    /// <param name="segments">The key segments to combine</param>
    /// <returns>A formatted cache key</returns>
    /// <exception cref="ArgumentException">Thrown when any segment is null or whitespace</exception>
    protected static string BuildKey(params string?[] segments)
    {
        for (var index = 0; index < segments.Length; index++)
        {
            if (string.IsNullOrWhiteSpace(segments[index]))
            {
                throw new ArgumentException($"Cache key segment at index {index} cannot be null or whitespace.", nameof(segments));
            }
        }

        return string.Join(":", segments);
    }
}
