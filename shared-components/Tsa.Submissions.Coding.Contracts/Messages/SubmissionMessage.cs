namespace Tsa.Submissions.Coding.Contracts.Messages;

public record SubmissionMessage(
    string ProblemId,
    string SubmissionId,
    DateTimeOffset SubmittedOn,
    string UserId);
