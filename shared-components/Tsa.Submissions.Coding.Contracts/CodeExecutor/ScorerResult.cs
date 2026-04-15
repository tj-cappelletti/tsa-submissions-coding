using System.Text.Json.Serialization;

namespace Tsa.Submissions.Coding.Contracts.CodeExecutor;

/// <summary>
///     Result of code quality scoring execution
/// </summary>
public record ScorerResult : ExecutionResult
{
    /// <summary>
    ///     Gets the cyclomatic complexity score
    /// </summary>
    [JsonPropertyName("cyclomaticComplexity")]
    public int CyclomaticComplexity { get; init; }

    /// <summary>
    ///     Gets the lines of code count
    /// </summary>
    [JsonPropertyName("linesOfCode")]
    public int LinesOfCode { get; init; }

    /// <summary>
    ///     Creates a ScorerResult representing failed execution
    /// </summary>
    public ScorerResult(
        string errorMessage,
        string standardError,
        string standardOutput)
        : base(errorMessage, standardError, standardOutput, false)
    {
        CyclomaticComplexity = 0;
        LinesOfCode = 0;
    }

    /// <summary>
    ///     Creates a ScorerResult representing successful execution with metrics
    /// </summary>
    public ScorerResult(
        string standardOutput,
        int cyclomaticComplexity,
        int linesOfCode)
        : base(string.Empty, string.Empty, standardOutput, true)
    {
        CyclomaticComplexity = cyclomaticComplexity;
        LinesOfCode = linesOfCode;
    }

    /// <summary>
    ///     Creates a ScorerResult from an exception
    /// </summary>
    public ScorerResult(Exception exception)
        : base(exception)
    {
        CyclomaticComplexity = 0;
        LinesOfCode = 0;
    }

    public static ScorerResult FromException(Exception exception) => new(exception);
}
