using System.Text.Json.Serialization;

namespace Tsa.Submissions.Coding.Contracts.TestCases;

public record TestCase
{
    [JsonPropertyName("expectedOutputDisplay")]
    public string ExpectedOutputDisplay { get; init; }

    [JsonPropertyName("expectedOutputRaw")]
    public string ExpectedOutputRaw { get; init; }

    [JsonPropertyName("inputs")]
    public List<TestCaseInput> Inputs { get; init; } = [];

    [JsonPropertyName("isActive")]
    public bool IsActive { get; init; }

    [JsonPropertyName("name")]
    public string Name { get; init; }

    [JsonPropertyName("outputDataType")]
    public string OutputDataType { get; init; }

    [JsonPropertyName("outputIsArray")]
    public bool OutputIsArray { get; init; }

    public TestCase(
        string name,
        List<TestCaseInput> inputs,
        string expectedOutputRaw,
        string expectedOutputDisplay,
        string outputDataType,
        bool outputIsArray,
        bool isActive)
    {
        Name = name;
        Inputs = inputs;
        ExpectedOutputRaw = expectedOutputRaw;
        ExpectedOutputDisplay = expectedOutputDisplay;
        OutputDataType = outputDataType;
        OutputIsArray = outputIsArray;
        IsActive = isActive;
    }

    public string GetUniqueId()
    {
        var inputs = Inputs
            .OrderBy(testCaseInput => testCaseInput.Index)
            .Select(testCaseInput => testCaseInput.DisplayInput);

        return $"{string.Join(",", inputs)}=>{ExpectedOutputDisplay}";
    }

    public override string ToString()
    {
        return $"{Name}|{GetUniqueId()}";
    }
}
