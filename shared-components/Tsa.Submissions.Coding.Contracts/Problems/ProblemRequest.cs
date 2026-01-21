namespace Tsa.Submissions.Coding.Contracts.Problems;

public record ProblemRequest(
    string Description,
    bool IsActive,
    string Title);
