using Tsa.Submissions.Coding.Contracts.TestSets;

namespace Tsa.Submissions.Coding.Contracts.Problems;

public record ProblemResponse
{
    public string Description { get; init; }

    public string Id { get; init; }

    public bool IsActive { get; init; }

    public IList<TestSetResponse> TestSets { get; init; } = new List<TestSetResponse>();

    public string Title { get; init; }

    public ProblemResponse(string id, string title, string description, bool isActive)
    {
        Id = id;
        Title = title;
        Description = description;
        IsActive = isActive;
    }

    public ProblemResponse(string id, string title, string description, bool isActive, IList<TestSetResponse> testSets)
    {
        Id = id;
        Title = title;
        Description = description;
        IsActive = isActive;
        TestSets = testSets;
    }
}
