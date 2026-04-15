using Tsa.Submissions.Coding.Contracts.CodeExecutor;

namespace Tsa.Submissions.Coding.CodeExecutor.Worker.Handlers;

/// <summary>
///     Handles side effects for execution results (API updates, notifications, etc.)
/// </summary>
public interface IResultHandler<in TResult> where TResult : ExecutionResult
{
    /// <summary>
    ///     Processes the execution result and performs necessary side effects
    /// </summary>
    Task HandleAsync(
        RunnerJobPayload payload,
        TResult result,
        CancellationToken cancellationToken = default);
}
