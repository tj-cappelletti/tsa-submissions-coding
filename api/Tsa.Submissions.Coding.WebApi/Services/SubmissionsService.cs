using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using Tsa.Submissions.Coding.WebApi.Configuration;
using Tsa.Submissions.Coding.WebApi.Entities;

namespace Tsa.Submissions.Coding.WebApi.Services;

public class SubmissionsService : MongoDbService<Submission>, ISubmissionsService
{
    public const string MongoDbCollectionName = "submissions";

    public string CollectionName => MongoDbCollectionName;

    public string ServiceName => "Submissions";

    public SubmissionsService(IMongoClient mongoClient, IOptions<SubmissionsDatabase> options, ILogger<SubmissionsService> logger) : base(
        mongoClient,
        options.Value.Name!,
        MongoDbCollectionName,
        logger)
    { }

    public async Task<List<Submission>> GetByProblemIdAsync(string problemId, CancellationToken cancellationToken = default)
    {
        var filterDefinition = Builders<Submission>.Filter.Eq(submission => submission.Problem!.Id, problemId);

        var cursor = await EntityCollection.FindAsync(filterDefinition, null, cancellationToken);

        var result = await cursor.ToListAsync(cancellationToken);

        return result;
    }
}