namespace Tsa.Submissions.Coding.CodeExecutor.Shared.Models;

/// <summary>
/// Payload containing all information needed to execute a submission
/// </summary>
public class ExecutionPayload
{
    /// <summary>
    /// Gets or sets the unique identifier for the submission
    /// </summary>
    public required string SubmissionId { get; set; }

    /// <summary>
    /// Gets or sets the programming language
    /// </summary>
    public required string Language { get; set; }

    /// <summary>
    /// Gets or sets the source code to execute
    /// </summary>
    public required string SourceCode { get; set; }

    /// <summary>
    /// Gets or sets the test cases to run
    /// </summary>
    public required List<TestCaseInput> TestCases { get; set; }
}
