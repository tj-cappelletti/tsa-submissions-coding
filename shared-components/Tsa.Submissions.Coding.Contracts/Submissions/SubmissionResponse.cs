using Tsa.Submissions.Coding.Contracts.Users;

namespace Tsa.Submissions.Coding.Contracts.Submissions;

public record SubmissionResponse(
    string Id,
    ProgrammingLanguageResponse Language,
    string ProblemId,
    string Solution,
    DateTimeOffset SubmittedOn,
    List<TestSetResultResponse> TestSetResults,
    string UserId);
