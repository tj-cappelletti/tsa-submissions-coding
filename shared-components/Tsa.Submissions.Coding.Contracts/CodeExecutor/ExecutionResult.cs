using System.Text.Json.Serialization;

namespace Tsa.Submissions.Coding.Contracts.CodeExecutor;

/// <summary>
///     Base class for execution results (business-level outcome)
/// </summary>
public abstract record ExecutionResult
{
    /// <summary>
    ///     Gets or sets any error message from the execution process
    /// </summary>
    [JsonPropertyName("errorMessage")]
    public string ErrorMessage { get; init; }

    /// <summary>
    ///     Gets whether the execution failed
    /// </summary>
    [JsonPropertyName("isFailure")]
    public bool IsFailure => !IsSuccess;

    /// <summary>
    ///     Gets or sets whether the execution was successful
    /// </summary>
    [JsonPropertyName("isSuccess")]
    public bool IsSuccess { get; init; }

    /// <summary>
    ///     Gets or sets the standard error output captured from the executed process.
    /// </summary>
    [JsonPropertyName("standardError")]
    public string StandardError { get; init; }

    /// <summary>
    ///     Gets or sets the standard output captured from the executed process.
    /// </summary>
    [JsonPropertyName("standardOutput")]
    public string StandardOutput { get; init; }

    protected ExecutionResult(string errorMessage, string standardError, string standardOutput, bool isSuccess)
    {
        ErrorMessage = errorMessage;
        StandardError = standardError;
        StandardOutput = standardOutput;
        IsSuccess = isSuccess;
    }

    /// <summary>
    ///     Protected constructor for creating failed execution results from exceptions
    /// </summary>
    protected ExecutionResult(Exception exception)
        : this(exception.Message, exception.StackTrace ?? string.Empty, string.Empty, false)
    {
    }
}
