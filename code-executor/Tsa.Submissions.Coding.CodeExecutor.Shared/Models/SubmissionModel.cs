namespace Tsa.Submissions.Coding.CodeExecutor.Shared.Models;

public class SubmissionModel
{
    public string? Id { get; set; }

    public bool IsFinalSubmission { get; set; }

    public ProgrammingLanguage? Language { get; set; }

    public string? ProblemId { get; set; }

    public string? Solution { get; set; }

    public DateTime? SubmittedOn { get; set; }

    public IList<TestSetResultModel>? TestSetResults { get; set; }

    public UserModel? User { get; set; }

    public void EnsureModelIsValid()
    {
        if (string.IsNullOrWhiteSpace(Id)) throw new InvalidOperationException("Submission ID cannot be null or empty.");

        if (Language == null || string.IsNullOrWhiteSpace(Language.Name)) throw new InvalidOperationException("A programming Language must be specified.");

        if (string.IsNullOrWhiteSpace(Solution)) throw new InvalidOperationException("The solution cannot be null or empty.");
    }
}
