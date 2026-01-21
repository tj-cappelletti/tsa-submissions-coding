namespace Tsa.Submissions.Coding.Contracts.Submissions;

public record SubmissionCreateRequest(
    ProgrammingLanguageRequest Language,
    string ProblemId,
    string Solution,
    DateTimeOffset SubmittedOn);
