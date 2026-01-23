namespace Tsa.Submissions.Coding.Contracts.TestCases;

public record TestCase
{
    public string ExpectedOutput { get; init; }

    public string Id => $"{Input}=>{ExpectedOutput}|{IsActive}";

    public string Input { get; init; }

    public bool IsActive { get; init; }

    public List<TestCaseLanguageFixture> LanguageFixtures { get; init; }

    public TestCase(string input, string expectedOutput, bool isActive) : this(input, expectedOutput, isActive, []) { }

    public TestCase(string input, string expectedOutput, bool isActive, List<TestCaseLanguageFixture> languageFixtures)
    {
        ExpectedOutput = expectedOutput;
        Input = input;
        IsActive = isActive;
        LanguageFixtures = languageFixtures;
    }
}
