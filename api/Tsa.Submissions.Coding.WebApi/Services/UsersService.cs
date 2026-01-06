using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using Tsa.Submissions.Coding.WebApi.Configuration;
using Tsa.Submissions.Coding.WebApi.Entities;

namespace Tsa.Submissions.Coding.WebApi.Services;

public sealed class UsersService : MongoDbService<User>, IUsersService
{
    public const string MongoDbCollectionName = "users";

    public string CollectionName => MongoDbCollectionName;

    public string ServiceName => "Users";

    public UsersService(IMongoClient mongoClient, IOptions<SubmissionsDatabase> options, ILogger<UsersService> logger) : base(
        mongoClient,
        options.Value.Name!,
        MongoDbCollectionName,
        logger)
    { }

    public async Task<User?> GetByUserNameAsync(string? userName, CancellationToken cancellationToken = default)
    {
        var filterDefinition = Builders<User>.Filter.Eq(user => user.UserName, userName);

        var cursor = await EntityCollection.FindAsync(filterDefinition, cancellationToken: cancellationToken);

        var result = await cursor.SingleOrDefaultAsync(cancellationToken);

        return result;
    }
}
