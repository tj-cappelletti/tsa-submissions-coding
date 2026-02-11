using System.Diagnostics.CodeAnalysis;
using Tsa.Submissions.Coding.WebApi.ExtensionMethods;
using Xunit;

namespace Tsa.Submissions.Coding.UnitTests.WebApi.ExtensionMethods;

[ExcludeFromCodeCoverage]
public class StringExtensionsTests
{
    [Fact]
    [Trait("TestCategory", "UnitTest")]
    public void SanitizeForLogging_LongString_TruncatesToMaxLength()
    {
        // Arrange
        var input = new string('a', 1100); // Create a string longer than 1000 characters

        // Act
        var result = input.SanitizeForLogging();

        // Assert
        Assert.True(result.Length <= 1015); // 1000 characters + "... [TRUNCATED]"
        Assert.EndsWith("... [TRUNCATED]", result);
    }

    [Fact]
    [Trait("TestCategory", "UnitTest")]
    public void SanitizeForLogging_NullOrEmptyString_ReturnsEmptyString()
    {
        // Arrange
        string? input = null;

        // Act
        var result = input.SanitizeForLogging();

        // Assert
        Assert.Equal(string.Empty, result);

        // Test for empty string
        input = string.Empty;
        result = input.SanitizeForLogging();
        Assert.Equal(string.Empty, result);

        // Test for whitespace string
        input = "   ";
        result = input.SanitizeForLogging();
        Assert.Equal(string.Empty, result);
    }

    [Fact]
    [Trait("TestCategory", "UnitTest")]
    public void SanitizeForLogging_StringWithMixedSensitiveData_RedactsAndTrims()
    {
        // Arrange
        var input = "   token=abcdef123456&password=12345&safeData=example   ";

        // Act
        var result = input.SanitizeForLogging();

        // Assert
        Assert.Equal("REDACTED&REDACTED&safeData=example", result);
    }

    [Fact]
    [Trait("TestCategory", "UnitTest")]
    public void SanitizeForLogging_StringWithoutSensitiveData_ReturnsTrimmedString()
    {
        // Arrange
        var input = "   This is a safe log message.   ";

        // Act
        var result = input.SanitizeForLogging();

        // Assert
        Assert.Equal("This is a safe log message.", result);
    }

    [Fact]
    [Trait("TestCategory", "UnitTest")]
    public void SanitizeForLogging_StringWithSensitiveData_RedactsSensitiveData()
    {
        // Arrange
        var input = "password=12345&token=abcdef123456&otherdata=example";

        // Act
        var result = input.SanitizeForLogging();

        // Assert
        Assert.Equal("REDACTED&REDACTED&otherdata=example", result);
    }
}
