using Tsa.Submissions.Coding.WebApi.Entities;

namespace Tsa.Submissions.Coding.WebApi.Services.Cache;

public sealed class ProgrammingLanguageCacheKeys : CacheKeyBuilder
{
    private const string ProgrammingLanguageIdPrefix = "programming_language_id";
    private const string ProgrammingLanguagesCollectionKey = "programming_languages";

    /// <summary>
    ///     Gets the cache key for the collection of all programming languages.
    /// </summary>
    public static string AllProgrammingLanguages()
    {
        return ProgrammingLanguagesCollectionKey;
    }

    /// <summary>
    ///     Builds a cache key for a specific programming language by its ID.
    /// </summary>
    public static string ById(string programmingLanguageId)
    {
        return BuildKey(ProgrammingLanguageIdPrefix, programmingLanguageId);
    }

    /// <summary>
    ///     Builds a cache key for a programming language entity.
    /// </summary>
    public static string ForEntity(ProgrammingLanguage programmingLanguage)
    {
        return ById(programmingLanguage.Id!);
    }
}
