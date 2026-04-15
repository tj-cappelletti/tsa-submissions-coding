using System;
using System.Text.Json.Serialization;

namespace Tsa.Submissions.Coding.Contracts.Events;

public record EventUserOverrideRequest
{
    [JsonPropertyName("endTime")]
    public DateTimeOffset EndTime { get; init; }
}
