using Tsa.Submissions.Coding.CodeExecutor.Runner.Services;
using Tsa.Submissions.Coding.CodeExecutor.Shared.Models;

namespace Tsa.Submissions.Coding.CodeExecutor.UnitTests.Services;

/// <summary>
/// Unit tests for TestCaseRunner
/// </summary>
public class TestCaseRunnerTests
{
    [Fact]
    [Trait("TestCategory", "UnitTest")]
    public async Task RunTestCasesAsync_WithValidPythonCode_ShouldReturnSuccessfulResult()
    {
        // Arrange
        var runner = new TestCaseRunner();
        var payload = new ExecutionPayload
        {
            SubmissionId = "test-123",
            Language = "Python",
            SourceCode = "print(input())",
            TestCases =
            [
                new TestCaseInput
                {
                    TestCaseId = "test-1",
                    Input = "Hello",
                    ExpectedOutput = "Hello",
                    TimeoutMs = 5000
                }
            ]
        };

        // Act
        var result = await runner.RunTestCasesAsync(payload);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("test-123", result.SubmissionId);
        Assert.True(result.Success);
        Assert.Single(result.TestResults);
        Assert.True(result.TestResults[0].Passed);
        Assert.Equal("Hello", result.TestResults[0].ActualOutput);
    }

    [Fact]
    [Trait("TestCategory", "UnitTest")]
    public async Task RunTestCasesAsync_WithInvalidPythonCode_ShouldReturnCompilationError()
    {
        // Arrange
        var runner = new TestCaseRunner();
        var payload = new ExecutionPayload
        {
            SubmissionId = "test-124",
            Language = "Python",
            SourceCode = "this is not valid python code{{{",
            TestCases =
            [
                new TestCaseInput
                {
                    TestCaseId = "test-1",
                    Input = "Hello",
                    ExpectedOutput = "Hello",
                    TimeoutMs = 5000
                }
            ]
        };

        // Act
        var result = await runner.RunTestCasesAsync(payload);

        // Assert
        Assert.NotNull(result);
        Assert.False(result.Success);
        Assert.NotNull(result.ErrorMessage);
        Assert.Contains("Compilation failed", result.ErrorMessage);
    }

    [Fact]
    [Trait("TestCategory", "UnitTest")]
    public async Task RunTestCasesAsync_WithIncorrectOutput_ShouldReturnFailedTest()
    {
        // Arrange
        var runner = new TestCaseRunner();
        var payload = new ExecutionPayload
        {
            SubmissionId = "test-125",
            Language = "Python",
            SourceCode = "print('Wrong output')",
            TestCases =
            [
                new TestCaseInput
                {
                    TestCaseId = "test-1",
                    Input = "",
                    ExpectedOutput = "Correct output",
                    TimeoutMs = 5000
                }
            ]
        };

        // Act
        var result = await runner.RunTestCasesAsync(payload);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.Success); // Execution succeeded, but test failed
        Assert.Single(result.TestResults);
        Assert.False(result.TestResults[0].Passed);
        Assert.Equal("Wrong output", result.TestResults[0].ActualOutput);
        Assert.Equal("Correct output", result.TestResults[0].ExpectedOutput);
    }

    [Fact]
    [Trait("TestCategory", "UnitTest")]
    public async Task RunTestCasesAsync_WithUnsupportedLanguage_ShouldReturnError()
    {
        // Arrange
        var runner = new TestCaseRunner();
        var payload = new ExecutionPayload
        {
            SubmissionId = "test-126",
            Language = "UnsupportedLanguage",
            SourceCode = "print('test')",
            TestCases =
            [
                new TestCaseInput
                {
                    TestCaseId = "test-1",
                    Input = "",
                    ExpectedOutput = "test",
                    TimeoutMs = 5000
                }
            ]
        };

        // Act
        var result = await runner.RunTestCasesAsync(payload);

        // Assert
        Assert.NotNull(result);
        Assert.False(result.Success);
        Assert.NotNull(result.ErrorMessage);
    }
}
