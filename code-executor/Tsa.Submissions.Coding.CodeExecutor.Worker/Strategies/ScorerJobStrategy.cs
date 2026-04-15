using k8s.Models;
using System.Text.Json;
using Tsa.Submissions.Coding.CodeExecutor.Worker.Configuration;
using Tsa.Submissions.Coding.Contracts.CodeExecutor;

namespace Tsa.Submissions.Coding.CodeExecutor.Worker.Strategies;

public class ScorerJobStrategy : IKubernetesJobStrategy<ScorerResult>
{
    private readonly ScorerImageRegistry _scorerImageRegistry;

    public string JobType => "scorer";

    public ScorerJobStrategy(ScorerImageRegistry scorerImageRegistry)
    {
        _scorerImageRegistry = scorerImageRegistry;
    }

    public V1Job CreateJobDefinition(RunnerJobPayload payload, string jobName, string @namespace)
    {
        var payloadJson = JsonSerializer.Serialize(payload);

        return new V1Job
        {
            ApiVersion = "batch/v1",
            Kind = "Job",
            Metadata = new V1ObjectMeta
            {
                Name = jobName,
                NamespaceProperty = @namespace
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
                                Name = "scorer",
                                Image = GetContainerImage(payload),
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
                                        { "memory", new ResourceQuantity("256Mi") },
                                        { "cpu", new ResourceQuantity("250m") }
                                    },
                                    Requests = new Dictionary<string, ResourceQuantity>
                                    {
                                        { "memory", new ResourceQuantity("128Mi") },
                                        { "cpu", new ResourceQuantity("125m") }
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

    public string GetContainerImage(RunnerJobPayload payload)
    {
        var registryUri = _scorerImageRegistry.RegistryUri;
        var imageName = _scorerImageRegistry.ImageName;
        var imageVersion = _scorerImageRegistry.ImageVersion;

        var fullyQualifiedImageName = $"{imageName}:{imageVersion}";

        return string.IsNullOrEmpty(registryUri)
            ? fullyQualifiedImageName
            : $"{registryUri}/{fullyQualifiedImageName}";
    }

    public ScorerResult ProcessResult(KubernetesJobResult result, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}
