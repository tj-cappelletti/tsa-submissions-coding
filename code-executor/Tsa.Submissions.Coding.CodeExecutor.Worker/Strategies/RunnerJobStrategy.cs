using System.Text.Json;
using k8s.Models;
using Tsa.Submissions.Coding.CodeExecutor.Worker.Configuration;
using Tsa.Submissions.Coding.Contracts.CodeExecutor;

namespace Tsa.Submissions.Coding.CodeExecutor.Worker.Strategies;

public class RunnerJobStrategy : IKubernetesJobStrategy<RunnerResult>
{
    private readonly RunnerImageRegistry _runnerImageRegistry;

    public string JobType => "runner";

    public RunnerJobStrategy(RunnerImageRegistry runnerImageRegistry)
    {
        _runnerImageRegistry = runnerImageRegistry;
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
                                Name = "runner",
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

    public string GetContainerImage(RunnerJobPayload payload)
    {
        return GetImageForLanguage(payload.Language, payload.LanguageVersion);
    }

    private string GetImageForLanguage(string language, string version)
    {
        var registryUri = _runnerImageRegistry.RegistryUri;
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

        return string.IsNullOrEmpty(registryUri) ? fullyQualifiedImageName : $"{registryUri}/{fullyQualifiedImageName}";
    }

    public RunnerResult ProcessResult(KubernetesJobResult result, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}
