using Tsa.Submissions.Coding.Contracts.TestCases;

namespace Tsa.Submissions.Coding.Contracts.Submissions;

public record SubmissionModifyRequest(
    DateTimeOffset EvaluatedOn,
    TestCaseResult[] TestCaseResults
);
