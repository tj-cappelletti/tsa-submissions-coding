using k8s.Models;
using Tsa.Submissions.Coding.Contracts.CodeExecutor;

namespace Tsa.Submissions.Coding.CodeExecutor.Worker.Strategies;

/// <summary>
///     Defines a strategy for creating and configuring Kubernetes jobs
/// </summary>
public interface IKubernetesJobStrategy<out TResult> where TResult : ExecutionResult
{
    /// <summary>
    ///     Gets the job type identifier used in naming and logging
    /// </summary>
    string JobType { get; }

    /// <summary>
    ///     Creates the Kubernetes job definition for this strategy
    /// </summary>
    /// <param name="payload">The execution payload</param>
    /// <param name="jobName">The generated job name</param>
    /// <param name="namespace">The Kubernetes namespace</param>
    /// <returns>A configured V1Job definition</returns>
    V1Job CreateJobDefinition(
        RunnerJobPayload payload,
        string jobName,
        string @namespace);

    /// <summary>
    ///     Gets the container image for the given strategy
    /// </summary>
    /// <param name="payload">The execution payload</param>
    /// <returns>The fully qualified image name</returns>
    string GetContainerImage(RunnerJobPayload payload);

    /// <summary>
    ///     Processes the job result and transforms it into the appropriate execution result type for this strategy
    /// </summary>
    /// <param name="result">The result from the Kubernetes job</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The processed result</returns>
    TResult ProcessResult(KubernetesJobResult result, CancellationToken cancellationToken = default);
}
