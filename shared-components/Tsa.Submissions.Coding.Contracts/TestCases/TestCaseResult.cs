namespace Tsa.Submissions.Coding.Contracts.TestCases;

public record TestCaseResult(
    string TestCaseId,
    string ActualOutput,
    string? Message,
    bool Passed,
    bool TimedOut,
    TimeSpan ExecutionTime
);
