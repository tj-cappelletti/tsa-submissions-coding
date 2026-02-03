using System.Text.Json.Serialization;

namespace Tsa.Submissions.Coding.Contracts.TestCases;

public record TestCaseInput
{
    [JsonPropertyName("dataType")]
    public string DataType { get; init; }

    [JsonPropertyName("displayInput")]
    public string DisplayInput { get; init; }

    [JsonPropertyName("index")]
    public int Index { get; init; }

    [JsonPropertyName("isArray")]
    public bool IsArray { get; init; }

    [JsonPropertyName("rawInput")]
    public string RawInput { get; init; }

    public TestCaseInput(string dataType, string displayInput, bool isArray, int index, string rawInput)
    {
        DataType = dataType;
        DisplayInput = displayInput;
        IsArray = isArray;
        Index = index;
        RawInput = rawInput;
    }
}
