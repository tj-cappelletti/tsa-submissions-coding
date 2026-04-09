using System.Text.Json.Serialization;

namespace Tsa.Submissions.Coding.Contracts.ProblemLanguageVariants;

public record CodeMetricsRequest
{
    [JsonPropertyName("cyclomaticComplexity")]
    public int CyclomaticComplexity { get; init; }

    [JsonPropertyName("executionTimeInMs")]
    public long ExecutionTimeInMs { get; init; }

    [JsonPropertyName("linesOfCode")]
    public int LinesOfCode { get; init; }

    public CodeMetricsRequest(int cyclomaticComplexity, long executionTimeInMs, int linesOfCode)
    {
        CyclomaticComplexity = cyclomaticComplexity;
        ExecutionTimeInMs = executionTimeInMs;
        LinesOfCode = linesOfCode;
    }
}
