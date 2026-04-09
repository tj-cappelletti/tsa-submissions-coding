namespace Tsa.Submissions.Coding.WebApi.Entities;

public class CodeMetrics
{
    public int CyclomaticComplexity { get; set; }

    public long ExecutionTimeInMs { get; set; }

    public int LinesOfCode { get; set; }
}
