using Tsa.Submissions.Coding.Contracts.Submissions;

namespace Tsa.Submissions.Coding.CodeExecutor.Runner;

internal class Program
{
    private static void Main(string[] args)
    {
        Console.WriteLine("Runner starting up...");

        var submission = new SubmissionResponse(
            "test",
            new ProgrammingLanguageResponse("C#", "9"),
            "test",
            "test",
            DateTimeOffset.UtcNow,
            [],
            "test");

        Console.WriteLine($"Mocking running process - Submission ID: {submission.Id}...");
        var sleepTimeSpan = TimeSpan.FromMinutes(1);
        Thread.Sleep(sleepTimeSpan);

        Console.WriteLine("Shutting down...");
    }
}
