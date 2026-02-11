namespace Tsa.Submissions.Coding.CodeExecutor.Worker.Configuration;

public class KubernetesCluster
{
    public const string SectionName = "KubernetesCluster";

    public int JobTimeoutMinutes { get; set; }

    public string? Namespace { get; set; }
}
