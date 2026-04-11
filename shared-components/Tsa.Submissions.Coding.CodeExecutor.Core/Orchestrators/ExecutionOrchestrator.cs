using System.Text.Json;
using Tsa.Submissions.Coding.CodeExecutor.Core.Strategies;
using Tsa.Submissions.Coding.Contracts.CodeExecutor;

namespace Tsa.Submissions.Coding.CodeExecutor.Core.Orchestrators;

/// <summary>
///     Orchestrates the execution of runner jobs with a pluggable execution strategy
/// </summary>
public static class ExecutionOrchestrator
{
    /// <summary>
    ///     Runs the execution strategy with standardized environment variable handling and output
    /// </summary>
    /// <typeparam name="TResult">The type of execution result</typeparam>
    /// <param name="strategy">The execution strategy to run</param>
    /// <returns>Exit code (0 for success, -1 for failure)</returns>
    public static int Run<TResult>(IExecutionStrategy<TResult> strategy)
        where TResult : ExecutionResult
    {
        Console.WriteLine("Runner starting up...");

        var executionPayloadJson = Environment.GetEnvironmentVariable("EXECUTION_PAYLOAD");
        var outputToken = Environment.GetEnvironmentVariable("OUTPUT_TOKEN") ?? "###CODE-EXECUTION-RESULT###";

        if (string.IsNullOrEmpty(executionPayloadJson))
        {
            Console.Error.WriteLine("ERROR: EXECUTION_PAYLOAD environment variable not set");
            return -1;
        }

        var runnerJobPayload = JsonSerializer.Deserialize<RunnerJobPayload>(executionPayloadJson);

        if (runnerJobPayload == null)
        {
            Console.Error.WriteLine("ERROR: Failed to deserialize EXECUTION_PAYLOAD");
            return -1;
        }

        Console.WriteLine($"Executing {strategy.StrategyName} for submission {runnerJobPayload.SubmissionId}...");

        var executionResult = strategy.Execute(runnerJobPayload);

        var executionResultJson = JsonSerializer.Serialize(executionResult);
        Console.WriteLine($"Runner job for submission {runnerJobPayload.SubmissionId} execution completed.");
        Console.WriteLine(outputToken);
        Console.WriteLine(executionResultJson);

        return executionResult.IsSuccess ? 0 : -1;
    }
}
