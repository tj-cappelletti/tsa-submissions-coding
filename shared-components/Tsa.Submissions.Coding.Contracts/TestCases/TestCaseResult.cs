namespace Tsa.Submissions.Coding.Contracts.TestCases;

public record TestCaseResult(
    string Input,
    string ExpectedOutput,
    string ActualOutput,
    bool Passed,
    bool TimedOut,
    TimeSpan ExecutionTime
);
