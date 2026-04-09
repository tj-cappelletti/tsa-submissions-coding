using Tsa.Submissions.Coding.WebApi.Entities;

namespace Tsa.Submissions.Coding.WebApi.Services.Cache;

/// <summary>
///     Provides methods for building consistent cache keys for problem language variant operations.
/// </summary>
public sealed class ProblemLanguageVariantCacheKeys : CacheKeyBuilder
{
    private const string VariantByProblemLanguageVersionPrefix = "problem_language_variant_plv";
    private const string VariantIdPrefix = "problem_language_variant_id";
    private const string VariantsByProblemIdPrefix = "problem_language_variants_problem_id";
    private const string VariantsCollectionKey = "problem_language_variants";

    /// <summary>
    ///     Gets the cache key for the collection of all problem language variants.
    /// </summary>
    public static string AllVariants()
    {
        return VariantsCollectionKey;
    }

    /// <summary>
    ///     Builds a cache key for a specific variant by its ID.
    /// </summary>
    /// <param name="variantId">The unique identifier of the variant</param>
    /// <returns>A cache key for the variant</returns>
    public static string ById(string variantId)
    {
        return BuildKey(VariantIdPrefix, variantId);
    }

    /// <summary>
    ///     Builds a cache key for all variants associated with a problem.
    /// </summary>
    /// <param name="problemId">The unique identifier of the problem</param>
    /// <returns>A cache key for the problem's variants</returns>
    public static string ByProblemId(string problemId)
    {
        return BuildKey(VariantsByProblemIdPrefix, problemId);
    }

    /// <summary>
    ///     Builds a cache key for a variant by its unique composite key
    ///     of problem ID, programming language ID, and version tag.
    /// </summary>
    /// <param name="problemId">The unique identifier of the problem</param>
    /// <param name="programmingLanguageId">The unique identifier of the programming language</param>
    /// <param name="versionTag">The programming language version tag</param>
    /// <returns>A cache key for the variant</returns>
    public static string ByProblemLanguageAndVersion(string problemId, string programmingLanguageId, string versionTag)
    {
        return BuildKey(VariantByProblemLanguageVersionPrefix, problemId, programmingLanguageId, versionTag);
    }

    /// <summary>
    ///     Builds a cache key for a specific variant entity by its ID.
    /// </summary>
    /// <param name="variant">The problem language variant entity</param>
    /// <returns>A cache key for the variant</returns>
    public static string ForEntity(ProblemLanguageVariant variant)
    {
        return ById(variant.Id!);
    }

    /// <summary>
    ///     Builds a cache key for all variants associated with the problem of a variant entity.
    /// </summary>
    /// <param name="variant">The problem language variant entity</param>
    /// <returns>A cache key for the problem's variants</returns>
    public static string ForEntityProblem(ProblemLanguageVariant variant)
    {
        return ByProblemId(variant.ProblemId!);
    }

    /// <summary>
    ///     Builds a cache key for a variant entity by its composite key.
    /// </summary>
    /// <param name="variant">The problem language variant entity</param>
    /// <returns>A cache key for the variant's composite key</returns>
    public static string ForEntityProblemLanguageAndVersion(ProblemLanguageVariant variant)
    {
        return ByProblemLanguageAndVersion(variant.ProblemId!, variant.ProgrammingLanguageId!, variant.ProgrammingLanguageVersionTag!);
    }
}
