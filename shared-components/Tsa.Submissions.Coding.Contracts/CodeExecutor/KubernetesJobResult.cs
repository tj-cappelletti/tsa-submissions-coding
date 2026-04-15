namespace Tsa.Submissions.Coding.Contracts.CodeExecutor;

/// <summary>
///     Represents the outcome of a Kubernetes job execution (infrastructure-level result)
/// </summary>
public record KubernetesJobResult
{
    /// <summary>
    ///     Gets the Kubernetes job identifier
    /// </summary>
    public string JobId { get; init; }

    /// <summary>
    ///     Gets the Kubernetes job name
    /// </summary>
    public string JobName { get; init; }

    /// <summary>
    ///     Gets the pod name that executed the job
    /// </summary>
    public string PodName { get; init; }

    /// <summary>
    ///     Gets the total execution time of the Kubernetes job
    /// </summary>
    public TimeSpan ExecutionTime { get; init; }

    /// <summary>
    ///     Gets whether the Kubernetes job was successfully scheduled
    /// </summary>
    public bool JobScheduledSuccessfully { get; init; }

    /// <summary>
    ///     Gets whether the Kubernetes job completed successfully (pod exited with code 0)
    /// </summary>
    public bool JobCompletedSuccessfully { get; init; }

    /// <summary>
    ///     Gets whether the Kubernetes job failed (scheduling or execution failure)
    /// </summary>
    public bool JobFailed => !JobScheduledSuccessfully || !JobCompletedSuccessfully;

    /// <summary>
    ///     Gets the raw logs from the pod
    /// </summary>
    public string Logs { get; init; }

    public KubernetesJobResult(
        string jobId,
        string jobName,
        string podName,
        TimeSpan executionTime,
        bool jobScheduledSuccessfully,
        bool jobCompletedSuccessfully,
        string logs)
    {
        JobId = jobId;
        JobName = jobName;
        PodName = podName;
        ExecutionTime = executionTime;
        JobScheduledSuccessfully = jobScheduledSuccessfully;
        JobCompletedSuccessfully = jobCompletedSuccessfully;
        Logs = logs;
    }
}
