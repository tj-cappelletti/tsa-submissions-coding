namespace Tsa.Submissions.Coding.CodeExecutor.Shared.Models;

public record RunnerJobPayload
{
    public required ProgrammingLanguage Language { get; init; }

    public required string ProblemId { get; init; }

    public required string Solution { get; init; }

    public required string SubmissionId { get; init; }
}
