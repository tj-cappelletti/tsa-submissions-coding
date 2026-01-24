using Tsa.Submissions.Coding.Contracts.TestCases;

namespace Tsa.Submissions.Coding.CodeExecutor.Runner.Executors;

/// <summary>
///     Context information for code execution
/// </summary>
public class CodeExecutionContext
{
    /// <summary>
    ///     Gets or sets the path to the prepared executable/script
    /// </summary>
    public string? ExecutablePath { get; set; }

    /// <summary>
    ///     Gets or sets the language being executed
    /// </summary>
    public required string Language { get; set; }

    /// <summary>
    ///     The version of the language to use
    /// </summary>
    public required string LanguageVersion { get; set; }

    /// <summary>
    ///     Gets or sets the source code to execute
    /// </summary>
    public required string SourceCode { get; set; }

    /// <summary>
    ///     Gets or sets the test cases to execute
    /// </summary>
    public required List<TestCase> TestCases { get; set; } = [];

    /// <summary>
    ///     Gets or sets the working directory for execution
    /// </summary>
    public required string WorkingDirectory { get; set; }
}
