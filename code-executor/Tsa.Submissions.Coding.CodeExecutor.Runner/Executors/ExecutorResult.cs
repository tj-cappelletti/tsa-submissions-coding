using System.Diagnostics;

namespace Tsa.Submissions.Coding.CodeExecutor.Runner.Executors;

/// <summary>
///     Represents the result of a language executor operation, providing standardized
///     information about process execution status, outputs, and errors.
/// </summary>
/// <remarks>
///     This record is used by <see cref="ILanguageExecutor" /> implementations to return
///     consistent execution results from operations like <see cref="ILanguageExecutor.Prepare" />
///     and <see cref="ILanguageExecutor.ExecuteTests" />.
/// </remarks>
public record ExecutorResult
{
    /// <summary>
    ///     Gets or sets a human-readable error message describing any failure that occurred during execution.
    /// </summary>
    /// <remarks>
    ///     This is typically set when <see cref="IsSuccess" /> is <c>false</c> to provide context about the failure.
    /// </remarks>
    public string? ErrorMessage { get; set; }

    /// <summary>
    ///     Gets or sets the exit code returned by the executed process.
    /// </summary>
    /// <remarks>
    ///     By convention, an exit code of 0 indicates success. Non-zero values indicate various error conditions.
    ///     A value of <see cref="int.MinValue" /> indicates the process did not execute (e.g., due to an exception).
    /// </remarks>
    public int ExitCode { get; set; }

    /// <summary>
    ///     Gets whether the execution failed.
    /// </summary>
    /// <value>
    ///     <c>true</c> if the execution was unsuccessful; otherwise, <c>false</c>.
    /// </value>
    public bool IsFailure => !IsSuccess;

    /// <summary>
    ///     Gets or sets whether the execution completed successfully.
    /// </summary>
    /// <value>
    ///     <c>true</c> if the execution was successful; otherwise, <c>false</c>.
    /// </value>
    public bool IsSuccess { get; set; }

    /// <summary>
    ///     Gets or sets the standard error (stderr) output captured from the executed process.
    /// </summary>
    /// <remarks>
    ///     Contains error messages, warnings, and diagnostic information written to stderr by the process.
    /// </remarks>
    public string StandardError { get; set; } = string.Empty;

    /// <summary>
    ///     Gets or sets the standard output (stdout) captured from the executed process.
    /// </summary>
    /// <remarks>
    ///     Contains normal output written to stdout by the process.
    /// </remarks>
    public string StandardOutput { get; set; } = string.Empty;

    /// <summary>
    ///     Creates an <see cref="ExecutorResult" /> representing a failed execution due to an exception.
    /// </summary>
    /// <param name="exception">The exception that caused the failure.</param>
    /// <returns>
    ///     An <see cref="ExecutorResult" /> with <see cref="IsSuccess" /> set to <c>false</c>,
    ///     <see cref="ExitCode" /> set to <see cref="int.MinValue" />, and error details populated
    ///     from the exception.
    /// </returns>
    public static ExecutorResult FromException(Exception exception)
    {
        return new ExecutorResult
        {
            ErrorMessage = exception.Message,
            ExitCode = int.MinValue,
            StandardError = exception.StackTrace ?? string.Empty,
            StandardOutput = string.Empty,
            IsSuccess = false
        };
    }

    /// <summary>
    ///     Creates an <see cref="ExecutorResult" /> from a completed <see cref="Process" />.
    /// </summary>
    /// <param name="process">The process that has finished executing.</param>
    /// <param name="errorMessage">Optional custom error message to include in the result.</param>
    /// <param name="expectedErrorCodes">Optional array of exit codes that should be considered successful.</param>
    /// <returns>
    ///     An <see cref="ExecutorResult" /> with execution details populated from the process.
    ///     <see cref="IsSuccess" /> is set to <c>true</c> if the process exit code is 0.
    /// </returns>
    /// <remarks>
    ///     This method reads from <see cref="Process.StandardOutput" /> and <see cref="Process.StandardError" />,
    ///     which must have been redirected when the process was started.
    /// </remarks>
    public static ExecutorResult FromProcess(Process process, string? errorMessage = null, int[]? expectedErrorCodes = null)
    {
        var exitCode = process.ExitCode;

        var isSuccess = exitCode == 0 || (expectedErrorCodes != null && expectedErrorCodes.Contains(exitCode));

        return new ExecutorResult
        {
            ErrorMessage = errorMessage ?? (exitCode != 0 ? $"Process exited with code {exitCode}" : null),
            ExitCode = exitCode,
            StandardError = process.StandardError.ReadToEnd(),
            StandardOutput = process.StandardOutput.ReadToEnd(),
            IsSuccess = isSuccess
        };
    }
}
