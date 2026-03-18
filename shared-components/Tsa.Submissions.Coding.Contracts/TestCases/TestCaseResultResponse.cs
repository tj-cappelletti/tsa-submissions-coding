using System.Text.Json.Serialization;

namespace Tsa.Submissions.Coding.Contracts.TestCases;

public record TestCaseResultResponse
{
    [JsonPropertyName("actualOutput")]
    public string ActualOutput { get; init; }

    [JsonPropertyName("executionTime")]
    public TimeSpan ExecutionTime { get; init; } = TimeSpan.Zero;

    [JsonPropertyName("message")]
    public string? Message { get; init; }

    [JsonPropertyName("passed")]
    public bool Passed { get; init; }

    [JsonPropertyName("testCaseId")]
    public string TestCaseId { get; init; }

    [JsonPropertyName("timedOut")]
    public bool TimedOut { get; init; }

    public TestCaseResultResponse(
        string testCaseId,
        string actualOutput,
        string? message,
        bool passed,
        bool timedOut,
        TimeSpan executionTime)
    {
        ActualOutput = actualOutput;
        ExecutionTime = executionTime;
        Message = message;
        Passed = passed;
        TestCaseId = testCaseId;
        TimedOut = timedOut;
    }
}
