namespace Tsa.Submissions.Coding.Contracts.TestSets;

public record TestSetRequest(
    List<TestSetValueRequest> Inputs,
    bool IsPublic,
    string Name,
    string ProblemId);
