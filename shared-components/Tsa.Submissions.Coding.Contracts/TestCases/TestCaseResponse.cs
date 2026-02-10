using System.Text.Json.Serialization;

namespace Tsa.Submissions.Coding.Contracts.TestCases;

public record TestCaseResponse
{
    [JsonPropertyName("expectedOutput")]
    public string ExpectedOutput { get; init; }

    [JsonPropertyName("id")]
    public string Id { get; init; }

    [JsonPropertyName("inputs")]
    public List<TestCaseInputResponse> Inputs { get; init; } = [];

    [JsonPropertyName("isActive")]
    public bool IsActive { get; init; }

    [JsonPropertyName("isPublic")]
    public bool IsPublic { get; init; }

    [JsonPropertyName("name")]
    public string Name { get; init; }

    [JsonPropertyName("outputDataType")]
    public string OutputDataType { get; init; }

    [JsonPropertyName("outputIsArray")]
    public bool OutputIsArray { get; init; }

    [JsonPropertyName("problemId")]
    public string ProblemId { get; init; }

    [JsonPropertyName("signature")]
    public string Signature { get; init; }

    public TestCaseResponse(
        string id,
        string problemId,
        string name,
        List<TestCaseInputResponse> inputs,
        string expectedOutput,
        string outputDataType,
        bool outputIsArray,
        string signature,
        bool isActive,
        bool isPublic)
    {
        ExpectedOutput = expectedOutput;
        Id = id;
        Inputs = inputs;
        IsActive = isActive;
        IsPublic = isPublic;
        Name = name;
        OutputDataType = outputDataType;
        OutputIsArray = outputIsArray;
        ProblemId = problemId;
        Signature = signature;
    }

    public override string ToString()
    {
        return $"{Name}|{Signature}";
    }
}
