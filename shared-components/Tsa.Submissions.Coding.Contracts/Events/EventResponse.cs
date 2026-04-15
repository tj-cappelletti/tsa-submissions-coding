using System;
using System.Text.Json.Serialization;

namespace Tsa.Submissions.Coding.Contracts.Events;

public record EventResponse
{
    [JsonPropertyName("endTime")]
    public DateTimeOffset? EndTime { get; init; }

    [JsonPropertyName("isActive")]
    public bool IsActive { get; init; }

    [JsonPropertyName("startTime")]
    public DateTimeOffset? StartTime { get; init; }

    public EventResponse(bool isActive, DateTimeOffset? startTime, DateTimeOffset? endTime)
    {
        EndTime = endTime;
        IsActive = isActive;
        StartTime = startTime;
    }
}
