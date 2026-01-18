namespace Tsa.Submissions.Coding.CodeExecutor.Worker.Configuration;

public class RunnerImageRegistry
{
    public const string SectionName = "RunnerImageRegistry";

    public string? ImageName { get; set; }

    public string? ImageVersion { get; set; }

    public LanguageTags? LanguageTags { get; set; }

    public string? RegistryUrl { get; set; }
}

public class LanguageTags
{
    public string? C { get; set; }

    public string? Cpp { get; set; }

    public string? CSharp { get; set; }

    public string? FSharp { get; set; }

    public string? Go { get; set; }

    public string? Java { get; set; }

    public string? NodeJs { get; set; }

    public string? Python { get; set; }

    public string? Ruby { get; set; }

    public string? VisualBasic { get; set; }
}
