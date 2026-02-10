using System.Text.Json.Serialization;

namespace Tsa.Submissions.Coding.Contracts.TestCases;

public class TestCaseRequest
{
    [JsonPropertyName("expectedOutput")]
    public string ExpectedOutput { get; init; }

    [JsonPropertyName("inputs")]
    public List<TestCaseInputRequest> Inputs { get; init; } = [];

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

    public TestCaseRequest(
        string name,
        List<TestCaseInputRequest> inputs,
        string expectedOutput,
        string outputDataType,
        bool outputIsArray,
        bool isActive,
        bool isPublic)
    {
        Name = name;
        Inputs = inputs;
        ExpectedOutput = expectedOutput;
        OutputDataType = outputDataType;
        OutputIsArray = outputIsArray;
        IsActive = isActive;
        IsPublic = isPublic;
    }
}
