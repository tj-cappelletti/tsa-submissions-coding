namespace Tsa.Submissions.Coding.Contracts.Submissions;

public record SubmissionModifyRequest(
    DateTimeOffset EvaluatedOn,
    TestSetResultRequest[] TestSetResults);
