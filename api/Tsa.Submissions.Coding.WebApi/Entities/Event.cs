using System;
using System.Collections.Generic;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Tsa.Submissions.Coding.WebApi.Entities;

public class Event : IMongoDbEntity
{
    public DateTimeOffset? EndTime { get; set; }

    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; }

    public bool IsActive { get; set; }

    public DateTimeOffset? StartTime { get; set; }

    public List<EventUserOverride> UserOverrides { get; set; } = [];

    /// <summary>
    ///     Returns true if the event is currently active for the given user,
    ///     taking per-user end time overrides into account.
    /// </summary>
    /// <param name="userId">The ID of the user to check</param>
    /// <returns>True if the event is running for the user; otherwise, false</returns>
    public bool IsActiveForUser(string userId)
    {
        if (!IsActive) return false;

        var now = DateTimeOffset.UtcNow;

        var userOverride = UserOverrides.Find(o => o.UserId == userId);
        var effectiveEndTime = userOverride?.EndTime ?? EndTime;

        return effectiveEndTime.HasValue && now < effectiveEndTime.Value;
    }
}
