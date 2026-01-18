using Tsa.Submissions.Coding.CodeExecutor.Shared.Models;

namespace Tsa.Submissions.Coding.CodeExecutor.Runner
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Runner starting up...");

            var submission = new SubmissionModel
            {
                Id = "test"
            };
            Console.WriteLine($"Mocking running process - Submission ID: {submission.Id}...");
            var sleepTimeSpan = TimeSpan.FromMinutes(1);
            Thread.Sleep(sleepTimeSpan);

            Console.WriteLine("Shutting down...");
        }
    }
}
