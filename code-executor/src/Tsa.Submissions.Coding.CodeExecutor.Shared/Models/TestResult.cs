namespace Tsa.Submissions.Coding.CodeExecutor.Shared.Models;

/// <summary>
/// Result of a single test case execution
/// </summary>
public class TestResult
{
    /// <summary>
    /// Gets or sets the test case identifier
    /// </summary>
    public required string TestCaseId { get; set; }

    /// <summary>
    /// Gets or sets whether the test passed
    /// </summary>
    public bool Passed { get; set; }

    /// <summary>
    /// Gets or sets the actual output from the code
    /// </summary>
    public string? ActualOutput { get; set; }

    /// <summary>
    /// Gets or sets the expected output
    /// </summary>
    public string? ExpectedOutput { get; set; }

    /// <summary>
    /// Gets or sets any error message from the test execution
    /// </summary>
    public string? Error { get; set; }

    /// <summary>
    /// Gets or sets the execution time in milliseconds
    /// </summary>
    public long ExecutionTimeMs { get; set; }
}
