using System.Diagnostics;
using Tsa.Submissions.Coding.CodeExecutor.Runner.Executors;
using Tsa.Submissions.Coding.CodeExecutor.Shared.Models;

namespace Tsa.Submissions.Coding.CodeExecutor.Runner.Services;

/// <summary>
/// Service for running test cases against code
/// </summary>
public class TestCaseRunner
{
    /// <summary>
    /// Runs all test cases for the given execution payload
    /// </summary>
    /// <param name="payload">The execution payload</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Execution result with test results</returns>
    public async Task<ExecutionResult> RunTestCasesAsync(ExecutionPayload payload, CancellationToken cancellationToken = default)
    {
        var result = new ExecutionResult
        {
            SubmissionId = payload.SubmissionId,
            Success = true
        };

        try
        {
            // Create executor for the language
            var executor = LanguageExecutorFactory.CreateExecutor(payload.Language);

            // Create working directory
            var workingDir = Path.Combine(Path.GetTempPath(), $"exec_{Guid.NewGuid():N}");
            Directory.CreateDirectory(workingDir);

            try
            {
                var context = new Executors.ExecutionContext
                {
                    SourceCode = payload.SourceCode,
                    WorkingDirectory = workingDir,
                    Language = payload.Language
                };

                // Prepare code (compile if needed)
                try
                {
                    await executor.PrepareAsync(context, cancellationToken);
                }
                catch (Exception ex)
                {
                    // Compilation failed - all tests fail
                    result.Success = false;
                    result.ErrorMessage = $"Compilation failed: {ex.Message}";
                    
                    foreach (var testCase in payload.TestCases)
                    {
                        result.TestResults.Add(new TestResult
                        {
                            TestCaseId = testCase.TestCaseId,
                            Passed = false,
                            ExpectedOutput = testCase.ExpectedOutput,
                            Error = $"Compilation failed: {ex.Message}",
                            ExecutionTimeMs = 0
                        });
                    }
                    
                    return result;
                }

                // Run each test case
                foreach (var testCase in payload.TestCases)
                {
                    var testResult = await RunSingleTestCaseAsync(executor, context, testCase, cancellationToken);
                    result.TestResults.Add(testResult);
                }
            }
            finally
            {
                // Cleanup working directory
                try
                {
                    Directory.Delete(workingDir, true);
                }
                catch
                {
                    // Ignore cleanup errors
                }
            }
        }
        catch (Exception ex)
        {
            result.Success = false;
            result.ErrorMessage = $"Execution error: {ex.Message}";
        }

        return result;
    }

    private static async Task<TestResult> RunSingleTestCaseAsync(
        ILanguageExecutor executor,
        Executors.ExecutionContext context,
        TestCaseInput testCase,
        CancellationToken cancellationToken)
    {
        var stopwatch = Stopwatch.StartNew();
        
        try
        {
            var (stdout, stderr, exitCode) = await executor.ExecuteAsync(
                context,
                testCase.Input,
                testCase.TimeoutMs,
                cancellationToken);

            stopwatch.Stop();

            // Check for timeout
            if (exitCode == -1)
            {
                return new TestResult
                {
                    TestCaseId = testCase.TestCaseId,
                    Passed = false,
                    ExpectedOutput = testCase.ExpectedOutput,
                    Error = "Execution timed out",
                    ExecutionTimeMs = stopwatch.ElapsedMilliseconds
                };
            }

            // Check for runtime errors
            if (exitCode != 0)
            {
                return new TestResult
                {
                    TestCaseId = testCase.TestCaseId,
                    Passed = false,
                    ActualOutput = stdout,
                    ExpectedOutput = testCase.ExpectedOutput,
                    Error = $"Runtime error (exit code {exitCode}): {stderr}",
                    ExecutionTimeMs = stopwatch.ElapsedMilliseconds
                };
            }

            // Compare output
            var actualOutput = stdout.Trim();
            var expectedOutput = testCase.ExpectedOutput.Trim();
            var passed = string.Equals(actualOutput, expectedOutput, StringComparison.Ordinal);

            return new TestResult
            {
                TestCaseId = testCase.TestCaseId,
                Passed = passed,
                ActualOutput = actualOutput,
                ExpectedOutput = expectedOutput,
                ExecutionTimeMs = stopwatch.ElapsedMilliseconds
            };
        }
        catch (Exception ex)
        {
            stopwatch.Stop();
            return new TestResult
            {
                TestCaseId = testCase.TestCaseId,
                Passed = false,
                ExpectedOutput = testCase.ExpectedOutput,
                Error = $"Execution error: {ex.Message}",
                ExecutionTimeMs = stopwatch.ElapsedMilliseconds
            };
        }
    }
}
