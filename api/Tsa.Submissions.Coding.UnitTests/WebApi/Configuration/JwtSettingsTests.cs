using Tsa.Submissions.Coding.WebApi.Configuration;
using Xunit;

namespace Tsa.Submissions.Coding.UnitTests.WebApi.Configuration;

public class JwtSettingsTests
{
    [Theory]
    [Trait("TestCategory", "UnitTest")]
    [InlineData(null, "ValidIssuer", "ValidKey", 1, JwtSettingsConfigError.Audience)]
    [InlineData("", "ValidIssuer", "ValidKey", 1, JwtSettingsConfigError.Audience)]
    [InlineData("ValidAudience", null, "ValidKey", 1, JwtSettingsConfigError.Issuer)]
    [InlineData("ValidAudience", "", "ValidKey", 1, JwtSettingsConfigError.Issuer)]
    [InlineData("ValidAudience", "ValidIssuer", null, 1, JwtSettingsConfigError.Key)]
    [InlineData("ValidAudience", "ValidIssuer", "", 1, JwtSettingsConfigError.Key)]
    [InlineData("ValidAudience", "ValidIssuer", "ValidKey", 0, JwtSettingsConfigError.ExpirationInHours)]
    [InlineData("ValidAudience", "ValidIssuer", "ValidKey", -1, JwtSettingsConfigError.ExpirationInHours)]
    public void GetError_InvalidFields_ReturnsExpectedError(
        string? audience,
        string? issuer,
        string? key,
        int expirationInHours,
        JwtSettingsConfigError expectedError)
    {
        // Arrange
        var jwtSettings = new JwtSettings
        {
            Audience = audience,
            Issuer = issuer,
            Key = key,
            ExpirationInHours = expirationInHours
        };

        // Act
        var result = jwtSettings.GetError();

        // Assert
        Assert.Equal(expectedError, result);
    }

    [Theory]
    [Trait("TestCategory", "UnitTest")]
    [InlineData(null, "ValidIssuer", "ValidKey", 1)]
    [InlineData("", "ValidIssuer", "ValidKey", 1)]
    [InlineData("ValidAudience", null, "ValidKey", 1)]
    [InlineData("ValidAudience", "", "ValidKey", 1)]
    [InlineData("ValidAudience", "ValidIssuer", null, 1)]
    [InlineData("ValidAudience", "ValidIssuer", "", 1)]
    [InlineData("ValidAudience", "ValidIssuer", "ValidKey", 0)]
    [InlineData("ValidAudience", "ValidIssuer", "ValidKey", -1)]
    public void IsValid_InvalidFields_ReturnsFalse(
        string? audience,
        string? issuer,
        string? key,
        int expirationInHours)
    {
        // Arrange
        var jwtSettings = new JwtSettings
        {
            Audience = audience,
            Issuer = issuer,
            Key = key,
            ExpirationInHours = expirationInHours
        };

        // Act
        var result = jwtSettings.IsValid();

        // Assert
        Assert.False(result);
    }

    [Fact]
    [Trait("TestCategory", "UnitTest")]
    public void GetError_AllFieldsValid_ReturnsNone()
    {
        // Arrange
        var jwtSettings = new JwtSettings
        {
            Audience = "ValidAudience",
            Issuer = "ValidIssuer",
            Key = "ValidKey",
            ExpirationInHours = 1
        };

        // Act
        var result = jwtSettings.GetError();

        // Assert
        Assert.Equal(JwtSettingsConfigError.None, result);
    }

    [Fact]
    [Trait("TestCategory", "UnitTest")]
    public void IsValid_AllFieldsValid_ReturnsTrue()
    {
        // Arrange
        var jwtSettings = new JwtSettings
        {
            Audience = "ValidAudience",
            Issuer = "ValidIssuer",
            Key = "ValidKey",
            ExpirationInHours = 1
        };

        // Act
        var result = jwtSettings.IsValid();

        // Assert
        Assert.True(result);
    }
}
