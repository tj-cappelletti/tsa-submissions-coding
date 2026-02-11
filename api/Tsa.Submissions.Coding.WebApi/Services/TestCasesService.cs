using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using Tsa.Submissions.Coding.WebApi.Configuration;
using Tsa.Submissions.Coding.WebApi.Entities;

namespace Tsa.Submissions.Coding.WebApi.Services;

public class TestCasesService : MongoDbService<TestCase>, ITestCasesService
{
    public const string MongoDbCollectionName = "problem_test_cases";

    public string CollectionName => MongoDbCollectionName;

    public string ServiceName => "TestCases";

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

    public async Task<List<TestCase>> GetByProblemAsync(Problem problem, CancellationToken cancellationToken = default)
    {
        var filterDefinition = Builders<TestCase>.Filter.Eq(testCase => testCase.ProblemId, problem.Id);

        var cursor = await EntityCollection.FindAsync(filterDefinition, cancellationToken: cancellationToken);

        var results = await cursor.ToListAsync(cancellationToken);

        return results;
    }

    public async Task<TestCase?> GetBySignatureAsync(string signature, CancellationToken cancellationToken = default)
    {
        var filterDefinition = Builders<TestCase>.Filter.Eq(testCase => testCase.Signature, signature);

        var cursor = await EntityCollection.FindAsync(filterDefinition, cancellationToken: cancellationToken);

        var result = await cursor.SingleOrDefaultAsync(cancellationToken);

        return result;
    }

    public async Task<bool> SignatureExistsAsync(string signature, CancellationToken cancellationToken = default)
    {
        var filterDefinition = Builders<TestCase>.Filter.Eq(testCase => testCase.Signature, signature);

        var cursor = await EntityCollection.FindAsync(filterDefinition, cancellationToken: cancellationToken);

        return await cursor.AnyAsync(cancellationToken);
    }
}
