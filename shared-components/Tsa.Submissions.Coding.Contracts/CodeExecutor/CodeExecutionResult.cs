using System.Text.Json.Serialization;
using Tsa.Submissions.Coding.Contracts.TestCases;

namespace Tsa.Submissions.Coding.Contracts.CodeExecutor;

/// <summary>
///     Result of code execution and test results
/// </summary>
public record CodeExecutionResult : ExecutionResult
{
    private readonly List<TestCaseResultRequest> _testCaseResults;

    /// <summary>
    ///     Gets or sets the test case results; if empty, no test cases were executed
    /// </summary>
    [JsonPropertyName("testCaseResults")]
    public IReadOnlyList<TestCaseResultRequest> TestCaseResults => _testCaseResults.AsReadOnly();

    /// <summary>
    ///     Creates a CodeExecutionResult representing failed execution and optionally any results for test cases that were
    ///     run.
    /// </summary>
    /// <param name="errorMessage">A message indicating what the error problem is</param>
    /// <param name="standardError">The standard error output captured during execution</param>
    /// <param name="standardOutput">The standard output captured during execution</param>
    /// <param name="testCaseResults">Optional list of test case results</param>
    public CodeExecutionResult(
        string errorMessage,
        string standardError,
        string standardOutput,
        List<TestCaseResultRequest>? testCaseResults = null) :
        base(errorMessage, standardError, standardOutput, false)
    {
        _testCaseResults = testCaseResults ?? [];
    }

    public CodeExecutionResult(Exception exception) : base(exception)
    {
        _testCaseResults = [];
    }

    /// <summary>
    ///     Creates a CodeExecutionResult representing successful execution with test case results.
    /// </summary>
    /// <param name="standardOutput">The standard output captured during execution</param>
    /// <param name="testCaseResults">The list of test case results for the execution</param>
    public CodeExecutionResult(string standardOutput, List<TestCaseResultRequest> testCaseResults) :
        base(string.Empty, string.Empty, standardOutput, true)
    {
        _testCaseResults = testCaseResults;

        IsSuccess = _testCaseResults.All(testCaseResult => testCaseResult.Passed);
    }

    public static CodeExecutionResult FromException(Exception exception)
    {
        return new CodeExecutionResult(exception);
    }
}
