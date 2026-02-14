using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using Tsa.Submissions.Coding.WebApi.Configuration;
using Tsa.Submissions.Coding.WebApi.Entities;

namespace Tsa.Submissions.Coding.WebApi.Services;

/// <summary>
///     Provides data access and caching operations for test case entities.
///     All read operations are cached for 2 hours to optimize performance during competitions.
/// </summary>
/// <remarks>
///     This service implements multi-dimensional caching:
///     - By test case ID for individual lookups
///     - By signature for duplicate detection
///     - By problem ID for problem-scoped queries
///     Cache invalidation occurs automatically on create, update, and delete operations.
///     The 2-hour cache duration aligns with the typical competition time window.
/// </remarks>
public class TestCasesService : MongoDbService<TestCase>, ITestCasesService
{
    public const string MongoDbCollectionName = "problem_test_cases";

    private const string TestCaseBySignatureCacheKey = "test_case_signature";
    private const string TestCaseIdCacheKey = "test_case_id";
    private const string TestCasesByProblemIdCacheKey = "test_cases_problem_id";
    private const string TestCasesCacheKey = "test_cases";

    /// <summary>
    ///     Gets the name of the MongoDB collection for test cases.
    /// </summary>
    public string CollectionName => MongoDbCollectionName;

    /// <summary>
    ///     Gets the service name for diagnostic and logging purposes.
    /// </summary>
    public string ServiceName => "TestCases";

    /// <summary>
    ///     Initializes a new instance of the <see cref="TestCasesService" /> class.
    /// </summary>
    /// <param name="cacheService">The cache service for storing and retrieving cached data</param>
    /// <param name="mongoClient">The MongoDB client instance</param>
    /// <param name="options">The database configuration options</param>
    /// <param name="logger">The logger instance</param>
    public TestCasesService(
        ICacheService cacheService,
        IMongoClient mongoClient,
        IOptions<SubmissionsDatabase> options,
        ILogger<TestCasesService> logger)
        : base(
            cacheService,
            mongoClient,
            options.Value.Name!,
            MongoDbCollectionName,
            logger) { }

    /// <summary>
    ///     Creates a new test case in the database and populates the cache.
    /// </summary>
    /// <param name="entity">The test case entity to create</param>
    /// <param name="cancellationToken">The cancellation token</param>
    /// <returns>A task representing the asynchronous operation</returns>
    /// <remarks>
    ///     This method caches the test case by ID and signature for fast subsequent lookups
    ///     and invalidates problem-scoped and collection caches to maintain consistency.
    /// </remarks>
    public override async Task CreateAsync(TestCase entity, CancellationToken cancellationToken = default)
    {
        await base.CreateAsync(entity, cancellationToken);

        await SetCacheAsync($"{TestCaseIdCacheKey}:{entity.Id}", entity, cancellationToken);
        await SetCacheAsync($"{TestCaseBySignatureCacheKey}:{entity.Signature}", entity, cancellationToken);

        // Null forgiveness is safe here because a test case must have a problem ID to be valid, and the database enforces this constraint.
        await InvalidateTestCasesCacheAsync(entity.ProblemId!, cancellationToken);

        await InvalidateTestCasesCacheAsync(cancellationToken);
    }

    /// <summary>
    ///     Gets all test cases from the database. Results are cached for 2 hours.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token</param>
    /// <returns>A list of all test cases</returns>
    public override async Task<List<TestCase>> GetAsync(CancellationToken cancellationToken = default)
    {
        return await GetOrSetCacheAsync(
            TestCasesCacheKey,
            async ct => await base.GetAsync(ct),
            cancellationToken
        );
    }

    /// <summary>
    ///     Gets a test case by its unique identifier. Results are cached for 2 hours.
    /// </summary>
    /// <param name="id">The test case's unique identifier</param>
    /// <param name="cancellationToken">The cancellation token</param>
    /// <returns>The test case if found; otherwise, null</returns>
    public override async Task<TestCase?> GetAsync(string id, CancellationToken cancellationToken = default)
    {
        return await GetOrSetCacheAsync(
            $"{TestCaseIdCacheKey}:{id}",
            async ct => await base.GetAsync(id, ct),
            cancellationToken
        );
    }

