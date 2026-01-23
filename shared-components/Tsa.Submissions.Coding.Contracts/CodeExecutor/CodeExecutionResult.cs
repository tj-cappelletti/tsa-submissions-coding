using Tsa.Submissions.Coding.Contracts.TestCases;

namespace Tsa.Submissions.Coding.Contracts.CodeExecutor;

/// <summary>
///     Result of code execution containing test results
/// </summary>
public class CodeExecutionResult
{
    /// <summary>
    ///     Gets or sets any error message from the execution process
    /// </summary>
    public string? ErrorMessage { get; set; }

    /// <summary>
    ///     Gets or sets the submission identifier
    /// </summary>
    public required string SubmissionId { get; set; }

    /// <summary>
    ///     Gets or sets whether the execution was successful
    /// </summary>
    public bool Success { get; set; }

    /// <summary>
    ///     Gets or sets the test case results
    /// </summary>
    public List<TestCaseResult> TestCaseResults { get; set; } = [];
}
