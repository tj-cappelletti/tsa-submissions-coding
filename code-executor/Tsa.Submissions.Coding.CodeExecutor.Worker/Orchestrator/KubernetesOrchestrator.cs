using System.Diagnostics;
using k8s;
using k8s.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Tsa.Submissions.Coding.CodeExecutor.Worker.Configuration;
using Tsa.Submissions.Coding.CodeExecutor.Worker.Strategies;
using Tsa.Submissions.Coding.Contracts.CodeExecutor;

namespace Tsa.Submissions.Coding.CodeExecutor.Worker.Orchestrator;

/// <summary>
///     Orchestrates the execution of Kubernetes jobs using pluggable job strategies
/// </summary>
public class KubernetesOrchestrator
{
    private readonly int _jobTimeoutMinutes;
    private readonly IKubernetes _kubernetesClient;
    private readonly ILogger<KubernetesOrchestrator> _logger;
    private readonly string _namespace;

    public KubernetesOrchestrator(
        IOptions<KubernetesCluster> kubernetesCluster,
        IKubernetes kubernetesClient,
        ILogger<KubernetesOrchestrator> logger)
    {
        _jobTimeoutMinutes = kubernetesCluster.Value.JobTimeoutMinutes;
        _kubernetesClient = kubernetesClient;
        _logger = logger;
        _namespace = kubernetesCluster.Value.Namespace ??
                     throw new ArgumentNullException(nameof(kubernetesCluster.Value.Namespace), "Kubernetes namespace cannot be null");
    }

