namespace Tsa.Submissions.Coding.CodeExecutor.Shared.Models;

/// <summary>
/// Result of code execution containing test results
/// </summary>
public class ExecutionResult
{
    /// <summary>
    /// Gets or sets the submission identifier
    /// </summary>
    public required string SubmissionId { get; set; }

    /// <summary>
    /// Gets or sets whether the execution was successful
    /// </summary>
    public bool Success { get; set; }

    /// <summary>
    /// Gets or sets any error message from the execution process
    /// </summary>
    public string? ErrorMessage { get; set; }

    /// <summary>
    /// Gets or sets the test results
    /// </summary>
    public List<TestResult> TestResults { get; set; } = [];
}
