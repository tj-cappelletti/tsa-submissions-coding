namespace Tsa.Submissions.Coding.CodeExecutor.Shared.Models;

public record RunnerJobResult
{
    /// <summary>
    ///     Indicates whether the code compiled successfully.
    /// </summary>
    public bool CodeCompiled { get; init; }

    /// <summary>
    ///     Indicates whether the job failed.
    /// </summary>
    public bool IsFailure => !IsSuccessful;

    /// <summary>
    ///     Indicates whether the job completed successfully.
    /// </summary>
    public bool IsSuccessful { get; init; }

    /// <summary>
    ///     The results of each test set executed as part of the job.
    /// </summary>
    public IList<TestSetResultModel> TestSetResults { get; init; } = new List<TestSetResultModel>();
}
