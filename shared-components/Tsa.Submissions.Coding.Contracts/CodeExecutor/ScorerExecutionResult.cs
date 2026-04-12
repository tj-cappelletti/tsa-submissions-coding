namespace Tsa.Submissions.Coding.Contracts.CodeExecutor;

public record ScorerExecutionResult : ExecutionResult
{
    public double CyclomaticComplexity { get; }

    public double LinesOfCode { get; }

    public ScorerExecutionResult(
        string errorMessage,
        string standardError,
        string standardOutput,
        bool isSuccess,
        double cyclomaticComplexity,
        double linesOfCode) : base(errorMessage, standardError,
        standardOutput, isSuccess)
    {
        CyclomaticComplexity = cyclomaticComplexity;
        LinesOfCode = linesOfCode;
    }

    public ScorerExecutionResult(Exception exception) : base(exception)
    {
        CyclomaticComplexity = 0;
        LinesOfCode = 0;
    }

    public static ScorerExecutionResult FromException(Exception exception)
    {
        return new ScorerExecutionResult(exception);
    }
}
