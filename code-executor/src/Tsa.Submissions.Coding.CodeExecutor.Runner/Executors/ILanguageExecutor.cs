namespace Tsa.Submissions.Coding.CodeExecutor.Runner.Executors;

/// <summary>
/// Interface for language-specific code execution
/// </summary>
public interface ILanguageExecutor
{
    /// <summary>
    /// Prepares the source code for execution (e.g., compilation)
    /// </summary>
    /// <param name="context">The execution context</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Task representing the preparation operation</returns>
    Task PrepareAsync(ExecutionContext context, CancellationToken cancellationToken = default);

    /// <summary>
    /// Executes the code with the given input
    /// </summary>
    /// <param name="context">The execution context</param>
    /// <param name="input">The input data</param>
    /// <param name="timeoutMs">Timeout in milliseconds</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Tuple containing stdout, stderr, and exit code</returns>
    Task<(string stdout, string stderr, int exitCode)> ExecuteAsync(
        ExecutionContext context,
        string input,
        int timeoutMs,
        CancellationToken cancellationToken = default);
}
