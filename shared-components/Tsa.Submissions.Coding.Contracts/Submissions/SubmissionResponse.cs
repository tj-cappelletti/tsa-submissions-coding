using Tsa.Submissions.Coding.Contracts.Users;

namespace Tsa.Submissions.Coding.Contracts.Submissions;

public record SubmissionResponse(
    string Id,
    ProgrammingLanguageResponse Language,
    string ProblemId,
    string Solution,
    DateTimeOffset SubmittedOn,
    IList<TestSetResultResponse> TestSetResults,
    string UserId);
