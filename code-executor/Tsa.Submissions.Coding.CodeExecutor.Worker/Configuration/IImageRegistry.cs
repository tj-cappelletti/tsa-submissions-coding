namespace Tsa.Submissions.Coding.CodeExecutor.Worker.Configuration;

public interface IImageRegistry
{
    string? ImageName { get; set; }

    string? ImageVersion { get; set; }

    string? RegistryUri { get; set; }
}
