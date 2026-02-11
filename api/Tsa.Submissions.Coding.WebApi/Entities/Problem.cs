using System.Collections.Generic;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Tsa.Submissions.Coding.WebApi.Entities;

public class Problem : IMongoDbEntity
{
    public string? Description { get; set; }

    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; }

    public bool IsActive { get; set; }
    
    public string? Title { get; set; }
}
