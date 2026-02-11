using System.Collections.Generic;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Tsa.Submissions.Coding.WebApi.Entities;

public class User : IMongoDbEntity
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; }

    public List<Participant>? Participants { get; set; }

    public string? PasswordHash { get; set; }

    public string? Role { get; set; }

    public Team? Team { get; set; }

    public string? UserName { get; set; }
}
