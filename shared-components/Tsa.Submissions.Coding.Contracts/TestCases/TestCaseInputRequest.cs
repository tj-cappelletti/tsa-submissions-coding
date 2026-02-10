using System.Text.Json.Serialization;

namespace Tsa.Submissions.Coding.Contracts.TestCases;

public record TestCaseInputRequest
{
    [JsonPropertyName("dataType")]
    public string DataType { get; init; }

    [JsonPropertyName("index")]
    public int Index { get; init; }

    [JsonPropertyName("isArray")]
    public bool IsArray { get; init; }

    [JsonPropertyName("value")]
    public string Value { get; init; }

    public TestCaseInputRequest(string dataType, bool isArray, string value, int index)
    {
        DataType = dataType;
        IsArray = isArray;
        Index = index;
        Value = value;
    }
}
