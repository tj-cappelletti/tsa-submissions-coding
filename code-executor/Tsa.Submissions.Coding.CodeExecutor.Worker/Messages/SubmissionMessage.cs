using System.Text.Json.Serialization;

namespace Tsa.Submissions.Coding.CodeExecutor.Worker.Messages;

public class SubmissionMessage
{
    [JsonPropertyName("submissionId")]
    public string? SubmissionId { get; set; }

    [JsonPropertyName("submittedAt")]
    public DateTimeOffset SubmittedAt { get; set; }

    public void EnsureMessageIsValid()
    {
        if (string.IsNullOrWhiteSpace(SubmissionId)) throw new InvalidOperationException("SubmissionId cannot be default");

        if (SubmittedAt == DateTimeOffset.MinValue) throw new InvalidOperationException("SubmittedAt cannot be default");
    }
}
