using System.Text.Json;
using Tsa.Submissions.Coding.CodeExecutor.Runner.Services;
using Tsa.Submissions.Coding.Contracts.CodeExecutor;

namespace Tsa.Submissions.Coding.CodeExecutor.Runner;

internal class Program
{
    private static int Main(string[] args)
    {
        Console.WriteLine("Runner starting up...");

        var executionPayloadJson = Environment.GetEnvironmentVariable("EXECUTION_PAYLOAD");

        if (string.IsNullOrEmpty(executionPayloadJson))
        {
            Console.Error.WriteLine("ERROR: EXECUTION_PAYLOAD environment variable not set");
            return -1;
        }

        var runnerJobPayload = JsonSerializer.Deserialize<RunnerJobPayload>(executionPayloadJson);

        if (runnerJobPayload == null)
        {
            Console.Error.WriteLine("ERROR: Failed to deserialize EXECUTION_PAYLOAD");
            return -1;
        }

        Console.WriteLine($"Executing submission {runnerJobPayload.SubmissionId}...");
        var testCaseRunner = new TestCaseRunner();

        var result = testCaseRunner.RunTestCases(runnerJobPayload);

        if (result.Success)
        {
            Console.WriteLine("Execution succeeded.");
            Console.WriteLine("Test Case Results:");
            foreach (var testCaseResult in result.TestCaseResults)
            {
                Console.WriteLine(
                    $"- Test Set Inputs: {testCaseResult.Input}, Passed: {testCaseResult.Passed}, Execution Time: {testCaseResult.ExecutionTime.Milliseconds} ms");
            }
        }
        else
        {
            Console.WriteLine($"Execution failed with error: {result.ErrorMessage}");
        }

        Console.WriteLine($"Submission {runnerJobPayload.SubmissionId} execution completed.");
        return 0;
    }
}