    /// <summary>
    ///     Executes a Kubernetes job using the provided strategy with idempotent behavior
    /// </summary>
    /// <param name="kubernetesJobStrategy">The job strategy defining the job configuration and behavior</param>
    /// <param name="payload">The execution payload</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The job execution result</returns>
    public async Task<TResult> ExecuteJobAsync<TResult>(
        IKubernetesJobStrategy<TResult> kubernetesJobStrategy,
        RunnerJobPayload payload,
        CancellationToken cancellationToken = default) where TResult : ExecutionResult
    {
        var jobName = $"code-executor-{kubernetesJobStrategy.JobType}-job-{payload.SubmissionId}";
        _logger.LogInformation("Starting job execution for {JobName} (submission {SubmissionId})", jobName, payload.SubmissionId);

        try
        {
            // Check if job already exists
            var existingJobResult = await CheckExistingJobAsync(jobName, payload.SubmissionId, cancellationToken);

            if (existingJobResult != null)
            {
                _logger.LogInformation("Found existing job {JobName}, processing its result", jobName);
                return kubernetesJobStrategy.ProcessResult(existingJobResult, cancellationToken);
            }

            // Create new job
            var image = kubernetesJobStrategy.GetContainerImage(payload);
            _logger.LogDebug("Using image {Image} for job {JobName}", image, jobName);

            var kubernetesJob = kubernetesJobStrategy.CreateJobDefinition(payload, jobName, _namespace);

            await _kubernetesClient.BatchV1.CreateNamespacedJobAsync(kubernetesJob, _namespace, cancellationToken: cancellationToken);
            _logger.LogInformation("Job {JobName} created successfully", jobName);

            // Wait for completion
            var jobResult = await WaitForJobCompletionAsync(jobName, payload.SubmissionId, cancellationToken);

            return kubernetesJobStrategy.ProcessResult(jobResult, cancellationToken);
        }
        catch (k8s.Autorest.HttpOperationException ex) when (ex.Response.StatusCode == System.Net.HttpStatusCode.Conflict)
        {
            // Job was created between our check and create attempt (race condition)
            _logger.LogWarning("Job {JobName} already exists (race condition detected), waiting for completion", jobName);

            var jobResult = await WaitForJobCompletionAsync(jobName, payload.SubmissionId, cancellationToken);
            return kubernetesJobStrategy.ProcessResult(jobResult, cancellationToken);
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "There was an unexpected error while trying to run the Kubernetes job {JobName}", jobName);
            throw;
        }
    }

    /// <summary>
    ///     Checks if a job already exists and returns its result if completed
    /// </summary>
    private async Task<KubernetesJobResult?> CheckExistingJobAsync(
        string jobName,
        string submissionId,
        CancellationToken cancellationToken)
    {
        try
        {
            var job = await _kubernetesClient.BatchV1.ReadNamespacedJobStatusAsync(
                jobName,
                _namespace,
                cancellationToken: cancellationToken);

            _logger.LogInformation("Found existing job {JobName} with status: Active={Active}, Succeeded={Succeeded}, Failed={Failed}",
                jobName, job.Status.Active, job.Status.Succeeded, job.Status.Failed);

            // Job exists and is still running
            if (job.Status.Active > 0)
            {
                _logger.LogInformation("Job {JobName} is still active, waiting for completion", jobName);
                return await WaitForJobCompletionAsync(jobName, submissionId, cancellationToken);
            }

            // Job exists and has completed (success or failure)
            if (job.Status.Succeeded > 0 || job.Status.Failed > 0)
            {
                var logs = await GetPodLogsAsync(jobName, cancellationToken);
                var executionTime = CalculateJobExecutionTime(job);

                var jobCompletedSuccessfully = job.Status.Succeeded > 0;

                _logger.LogInformation("Job {JobName} has already completed. Success: {Success}", jobName, jobCompletedSuccessfully);

                return new KubernetesJobResult(
                    job.Uid(),
                    jobName,
                    string.Empty, // Pod name not critical for completed jobs
                    executionTime,
                    true, // Job was scheduled
                    jobCompletedSuccessfully,
                    logs);
            }

            // Job exists but has no status (shouldn't happen, but handle gracefully)
            _logger.LogWarning("Job {JobName} exists but has no completion status, waiting for updates", jobName);
            return await WaitForJobCompletionAsync(jobName, submissionId, cancellationToken);
        }
        catch (k8s.Autorest.HttpOperationException ex) when (ex.Response.StatusCode == System.Net.HttpStatusCode.NotFound)
        {
            // Job doesn't exist - this is the normal case for new jobs
            _logger.LogDebug("Job {JobName} does not exist yet", jobName);
            return null;
        }
    }

    /// <summary>
    ///     Calculates the execution time of a completed job
    /// </summary>
    private static TimeSpan CalculateJobExecutionTime(V1Job job)
    {
        if (job.Status.StartTime == null)
        {
            return TimeSpan.Zero;
        }

        var endTime = job.Status.CompletionTime ?? DateTimeOffset.UtcNow;
        return endTime - job.Status.StartTime.Value;
    }

    private async Task<string> GetPodLogsAsync(string jobName, CancellationToken cancellationToken)
    {
        try
        {
            var pods = await _kubernetesClient.CoreV1.ListNamespacedPodAsync(
                _namespace,
                labelSelector: $"job-name={jobName}",
                cancellationToken: cancellationToken);

            if (pods.Items.Count == 0)
            {
                _logger.LogWarning("No pods found for job {JobName}", jobName);
                return string.Empty;
            }

            var pod = pods.Items[0];

            var logsStream = await _kubernetesClient.CoreV1.ReadNamespacedPodLogAsync(
                pod.Metadata.Name,
                _namespace,
                cancellationToken: cancellationToken);

            using var reader = new StreamReader(logsStream);

            var logs = await reader.ReadToEndAsync(cancellationToken);

            return logs;
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "Error getting pod logs for job {JobName}", jobName);
            return string.Empty;
        }
    }

    private async Task<KubernetesJobResult> WaitForJobCompletionAsync(
        string jobName,
        string submissionId,
        CancellationToken cancellationToken)
    {
        var timeout = TimeSpan.FromMinutes(_jobTimeoutMinutes);
        var stopwatch = Stopwatch.StartNew();
        var pollingInterval = TimeSpan.FromSeconds(2);

        _logger.LogInformation("Waiting for job {JobName} to complete (timeout: {Timeout} minutes)", jobName, _jobTimeoutMinutes);

        while (stopwatch.Elapsed < timeout)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                _logger.LogWarning("Job {JobName} monitoring cancelled", jobName);
                throw new OperationCanceledException("Job monitoring was cancelled", cancellationToken);
            }

            try
            {
                var job = await _kubernetesClient.BatchV1.ReadNamespacedJobStatusAsync(
                    jobName,
                    _namespace,
                    cancellationToken: cancellationToken);

                // Job succeeded
                if (job.Status.Succeeded > 0)
                {
                    var logs = await GetPodLogsAsync(jobName, cancellationToken);
                    _logger.LogInformation("Job {JobName} for submission {SubmissionId} completed successfully in {Duration}",
                        jobName, submissionId, stopwatch.Elapsed);

                    return new KubernetesJobResult(
                        job.Uid(),
                        jobName,
                        string.Empty,
                        stopwatch.Elapsed,
                        true,
                        true,
                        logs);
                }

                // Job failed
                if (job.Status.Failed > 0)
                {
                    var logs = await GetPodLogsAsync(jobName, cancellationToken);
                    _logger.LogError("Job {JobName} for submission {SubmissionId} failed after {Duration}",
                        jobName, submissionId, stopwatch.Elapsed);

                    return new KubernetesJobResult(
                        job.Uid(),
                        jobName,
                        string.Empty,
                        stopwatch.Elapsed,
                        true,
                        false,
                        logs);
                }

                // Job still running
                _logger.LogDebug("Job {JobName} still running (Active: {Active}), elapsed: {Elapsed}",
                    jobName, job.Status.Active, stopwatch.Elapsed);
            }
            catch (k8s.Autorest.HttpOperationException ex) when (ex.Response.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                // Job was deleted while we were waiting
                _logger.LogError("Job {JobName} was deleted while waiting for completion", jobName);
                throw new InvalidOperationException($"Job {jobName} was deleted during execution", ex);
            }

            await Task.Delay(pollingInterval, cancellationToken);
        }

        // Timeout reached
        var timeoutLogs = await GetPodLogsAsync(jobName, cancellationToken);
        _logger.LogError("Job {JobName} for submission {SubmissionId} timed out after {Timeout} minutes",
            jobName, submissionId, _jobTimeoutMinutes);

        return new KubernetesJobResult(
            string.Empty,
            jobName,
            string.Empty,
            timeout,
            true,
            false,
            timeoutLogs);
    }
}