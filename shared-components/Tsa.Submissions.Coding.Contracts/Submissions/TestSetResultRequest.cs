namespace Tsa.Submissions.Coding.Contracts.Submissions;

public record TestSetResultRequest(
    bool Passed,
    TimeSpan RunDuration,
    string TestSetId);
