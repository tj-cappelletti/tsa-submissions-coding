namespace Tsa.Submissions.Coding.Contracts.Submissions;

public record TestSetResultResponse(
    bool Passed,
    TimeSpan RunDuration,
    string TestSetId);
