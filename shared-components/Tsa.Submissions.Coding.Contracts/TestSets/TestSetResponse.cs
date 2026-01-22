namespace Tsa.Submissions.Coding.Contracts.TestSets;

public record TestSetResponse(
    string Id,
    List<TestSetValueResponse> Inputs,
    bool IsPublic,
    string? Name,
    string? ProblemId);
