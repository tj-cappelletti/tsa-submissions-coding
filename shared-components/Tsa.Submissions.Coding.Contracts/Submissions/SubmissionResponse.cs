using Tsa.Submissions.Coding.Contracts.TestCases;

namespace Tsa.Submissions.Coding.Contracts.Submissions;

public record SubmissionResponse(
    string Id,
    ProgrammingLanguageResponse Language,
    string ProblemId,
    string Solution,
    DateTimeOffset SubmittedOn,
    List<TestCaseResult> TestCaseResults,
    string UserId);
