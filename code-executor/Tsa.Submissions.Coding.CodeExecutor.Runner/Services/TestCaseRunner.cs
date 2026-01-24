using System.Diagnostics;
using Tsa.Submissions.Coding.CodeExecutor.Runner.Executors;
using Tsa.Submissions.Coding.Contracts.CodeExecutor;
using Tsa.Submissions.Coding.Contracts.TestCases;

namespace Tsa.Submissions.Coding.CodeExecutor.Runner.Services;

public class TestCaseRunner
{
    public CodeExecutionResult RunTestCases(RunnerJobPayload payload)
    {
        var result = new CodeExecutionResult
        {
            SubmissionId = payload.SubmissionId,
            Success = true
        };

        try
        {
            Console.WriteLine("Creating language specific executor");
            // Create language specific executor
            var executor = LanguageExecutorFactory.CreateExecutor(payload.Language);
             
            Console.WriteLine("Creating working directory");
            // Create working directory
            var workingDir = Path.Combine(Path.GetTempPath(), $"exec_{Guid.NewGuid():N}");
            Directory.CreateDirectory(workingDir);

            var codeExecutionContext = new CodeExecutionContext
            {
                Language = payload.Language,
                LanguageVersion = payload.LanguageVersion,
                SourceCode = payload.Solution,
                TestCases = payload.TestCases,
                WorkingDirectory = workingDir
            };

            try
            {
                Console.WriteLine("Preparing code for execution");
                executor.Prepare(codeExecutionContext);
            }
            catch (Exception ex)
            {
                // Compilation failed - all tests fail
                result.Success = false;
                result.ErrorMessage = $"Compilation failed: {ex.Message}";

                foreach (var testCase in payload.TestCases)
                {
                    result.TestCaseResults.Add(
                        new TestCaseResult(
                            testCase.Input,
                            testCase.ExpectedOutput,
                            string.Empty,
                            false,
                            false,
                            TimeSpan.Zero
                        )
                    );
                }

                return result;
            }

            var testCaseResults = ExecuteTestCases(
                executor,
                codeExecutionContext,
                TimeSpan.FromSeconds(30));
        }
        catch (Exception exception)
        {
            result.Success = false;
            result.ErrorMessage = $"Execution error: {exception.Message}";
        }

        return result;
    }

    private static List<TestCaseResult> ExecuteTestCases(
        ILanguageExecutor executor,
        CodeExecutionContext context,
        TimeSpan timeout)
    {
        var testCaseResults = new List<TestCaseResult>();
        foreach (var testCase in context.TestCases)
        {
            Console.WriteLine($"Executing test case with input: {testCase.Input}");
            var stopwatch = Stopwatch.StartNew();
            var (stdout, stderr, exitCode) = executor.Execute(context, testCase.Input, timeout);
            stopwatch.Stop();
            var passed = stdout.Trim() == testCase.ExpectedOutput.Trim() && exitCode == 0;
            testCaseResults.Add(
                new TestCaseResult(
                    testCase.Input,
                    testCase.ExpectedOutput,
                    stdout,
                    passed,
                    exitCode == 0,
                    stopwatch.Elapsed
                )
            );
            Console.WriteLine($"Test case result - Passed: {passed}, Execution Time: {stopwatch.ElapsedMilliseconds} ms");
        }
        return testCaseResults;
    }
}
