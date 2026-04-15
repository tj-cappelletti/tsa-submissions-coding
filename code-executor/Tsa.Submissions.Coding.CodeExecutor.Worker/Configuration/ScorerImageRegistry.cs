namespace Tsa.Submissions.Coding.CodeExecutor.Worker.Configuration;

public class ScorerImageRegistry: IImageRegistry
{
    public const string SectionName = "ScorerImageRegistry";

    public string? ImageName { get; set; }

    public string? ImageVersion { get; set; }

    public string? RegistryUri { get; set; }
}