    /// <summary>
    ///     Gets all test cases for a specific problem. Results are cached for 2 hours.
    /// </summary>
    /// <param name="problem">The problem whose test cases should be retrieved</param>
    /// <param name="cancellationToken">The cancellation token</param>
    /// <returns>A list of test cases associated with the problem</returns>
    public async Task<List<TestCase>> GetByProblemAsync(Problem problem, CancellationToken cancellationToken = default)
    {
        return await GetOrSetCacheAsync(
            $"{TestCasesByProblemIdCacheKey}:{problem.Id}",
            async ct => await GetByProblemFromDbAsync(problem, ct),
            cancellationToken);
    }

    /// <summary>
    ///     Retrieves test cases for a problem directly from the database, bypassing the cache.
    /// </summary>
    /// <param name="problem">The problem whose test cases should be retrieved</param>
    /// <param name="cancellationToken">The cancellation token</param>
    /// <returns>A list of test cases associated with the problem</returns>
    private async Task<List<TestCase>> GetByProblemFromDbAsync(Problem problem, CancellationToken cancellationToken = default)
    {
        var filterDefinition = Builders<TestCase>.Filter.Eq(testCase => testCase.ProblemId, problem.Id);

        var cursor = await EntityCollection.FindAsync(filterDefinition, cancellationToken: cancellationToken);

        return await cursor.ToListAsync(cancellationToken);
    }

    /// <summary>
    ///     Gets a test case by its signature hash. Results are cached for 2 hours.
    /// </summary>
    /// <param name="problem">The problem whose test case should be retrieved</param>
    /// <param name="signature">The signature hash to search for</param>
    /// <param name="cancellationToken">The cancellation token</param>
    /// <returns>The test case with the matching signature, or null if not found</returns>
    /// <remarks>
    ///     This method is used for duplicate detection during test case creation and updates.
    ///     It leverages caching to provide fast lookups for recently created or accessed test cases.
    /// </remarks>
    public async Task<TestCase?> GetBySignatureAsync(Problem problem, string signature, CancellationToken cancellationToken = default)
    {
        return await GetOrSetCacheAsync(
            $"{TestCaseBySignatureCacheKey}:{problem.Id}:{signature}",
            async ct => await GetBySignatureFromDbAsync(problem, signature, ct),
            cancellationToken);
    }

    /// <summary>
    ///     Retrieves a test case by signature directly from the database, bypassing the cache.
    /// </summary>
    /// <param name="problem">The problem whose test case should be retrieved</param>
    /// <param name="signature">The signature hash to search for</param>
    /// <param name="cancellationToken">The cancellation token</param>
    /// <returns>The test case with the matching signature, or null if not found</returns>
    private async Task<TestCase?> GetBySignatureFromDbAsync(Problem problem, string signature, CancellationToken cancellationToken = default)
    {
        var filterDefinition = Builders<TestCase>.Filter.And(
            Builders<TestCase>.Filter.Eq(testCase => testCase.ProblemId, problem.Id),
            Builders<TestCase>.Filter.Eq(testCase => testCase.Signature, signature)
        );

        var cursor = await EntityCollection.FindAsync(filterDefinition, cancellationToken: cancellationToken);

        return await cursor.SingleOrDefaultAsync(cancellationToken);
    }

    /// <summary>
    ///     Invalidates all cache entries for a specific test case, including ID, signature, problem scope, and collection
    ///     caches.
    /// </summary>
    /// <param name="testCase">The test case whose cache entries should be invalidated</param>
    /// <param name="cancellationToken">The cancellation token</param>
    /// <returns>A task representing the asynchronous operation</returns>
    private async Task InvalidateTestCaseCacheAsync(TestCase testCase, CancellationToken cancellationToken)
    {
        await CacheService.RemoveAsync($"{TestCaseIdCacheKey}:{testCase.Id}", cancellationToken);
        await CacheService.RemoveAsync($"{TestCaseBySignatureCacheKey}:{testCase.Signature}", cancellationToken);

        // Null forgiveness is safe here because a test case must have a problem ID to be valid, and the database enforces this constraint.
        await InvalidateTestCasesCacheAsync(testCase.ProblemId!, cancellationToken);

        await InvalidateTestCasesCacheAsync(cancellationToken);
    }

