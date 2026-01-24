namespace Tsa.Submissions.Coding.CodeExecutor.Runner.Executors;

/// <summary>
///     Interface for language-specific code execution
/// </summary>
public interface ILanguageExecutor
{
    /// <summary>
    ///     Executes the code with the given input
    /// </summary>
    /// <param name="context">The execution context</param>
    /// <param name="input">The input data</param>
    /// <param name="timeout">The time span before timeout</param>
    /// <returns>Tuple containing stdout, stderr, and exit code</returns>
    (string stdout, string stderr, int exitCode) Execute(
        CodeExecutionContext context,
        string input,
        TimeSpan timeout);

    /// <summary>
    ///     Prepares the source code for execution (e.g., compilation)
    /// </summary>
    /// <param name="context">The execution context</param>
    /// <returns>Task representing the preparation operation</returns>
    void Prepare(CodeExecutionContext context);
}
