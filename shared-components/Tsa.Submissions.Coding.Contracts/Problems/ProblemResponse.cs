using Tsa.Submissions.Coding.Contracts.TestCases;

namespace Tsa.Submissions.Coding.Contracts.Problems;

public record ProblemResponse
{
    public string Description { get; init; }

    public string Id { get; init; }

    public bool IsActive { get; init; }

    public List<TestCase> TestCases { get; init; } = [];

    public string Title { get; init; }

    public ProblemResponse(string id, string title, string description, bool isActive)
    {
        Id = id;
        Title = title;
        Description = description;
        IsActive = isActive;
    }

    public ProblemResponse(string id, string title, string description, bool isActive, List<TestCase> TestCases)
    {
        Id = id;
        Title = title;
        Description = description;
        IsActive = isActive;
        TestCases = TestCases;
    }
}
