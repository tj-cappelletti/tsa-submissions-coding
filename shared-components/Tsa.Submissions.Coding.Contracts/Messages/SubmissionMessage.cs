using System.Text.Json.Serialization;

namespace Tsa.Submissions.Coding.Contracts.Messages;

public record SubmissionMessage
{
    [JsonPropertyName("problemId")]
    public string ProblemId { get; }

    [JsonPropertyName("submissionId")]
    public string SubmissionId { get; }

    [JsonPropertyName("submittedOn")]
    public DateTimeOffset SubmittedOn { get; }

    [JsonPropertyName("userId")]
    public string UserId { get; }

    public SubmissionMessage(string problemId, string submissionId, DateTimeOffset submittedOn, string userId)
    {
        ProblemId = problemId;
        SubmissionId = submissionId;
        SubmittedOn = submittedOn;
        UserId = userId;
    }
}
