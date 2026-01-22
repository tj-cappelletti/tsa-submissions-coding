namespace Tsa.Submissions.Coding.Contracts.CodeExecutor;

public class RunnerJobResult
{
    public bool IsFailure => !IsSuccessful;
    public bool IsSuccessful { get; init; }
}
