namespace Tsa.Submissions.Coding.Contracts.TestSets;

public record TestSetValueResponse(
    string DataType,
    int Index,
    bool IsArray,
    string ValueAsJson);
