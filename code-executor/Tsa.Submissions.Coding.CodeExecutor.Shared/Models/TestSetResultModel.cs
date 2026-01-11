namespace Tsa.Submissions.Coding.CodeExecutor.Shared.Models;

public class TestSetResultModel
{
    public bool Passed { get; set; }

    public TimeSpan? RunDuration { get; set; }

    public string? TestSetId { get; set; }
}
