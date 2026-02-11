using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using Tsa.Submissions.Coding.WebApi.Configuration;
using Tsa.Submissions.Coding.WebApi.Entities;

namespace Tsa.Submissions.Coding.WebApi.Services;

public class ProgrammingLanguagesService : MongoDbService<ProgrammingLanguage>, IProgrammingLanguagesService
{
    public const string MongoDbCollectionName = "programming_languages";

    public string CollectionName => MongoDbCollectionName;

    public string ServiceName => "ProgrammingLanguages";

    public ProgrammingLanguagesService(IMongoClient mongoClient, IOptions<SubmissionsDatabase> options, ILogger<ProgrammingLanguagesService> logger) :
        base(
            mongoClient,
            options.Value.Name!,
            MongoDbCollectionName,
            logger) { }
}
