using Tsa.Submissions.Coding.Contracts.CodeExecutor;

namespace Tsa.Submissions.Coding.CodeExecutor.Core.Strategies;

/// <summary>
///     Defines a strategy for executing a specific type of runner job
/// </summary>
/// <typeparam name="TResult">The type of execution result this strategy produces</typeparam>
public interface IExecutionStrategy<out TResult> where TResult : ExecutionResult
{
    /// <summary>
    ///     Gets the name of the strategy.
    /// </summary>
    /// <remarks>
    ///     The strategy name is typically used for logging, diagnostics, and identifying the strategy in a collection of
    ///     strategies.
    /// </remarks>
    string StrategyName { get; }

    /// <summary>
    ///     Executes the strategy with the provided payload
    /// </summary>
    /// <param name="payload">The job payload containing execution parameters</param>
    /// <returns>The execution result</returns>
    TResult Execute(RunnerJobPayload payload);
}
