using System.Text.Json.Serialization;
using Tsa.Submissions.Coding.Contracts.TestCases;

namespace Tsa.Submissions.Coding.Contracts.CodeExecutor;

/// <summary>
///     Result of test case execution
/// </summary>
public record RunnerResult : ExecutionResult
{
    private readonly List<TestCaseResultRequest> _testCaseResults;

    /// <summary>
    ///     Gets the test case results; if empty, no test cases were executed
    /// </summary>
    [JsonPropertyName("testCaseResults")]
    public IReadOnlyList<TestCaseResultRequest> TestCaseResults => _testCaseResults.AsReadOnly();

    /// <summary>
    ///     Creates a RunnerResult representing failed execution and optionally any results for test cases that were run.
    /// </summary>
    public RunnerResult(
        string errorMessage,
        string standardError,
        string standardOutput,
        List<TestCaseResultRequest>? testCaseResults = null)
        : base(errorMessage, standardError, standardOutput, false)
    {
        _testCaseResults = testCaseResults ?? [];
    }

    /// <summary>
    ///     Creates a RunnerResult representing successful execution with test case results.
    /// </summary>
    public RunnerResult(string standardOutput, List<TestCaseResultRequest> testCaseResults)
        : base(string.Empty, string.Empty, standardOutput, true)
    {
        _testCaseResults = testCaseResults;
        IsSuccess = _testCaseResults.All(testCaseResult => testCaseResult.Passed);
    }

    /// <summary>
    ///     Creates a RunnerResult from an exception
    /// </summary>
    public RunnerResult(Exception exception)
        : base(exception)
    {
        _testCaseResults = [];
    }

    public static RunnerResult FromException(Exception exception) => new(exception);
}
