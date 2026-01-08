using System.Text;
using System.Text.Json;
using k8s;
using k8s.Models;
using Tsa.Submissions.Coding.CodeExecutor.Shared.Models;

namespace Tsa.Submissions.Coding.CodeExecutor.Worker.Services;

/// <summary>
/// Manages Kubernetes Jobs for code execution
/// </summary>
public class KubernetesJobManager
{
    private readonly IKubernetes _kubernetesClient;
    private readonly IConfiguration _configuration;
    private readonly ILogger<KubernetesJobManager> _logger;
    private readonly string _namespace;
    private readonly int _jobTimeoutMinutes;

    /// <summary>
    /// Initializes a new instance of the <see cref="KubernetesJobManager"/> class
    /// </summary>
    public KubernetesJobManager(IKubernetes kubernetesClient, IConfiguration configuration, ILogger<KubernetesJobManager> logger)
    {
        _kubernetesClient = kubernetesClient;
        _configuration = configuration;
        _logger = logger;
        _namespace = configuration["Kubernetes:Namespace"] ?? "code-execution";
        _jobTimeoutMinutes = int.Parse(configuration["Kubernetes:JobTimeoutMinutes"] ?? "3");
    }

    /// <summary>
    /// Creates and runs a Kubernetes Job for code execution
    /// </summary>
    /// <param name="payload">The execution payload</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Execution result from the job</returns>
    public async Task<ExecutionResult?> ExecuteJobAsync(ExecutionPayload payload, CancellationToken cancellationToken = default)
    {
        var jobName = $"exec-{payload.SubmissionId.ToLower().Replace("_", "-")}-{DateTimeOffset.UtcNow.ToUnixTimeSeconds()}";
        
        try
        {
            _logger.LogInformation("Creating Kubernetes job {JobName} for submission {SubmissionId}", jobName, payload.SubmissionId);

            // Get the image for the language
            var image = GetImageForLanguage(payload.Language);
            
            // Serialize payload to JSON for environment variable
            var payloadJson = JsonSerializer.Serialize(payload);

            // Create job specification
            var job = new V1Job
            {
                ApiVersion = "batch/v1",
                Kind = "Job",
                Metadata = new V1ObjectMeta
                {
                    Name = jobName,
                    NamespaceProperty = _namespace,
                    Labels = new Dictionary<string, string>
                    {
                        { "app", "code-executor" },
                        { "submission-id", payload.SubmissionId }
                    }
                },
                Spec = new V1JobSpec
                {
                    BackoffLimit = 0,
                    TtlSecondsAfterFinished = 300, // Auto-cleanup after 5 minutes
                    Template = new V1PodTemplateSpec
                    {
                        Metadata = new V1ObjectMeta
                        {
                            Labels = new Dictionary<string, string>
                            {
                                { "app", "code-executor" },
                                { "submission-id", payload.SubmissionId }
                            }
                        },
                        Spec = new V1PodSpec
                        {
                            RestartPolicy = "Never",
                            Containers = new List<V1Container>
                            {
                                new V1Container
                                {
                                    Name = "runner",
                                    Image = image,
                                    ImagePullPolicy = "IfNotPresent",
                                    Env = new List<V1EnvVar>
                                    {
                                        new V1EnvVar
                                        {
                                            Name = "EXECUTION_PAYLOAD",
                                            Value = payloadJson
                                        }
                                    },
                                    Resources = new V1ResourceRequirements
                                    {
                                        Limits = new Dictionary<string, ResourceQuantity>
                                        {
                                            { "memory", new ResourceQuantity("512Mi") },
                                            { "cpu", new ResourceQuantity("500m") }
                                        },
                                        Requests = new Dictionary<string, ResourceQuantity>
                                        {
                                            { "memory", new ResourceQuantity("256Mi") },
                                            { "cpu", new ResourceQuantity("250m") }
                                        }
                                    },
                                    SecurityContext = new V1SecurityContext
                                    {
                                        RunAsNonRoot = true,
                                        RunAsUser = 1000,
                                        AllowPrivilegeEscalation = false,
                                        Capabilities = new V1Capabilities
                                        {
                                            Drop = new List<string> { "ALL" }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            };

            // Create the job
            await _kubernetesClient.BatchV1.CreateNamespacedJobAsync(job, _namespace, cancellationToken: cancellationToken);
            
            _logger.LogInformation("Job {JobName} created successfully", jobName);

            // Wait for job completion
            var result = await WaitForJobCompletionAsync(jobName, payload.SubmissionId, cancellationToken);

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error executing job {JobName}", jobName);
            return new ExecutionResult
            {
                SubmissionId = payload.SubmissionId,
                Success = false,
                ErrorMessage = $"Job execution failed: {ex.Message}"
            };
        }
        finally
        {
            // Cleanup job (TTL will also handle this, but we can try to delete immediately)
            try
            {
                await _kubernetesClient.BatchV1.DeleteNamespacedJobAsync(
                    jobName,
                    _namespace,
                    propagationPolicy: "Background",
                    cancellationToken: cancellationToken);
            }
            catch
            {
                // Ignore cleanup errors
            }
        }
    }

    private async Task<ExecutionResult?> WaitForJobCompletionAsync(string jobName, string submissionId, CancellationToken cancellationToken)
    {
        var timeout = TimeSpan.FromMinutes(_jobTimeoutMinutes);
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();

        while (stopwatch.Elapsed < timeout)
        {
            if (cancellationToken.IsCancellationRequested)
                break;

            try
            {
                var job = await _kubernetesClient.BatchV1.ReadNamespacedJobStatusAsync(jobName, _namespace, cancellationToken: cancellationToken);

                // Check if job completed successfully
                if (job.Status.Succeeded > 0)
                {
                    _logger.LogInformation("Job {JobName} completed successfully", jobName);
                    return await GetJobResultsAsync(jobName, submissionId, cancellationToken);
                }

                // Check if job failed
                if (job.Status.Failed > 0)
                {
                    _logger.LogError("Job {JobName} failed", jobName);
                    var logs = await GetPodLogsAsync(jobName, cancellationToken);
                    return new ExecutionResult
                    {
                        SubmissionId = submissionId,
                        Success = false,
                        ErrorMessage = $"Job failed. Logs: {logs}"
                    };
                }

                // Wait before checking again
                await Task.Delay(TimeSpan.FromSeconds(2), cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking job status for {JobName}", jobName);
                await Task.Delay(TimeSpan.FromSeconds(2), cancellationToken);
            }
        }

        _logger.LogError("Job {JobName} timed out after {Timeout} minutes", jobName, _jobTimeoutMinutes);
        return new ExecutionResult
        {
            SubmissionId = submissionId,
            Success = false,
            ErrorMessage = "Job execution timed out"
        };
    }

    private async Task<ExecutionResult?> GetJobResultsAsync(string jobName, string submissionId, CancellationToken cancellationToken)
    {
        try
        {
            var logs = await GetPodLogsAsync(jobName, cancellationToken);
            
            if (string.IsNullOrWhiteSpace(logs))
            {
                _logger.LogError("No logs found for job {JobName}", jobName);
                return new ExecutionResult
                {
                    SubmissionId = submissionId,
                    Success = false,
                    ErrorMessage = "No output from job"
                };
            }

            // Parse JSON result from logs
            var result = JsonSerializer.Deserialize<ExecutionResult>(logs);
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error parsing job results for {JobName}", jobName);
            return new ExecutionResult
            {
                SubmissionId = submissionId,
                Success = false,
                ErrorMessage = $"Failed to parse job results: {ex.Message}"
            };
        }
    }

    private async Task<string> GetPodLogsAsync(string jobName, CancellationToken cancellationToken)
    {
        try
        {
            // Find the pod for this job
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
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting pod logs for job {JobName}", jobName);
            return string.Empty;
        }
    }

    private string GetImageForLanguage(string language)
    {
        var baseUrl = _configuration["ImageRegistry:BaseUrl"];
        var imageName = _configuration[$"ImageRegistry:Images:{language}"];
        
        if (string.IsNullOrEmpty(imageName))
        {
            throw new InvalidOperationException($"No image configured for language: {language}");
        }

        return string.IsNullOrEmpty(baseUrl) ? imageName : $"{baseUrl}/{imageName}";
    }
}
