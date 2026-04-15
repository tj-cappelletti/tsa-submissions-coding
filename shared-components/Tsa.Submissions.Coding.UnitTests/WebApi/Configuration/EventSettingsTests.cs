using System.Diagnostics.CodeAnalysis;
using Tsa.Submissions.Coding.WebApi.Configuration;
using Xunit;

namespace Tsa.Submissions.Coding.UnitTests.WebApi.Configuration;

[ExcludeFromCodeCoverage]
public class EventSettingsTests
{
    [Theory]
    [Trait("TestCategory", "UnitTest")]
    [InlineData(0, EventSettingsConfigError.DurationInMinutes)]
    [InlineData(-1, EventSettingsConfigError.DurationInMinutes)]
    public void GetError_InvalidFields_ReturnsExpectedError(int durationInMinutes, EventSettingsConfigError expectedError)
    {
        // Arrange
        var eventSettings = new EventSettings { DurationInMinutes = durationInMinutes };

        // Act
        var result = eventSettings.GetError();

        // Assert
        Assert.Equal(expectedError, result);
    }

    [Theory]
    [Trait("TestCategory", "UnitTest")]
    [InlineData(0)]
    [InlineData(-1)]
    public void IsValid_InvalidFields_ReturnsFalse(int durationInMinutes)
    {
        // Arrange
        var eventSettings = new EventSettings { DurationInMinutes = durationInMinutes };

        // Act
        var result = eventSettings.IsValid();

        // Assert
        Assert.False(result);
    }

    [Fact]
    [Trait("TestCategory", "UnitTest")]
    public void GetError_ValidFields_ReturnsNone()
    {
        // Arrange
        var eventSettings = new EventSettings { DurationInMinutes = 120 };

        // Act
        var result = eventSettings.GetError();

        // Assert
        Assert.Equal(EventSettingsConfigError.None, result);
    }

    [Fact]
    [Trait("TestCategory", "UnitTest")]
    public void IsValid_ValidFields_ReturnsTrue()
    {
        // Arrange
        var eventSettings = new EventSettings { DurationInMinutes = 120 };

        // Act
        var result = eventSettings.IsValid();

        // Assert
        Assert.True(result);
    }
}
