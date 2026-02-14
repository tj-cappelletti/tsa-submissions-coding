using Tsa.Submissions.Coding.WebApi.Entities;

namespace Tsa.Submissions.Coding.WebApi.Services.Cache;

/// <summary>
///     Provides methods for building consistent cache keys for test case operations.
/// </summary>
public sealed class TestCaseCacheKeys : CacheKeyBuilder
{
    private const string TestCaseIdPrefix = "test_case_id";
    private const string TestCasesByProblemIdPrefix = "test_cases_problem_id";
    private const string TestCasesCollectionKey = "test_cases";
    private const string TestCaseSignaturePrefix = "test_case_signature";

    /// <summary>
    ///     Gets the cache key for the collection of all test cases.
    /// </summary>
    /// <returns>The cache key for all test cases</returns>
    public static string AllTestCases()
    {
        return TestCasesCollectionKey;
    }

    /// <summary>
    ///     Builds a cache key for a specific test case by its ID.
    /// </summary>
    /// <param name="testCaseId">The unique identifier of the test case</param>
    /// <returns>A cache key for the test case</returns>
    public static string ById(string testCaseId)
    {
        return BuildKey(TestCaseIdPrefix, testCaseId);
    }

    /// <summary>
    ///     Builds a cache key for all test cases associated with a problem.
    /// </summary>
    /// <param name="problemId">The unique identifier of the problem</param>
    /// <returns>A cache key for the problem's test cases</returns>
    public static string ByProblemId(string problemId)
    {
        return BuildKey(TestCasesByProblemIdPrefix, problemId);
    }

    /// <summary>
    ///     Builds a cache key for a test case by its problem ID and signature.
    /// </summary>
    /// <param name="problemId">The unique identifier of the problem</param>
    /// <param name="signature">The signature hash of the test case</param>
    /// <returns>A cache key for the test case signature</returns>
    public static string BySignature(string problemId, string signature)
    {
        return BuildKey(TestCaseSignaturePrefix, problemId, signature);
    }

    /// <summary>
    ///     Builds a cache key for a specific test case by its ID using the entity.
    /// </summary>
    /// <param name="testCase">The test case entity</param>
    /// <returns>A cache key for the test case</returns>
    public static string ForEntity(TestCase testCase)
    {
        return ById(testCase.Id!);
    }

    /// <summary>
    ///     Builds a cache key for all test cases associated with the problem of a test case entity.
    /// </summary>
    /// <param name="testCase">The test case entity</param>
    /// <returns>A cache key for the problem's test cases</returns>
    public static string ForEntityProblem(TestCase testCase)
    {
        return ByProblemId(testCase.ProblemId!);
    }

    /// <summary>
    ///     Builds a cache key for a test case signature using the entity.
    /// </summary>
    /// <param name="testCase">The test case entity</param>
    /// <returns>A cache key for the test case signature</returns>
    public static string ForEntitySignature(TestCase testCase)
    {
        return BySignature(testCase.ProblemId!, testCase.Signature!);
    }
}
