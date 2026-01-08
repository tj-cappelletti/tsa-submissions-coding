namespace Tsa.Submissions.Coding.CodeExecutor.Shared.Models;

/// <summary>
/// Input data for a single test case
/// </summary>
public class TestCaseInput
{
    /// <summary>
    /// Gets or sets the test case identifier
    /// </summary>
    public required string TestCaseId { get; set; }

    /// <summary>
    /// Gets or sets the input data for the test
    /// </summary>
    public required string Input { get; set; }

    /// <summary>
    /// Gets or sets the expected output
    /// </summary>
    public required string ExpectedOutput { get; set; }

    /// <summary>
    /// Gets or sets the timeout in milliseconds (default: 30000)
    /// </summary>
    public int TimeoutMs { get; set; } = 30000;
}
