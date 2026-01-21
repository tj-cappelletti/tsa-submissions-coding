namespace Tsa.Submissions.Coding.Contracts.TestSets;

public record TestSetValueRequest(
    string DataType,
    int Index,
    bool IsArray,
    string? ValueAsJson);
