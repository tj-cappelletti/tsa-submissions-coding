namespace Tsa.Submissions.Coding.Contracts.CodeExecutor;

public class RunnerJobResult
{
    /// <summary>
    ///     Gets the total execution time of the job.
    /// </summary>
    public TimeSpan ExecutionTime { get; init; }

    /// <summary>
    ///     Indicates whether the job completed successfully.
    /// </summary>
    public bool JobCompletedSuccessfully { get; init; }

    /// <summary>
    ///     Indicates whether the job failed or not.
    /// </summary>
    public bool JobFailed => !JobSuccessfullyScheduled || !JobCompletedSuccessfully;

    /// <summary>
    ///     Gets the job identifier.
    /// </summary>
    public string JobId { get; init; }

    /// <summary>
    ///     Gets the name of the job.
    /// </summary>
    public string JobName { get; init; }

    /// <summary>
    ///     Indicates whether the job was scheduled successfully.
    /// </summary>
    public bool JobSuccessfullyScheduled { get; init; }

    /// <summary>
    ///     Gets the log content as a string.
    /// </summary>
    public string Logs { get; init; }

    /// <summary>
    ///     Gets the name of the pod the that ran the job.
    /// </summary>
    public string PodName { get; init; }

    public RunnerJobResult(
        string jobId,
        string jobName,
        string podName,
        bool jobSuccessfullyScheduled,
        bool jobCompletedSuccessfully,
        TimeSpan executionTime,
        string logs)
    {
        ExecutionTime = executionTime;
        JobCompletedSuccessfully = jobCompletedSuccessfully;
        JobId = jobId;
        JobName = jobName;
        JobSuccessfullyScheduled = jobSuccessfullyScheduled;
        Logs = logs;
        PodName = podName;
    }
}
