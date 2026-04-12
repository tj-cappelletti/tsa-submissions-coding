using Tsa.Submissions.Coding.CodeExecutor.Core.Orchestrators;

namespace Tsa.Submissions.Coding.CodeExecutor.Scorer;

internal class Program
{
    private static int Main(string[] _)
    {
        return ExecutionOrchestrator.Run(new ScorerExecutionStrategy());
    }
}
