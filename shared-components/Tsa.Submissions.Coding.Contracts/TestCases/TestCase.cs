using System.Text.Json.Serialization;

namespace Tsa.Submissions.Coding.Contracts.TestCases;

public record TestCase
{
    [JsonPropertyName("expectedOutput")]
    public string ExpectedOutput { get; init; }

    [JsonPropertyName("input")]
    public string Input { get; init; }

    [JsonPropertyName("isActive")]
    public bool IsActive { get; init; }

    [JsonPropertyName("languageFixtures")]
    public List<TestCaseLanguageFixture> LanguageFixtures { get; init; }

    [JsonPropertyName("name")]
    public string Name { get; init; }

    public TestCase(string name, string input, string expectedOutput, bool isActive) : this(name, input, expectedOutput, isActive, []) { }

    [JsonConstructor]
    public TestCase(string name, string input, string expectedOutput, bool isActive, List<TestCaseLanguageFixture> languageFixtures)
    {
        ExpectedOutput = expectedOutput;
        Input = input;
        IsActive = isActive;
        LanguageFixtures = languageFixtures;
        Name = name;
    }

    public string GetUniqueId()
    {
        return $"{Input}=>{ExpectedOutput}|{IsActive}";
    }
}
