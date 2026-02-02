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
        var outputToken = Environment.GetEnvironmentVariable("OUTPUT_TOKEN") ?? "###CODE-EXECUTION-RESULT###";

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

        var codeExecutionResult = testCaseRunner.RunTestCases(runnerJobPayload);

        var codeExecutionResultJson = JsonSerializer.Serialize(codeExecutionResult);

        Console.WriteLine($"Runner job for submission {runnerJobPayload.SubmissionId} execution completed.");
        Console.WriteLine(outputToken);
        Console.WriteLine(codeExecutionResultJson);

        return 0;
    }
}
