using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using Tsa.Submissions.Coding.WebApi.Configuration;
using Tsa.Submissions.Coding.WebApi.Entities;

namespace Tsa.Submissions.Coding.WebApi.Services;

public class TestSetsService : MongoDbService<TestSet>, ITestSetsService
{
    public const string MongoDbCollectionName = "test_sets";

    public string CollectionName => MongoDbCollectionName;

    public string ServiceName => "TestSets";

    public TestSetsService(IMongoClient mongoClient, IOptions<SubmissionsDatabase> options, ILogger<TestSetsService> logger) : base(
        mongoClient,
        options.Value.Name!,
        MongoDbCollectionName,
        logger)
    { }

    public async Task<List<TestSet>> GetAsync(Problem problem, CancellationToken cancellationToken = default)
    {
        var filterDefinition = Builders<TestSet>.Filter.Eq(_ => _.Problem!.Id, problem.Id);

        var cursor = await EntityCollection.FindAsync(filterDefinition, null, cancellationToken);

        var result = await cursor.ToListAsync(cancellationToken);

        return result;
    }
}
