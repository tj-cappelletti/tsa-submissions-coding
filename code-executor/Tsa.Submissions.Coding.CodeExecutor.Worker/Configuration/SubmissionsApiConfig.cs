namespace Tsa.Submissions.Coding.CodeExecutor.Worker.Configuration;

public class SubmissionsApiConfig
{
    public const string SectionName = "SubmissionsApi";

    public ApiAuthConfig Authentication { get; set; } = new();

    public string BaseUrl { get; set; } = string.Empty;
}
