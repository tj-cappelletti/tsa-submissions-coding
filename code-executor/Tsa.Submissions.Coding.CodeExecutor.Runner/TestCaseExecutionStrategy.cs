using System.Text;
using Tsa.Submissions.Coding.CodeExecutor.Core.Strategies;
using Tsa.Submissions.Coding.CodeExecutor.Runner.Executors;
using Tsa.Submissions.Coding.Contracts.CodeExecutor;

namespace Tsa.Submissions.Coding.CodeExecutor.Runner;

public class TestCaseExecutionStrategy : IExecutionStrategy<CodeExecutionResult>
{
    public const int DefaultOverheadTimeInSeconds = 2;
    public const int DefaultTimeoutPerTestCaseInSeconds = 2;

    public string StrategyName => nameof(TestCaseExecutionStrategy);

    public CodeExecutionResult Execute(RunnerJobPayload payload)
    {
        try
        {
            Console.WriteLine($"Creating language specific executor - {payload.Language}");
            var executor = LanguageExecutorFactory.CreateExecutor(payload.Language);

            var workingDir = Path.Combine(Path.GetTempPath(), $"exec_{Guid.NewGuid():N}");
            Console.WriteLine($"Creating working directory - {workingDir}");
            Directory.CreateDirectory(workingDir);

            var codeExecutionContext = new CodeExecutionContext
            {
                Language = payload.Language,
                LanguageFixture = payload.LanguageFixture,
                LanguageVersion = payload.LanguageVersion,
                SourceCode = payload.Solution,
                TestCases = payload.TestCases,
                WorkingDirectory = workingDir
            };

            var outputStringBuilder = new StringBuilder();

            Console.WriteLine("Preparing code for execution");
            var prepareExecutorResult = executor.Prepare(codeExecutionContext);

            if (prepareExecutorResult.IsFailure)
            {
                return new CodeExecutionResult("Code preparation failed", prepareExecutorResult.StandardError, prepareExecutorResult.StandardOutput);
            }

            outputStringBuilder.AppendLine("==== Prepare Code Step ====");
            outputStringBuilder.AppendLine(prepareExecutorResult.StandardOutput);
            outputStringBuilder.AppendLine();

            //TODO: Make timeout configurable
            var timeoutInSeconds = payload.TestCases.Count * DefaultTimeoutPerTestCaseInSeconds + DefaultOverheadTimeInSeconds;

            Console.WriteLine("Executing build step - Timeout(s): {0:D}", timeoutInSeconds);
            var buildExecutorResult = executor.ExecuteBuild(codeExecutionContext, TimeSpan.FromSeconds(timeoutInSeconds));

            if (buildExecutorResult.IsFailure)
            {
                return new CodeExecutionResult("Code build failed", buildExecutorResult.StandardError, buildExecutorResult.StandardOutput);
            }

            outputStringBuilder.AppendLine("==== Build Code Step ====");
            outputStringBuilder.AppendLine(buildExecutorResult.StandardOutput);
            outputStringBuilder.AppendLine();

            Console.WriteLine("Executing test cases - Timeout(s): {0:D}", timeoutInSeconds);
            var executeTestsExecutorResult = executor.ExecuteTests(codeExecutionContext, TimeSpan.FromSeconds(timeoutInSeconds));

            outputStringBuilder.AppendLine("==== Execute Tests Step ====");
            outputStringBuilder.AppendLine(executeTestsExecutorResult.StandardOutput);

            if (executeTestsExecutorResult.IsFailure)
            {
                return new CodeExecutionResult("Code execution failed", executeTestsExecutorResult.StandardError, outputStringBuilder.ToString());
            }

            var testCaseResults = executor.GetTestCaseResults(codeExecutionContext);

            return new CodeExecutionResult(outputStringBuilder.ToString(), testCaseResults);
        }
        catch (Exception exception)
        {
            Console.WriteLine($"Exception occurred: {exception.Message}");
            return CodeExecutionResult.FromException(exception);
        }
    }
}
