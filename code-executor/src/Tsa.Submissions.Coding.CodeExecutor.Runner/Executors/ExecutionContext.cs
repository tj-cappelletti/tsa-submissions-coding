namespace Tsa.Submissions.Coding.CodeExecutor.Runner.Executors;

/// <summary>
/// Context information for code execution
/// </summary>
public class ExecutionContext
{
    /// <summary>
    /// Gets or sets the source code to execute
    /// </summary>
    public required string SourceCode { get; set; }

    /// <summary>
    /// Gets or sets the working directory for execution
    /// </summary>
    public required string WorkingDirectory { get; set; }

    /// <summary>
    /// Gets or sets the path to the prepared executable/script
    /// </summary>
    public string? ExecutablePath { get; set; }

    /// <summary>
    /// Gets or sets the language being executed
    /// </summary>
    public required string Language { get; set; }
}
