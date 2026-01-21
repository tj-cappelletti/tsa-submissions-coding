namespace Tsa.Submissions.Coding.Contracts.TestSets;

public record TestSetResponse(
    string Id,
    IList<TestSetValueResponse> Inputs,
    bool IsPublic,
    string? Name,
    string? ProblemId);
