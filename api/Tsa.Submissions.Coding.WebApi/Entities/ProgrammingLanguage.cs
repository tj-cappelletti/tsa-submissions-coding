using System.Collections.Generic;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Tsa.Submissions.Coding.WebApi.Entities;

public class ProgrammingLanguage : IMongoDbEntity
{
    public string? FileExtension { get; set; }

    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; }

    public string? Identifier { get; set; }

    public bool IsEnabled { get; set; } = true;

    public string? Name { get; set; }

    public List<ProgrammingLanguageVersion> Versions { get; set; } = [];
}
