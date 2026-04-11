using Tsa.Submissions.Coding.CodeExecutor.Core.Orchestrators;

namespace Tsa.Submissions.Coding.CodeExecutor.Runner;

internal class Program
{
    private static int Main(string[] _)
    {
        return ExecutionOrchestrator.Run(new TestCaseExecutionStrategy());
    }
}
