using System.Diagnostics;
using System.Text.Json;
using k8s;
using k8s.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Tsa.Submissions.Coding.CodeExecutor.Worker.Configuration;
using Tsa.Submissions.Coding.Contracts.CodeExecutor;

namespace Tsa.Submissions.Coding.CodeExecutor.Worker.Services;

public class KubernetesJobManager
{
    private readonly int _jobTimeoutMinutes;
    private readonly IKubernetes _kubernetesClient;
    private readonly ILogger<KubernetesJobManager> _logger;
    private readonly string _namespace;
    private readonly RunnerImageRegistry _runnerImageRegistry;

    public KubernetesJobManager(IOptions<KubernetesCluster> kubernetesCluster, IOptions<RunnerImageRegistry> runnerImageRegistry,
        IKubernetes kubernetesClient,
        ILogger<KubernetesJobManager> logger)
    {
        _runnerImageRegistry = runnerImageRegistry.Value;

        _jobTimeoutMinutes = kubernetesCluster.Value.JobTimeoutMinutes;
        _kubernetesClient = kubernetesClient;
        _logger = logger;
        _namespace = kubernetesCluster.Value.Namespace ??
                     throw new ArgumentNullException(nameof(kubernetesCluster.Value.Namespace), "Kubernetes namespace cannot be null");
    }

    private V1Job CreateJobDefinition(RunnerJobPayload payload, string jobName, string image, string payloadJson)
    {
        return new V1Job
        {
            ApiVersion = "batch/v1",
            Kind = "Job",
            Metadata = new V1ObjectMeta
            {
                Name = jobName,
                NamespaceProperty = _namespace
                // TODO: Add labels and annotations as needed
            },
            Spec = new V1JobSpec
            {
                BackoffLimit = 0,
                TtlSecondsAfterFinished = 300,
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
                            new()
                            {
                                Name = "runner",
                                Image = image,
                                ImagePullPolicy = "IfNotPresent",
                                Env = new List<V1EnvVar>
                                {
                                    new()
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
    }

    public async Task ExecuteJobAsync(RunnerJobPayload payload, CancellationToken cancellationToken = default)
    {
        var jobName = $"code-executor-runner-job-{payload.SubmissionId}";

        try
        {
            _logger.LogInformation("Creating Kubernetes job {JobName} for submission {SubmissionId}", jobName, payload.SubmissionId);

            var image = GetImageForLanguage(payload.Language, payload.LanguageVersion);
            _logger.LogDebug("Using image {Image} for language {Language} {Version}", image, payload.Language, payload.LanguageVersion);

            var payloadJson = JsonSerializer.Serialize(payload);

            var kubernetesJob = CreateJobDefinition(payload, jobName, image, payloadJson);

            await _kubernetesClient.BatchV1.CreateNamespacedJobAsync(kubernetesJob, _namespace, cancellationToken: cancellationToken);

            _logger.LogInformation("Job {JobName} created successfully", jobName);

            var result = await WaitForJobCompletionAsync(jobName, payload.SubmissionId, cancellationToken);

            if (result.IsFailure)
                _logger.LogError("Job {JobName} for submission {SubmissionId} did not complete successfully", jobName, payload.SubmissionId);
        }
        catch (Exception exception)
        {
            Console.WriteLine(exception);
            throw;
        }
    }

    private string GetImageForLanguage(string language, string version)
    {
        var baseUrl = _runnerImageRegistry.RegistryUrl;
        var imageName = _runnerImageRegistry.ImageName;
        var imageVersion = _runnerImageRegistry.ImageVersion;

        var languageTag = language.ToLower() switch
        {
            "c" => _runnerImageRegistry.LanguageTags?.C,
            "cpp" or "c++" => _runnerImageRegistry.LanguageTags?.Cpp,
            "csharp" or "c#" => _runnerImageRegistry.LanguageTags?.CSharp,
            "fsharp" or "f#" => _runnerImageRegistry.LanguageTags?.FSharp,
            "go" or "golang" => _runnerImageRegistry.LanguageTags?.Go,
            "java" => _runnerImageRegistry.LanguageTags?.Java,
            "nodejs" or "node.js" or "javascript" or "typescript" => _runnerImageRegistry.LanguageTags?.NodeJs,
            "python" => _runnerImageRegistry.LanguageTags?.Python,
            "ruby" => _runnerImageRegistry.LanguageTags?.Ruby,
            "visualbasic" or "vb" or "vb.net" => _runnerImageRegistry.LanguageTags?.VisualBasic,
            _ => throw new NotSupportedException($"Programming language '{language}' is not supported.")
        };

        var fullyQualifiedImageName = $"{imageName}:{imageVersion}-{languageTag}{version}";

        return string.IsNullOrEmpty(baseUrl) ? fullyQualifiedImageName : $"{baseUrl}/{fullyQualifiedImageName}";
    }

    private async Task<RunnerJobResult> WaitForJobCompletionAsync(string jobName, string submissionId, CancellationToken cancellationToken)
    {
        var timeout = TimeSpan.FromMinutes(_jobTimeoutMinutes);
        var stopwatch = Stopwatch.StartNew();

        while (stopwatch.Elapsed < timeout)
        {
            if (cancellationToken.IsCancellationRequested) break;

            var job = await _kubernetesClient.BatchV1.ReadNamespacedJobStatusAsync(jobName, _namespace, cancellationToken: cancellationToken);

            if (job.Status.Succeeded > 0)
            {
                _logger.LogInformation("Job {JobName} for submission {SubmissionId} completed successfully", jobName, submissionId);
                return new RunnerJobResult
                {
                    IsSuccessful = true
                };
            }

            if (job.Status.Failed > 0)
            {
                _logger.LogError("Job {JobName} for submission {SubmissionId} failed", jobName, submissionId);
                return new RunnerJobResult
                {
                    IsSuccessful = false
                };
            }

            await Task.Delay(TimeSpan.FromSeconds(2), cancellationToken);
        }

        _logger.LogError("Job {JobName} for submission {SubmissionId} timed out after {Timeout} minutes", jobName, submissionId, _jobTimeoutMinutes);
        return new RunnerJobResult
        {
            IsSuccessful = false
        };
    }
}