    /// <summary>
    ///     Invalidates the cached test cases for a specific problem.
    /// </summary>
    /// <param name="problemId">The ID of the problem whose test cases cache should be invalidated</param>
    /// <param name="cancellationToken">The cancellation token</param>
    /// <returns>A task representing the asynchronous operation</returns>
    private async Task InvalidateTestCasesCacheAsync(string problemId, CancellationToken cancellationToken)
    {
        await CacheService.RemoveAsync($"{TestCasesByProblemIdCacheKey}:{problemId}", cancellationToken);
    }

    /// <summary>
    ///     Invalidates the cached collection of all test cases.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token</param>
    /// <returns>A task representing the asynchronous operation</returns>
    private async Task InvalidateTestCasesCacheAsync(CancellationToken cancellationToken)
    {
        await CacheService.RemoveAsync(TestCasesCacheKey, cancellationToken);
    }

    /// <summary>
    ///     Removes a test case from the database and invalidates all related cache entries.
    /// </summary>
    /// <param name="entity">The test case entity to remove</param>
    /// <param name="cancellationToken">The cancellation token</param>
    /// <returns>A task representing the asynchronous operation</returns>
    public override async Task RemoveAsync(TestCase entity, CancellationToken cancellationToken = default)
    {
        await base.RemoveAsync(entity, cancellationToken);

        await InvalidateTestCaseCacheAsync(entity, cancellationToken);
    }

    /// <summary>
    ///     Checks whether a test case with the specified signature exists.
    /// </summary>
    /// <param name="problem">The problem whose test case should be retrieved</param>
    /// <param name="signature">The signature hash to search for</param>
    /// <param name="cancellationToken">The cancellation token</param>
    /// <returns>True if a test case with the signature exists; otherwise, false</returns>
    /// <remarks>
    ///     This method checks the cache first for performance, then falls back to the database if needed.
    ///     Used for duplicate detection during test case creation and updates.
    /// </remarks>
    public async Task<bool> SignatureExistsAsync(Problem problem, string signature, CancellationToken cancellationToken = default)
    {
        var cachedTestCase = await CacheService.GetAsync<TestCase>($"{TestCaseBySignatureCacheKey}:{problem.Id}:{signature}", cancellationToken);

        if (cachedTestCase != null) return true;

        var testCase = await GetBySignatureAsync(problem, signature, cancellationToken);

        return testCase != null;
    }

    /// <summary>
    ///     Updates a test case in the database and refreshes its cache entries.
    /// </summary>
    /// <param name="entity">The test case entity with updated values</param>
    /// <param name="cancellationToken">The cancellation token</param>
    /// <returns>A task representing the asynchronous operation</returns>
    /// <remarks>
    ///     This method updates the individual test case cache entries (by ID and signature)
    ///     and invalidates problem-scoped and collection caches to ensure consistency.
    /// </remarks>
    public override async Task UpdateAsync(TestCase entity, CancellationToken cancellationToken = default)
    {
        await base.UpdateAsync(entity, cancellationToken);

        await SetCacheAsync($"{TestCaseIdCacheKey}:{entity.Id}", entity, cancellationToken);
        await SetCacheAsync($"{TestCaseBySignatureCacheKey}:{entity.Signature}", entity, cancellationToken);

        // Null forgiveness is safe here because a test case must have a problem ID to be valid, and the database enforces this constraint.
        await InvalidateTestCasesCacheAsync(entity.ProblemId!, cancellationToken);

        await InvalidateTestCasesCacheAsync(cancellationToken);
    }
}
