namespace Tsa.Submissions.Coding.Contracts.TestSets;

public record TestSetRequest(
    IList<TestSetValueRequest> Inputs,
    bool IsPublic,
    string Name,
    string ProblemId);
