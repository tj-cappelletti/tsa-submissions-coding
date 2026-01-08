namespace Tsa.Submissions.Coding.CodeExecutor.Worker.Models;

/// <summary>
/// Message received from RabbitMQ queue containing submission information
/// </summary>
public class SubmissionMessage
{
    /// <summary>
    /// Gets or sets the unique identifier for the submission
    /// </summary>
    public required string SubmissionId { get; set; }

    /// <summary>
    /// Gets or sets the programming language
    /// </summary>
    public required string Language { get; set; }
}
