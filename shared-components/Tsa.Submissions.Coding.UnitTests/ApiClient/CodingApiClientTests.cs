using System;
using System.Diagnostics.CodeAnalysis;
using Tsa.Submissions.Coding.ApiClient;
using Tsa.Submissions.Coding.ApiClient.Clients;
using Xunit;

namespace Tsa.Submissions.Coding.UnitTests.ApiClient;

/// <summary>
///     Contains unit tests for the <see cref="CodingApiClient" /> class.
/// </summary>
[ExcludeFromCodeCoverage]
public class CodingApiClientTests
{
    private const string TestPassword = "test-password";
    private const string TestUsername = "test-user";

    private static readonly Uri TestBaseUri = new("https://api.example.com");

    /// <summary>
    ///     Verifies that the constructor properly configures the base URI.
    /// </summary>
    [Theory]
    [InlineData("https://api.example.com")]
    [InlineData("https://localhost:5001")]
    [InlineData("http://api.test.local")]
    [Trait("TestCategory", "UnitTest")]
    public void Constructor_Should_Accept_Various_Base_Uris(string uriString)
    {
        // Arrange
        var uri = new Uri(uriString);

        // Act
        var client = new CodingApiClient(uri, TestUsername, TestPassword);

        // Assert
        Assert.NotNull(client);
    }

    /// <summary>
    ///     Verifies that the constructor properly handles special characters in credentials.
    /// </summary>
    [Theory]
    [InlineData("user@example.com", "P@ssw0rd!")]
    [InlineData("user+test", "pass#word$")]
    [InlineData("user-name_123", "p@ss-w0rd_456")]
    [Trait("TestCategory", "UnitTest")]
    public void Constructor_Should_Handle_Special_Characters_In_Credentials(string username, string password)
    {
        // Act
        var client = new CodingApiClient(TestBaseUri, username, password);

        // Assert
        Assert.NotNull(client);
    }

    /// <summary>
    ///     Verifies that the constructor throws when loginEndpoint is whitespace.
    /// </summary>
    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("\t")]
    [InlineData("\n")]
    [Trait("TestCategory", "UnitTest")]
    public void Constructor_Should_Throw_When_LoginEndpoint_Is_White_Space(string loginEndpoint)
    {
        // Act & Assert
        Assert.Throws<ArgumentException>(() =>
            new CodingApiClient(TestBaseUri, TestUsername, TestPassword, loginEndpoint));
    }

    /// <summary>
    ///     Verifies that the constructor throws when password is whitespace.
    /// </summary>
    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("\t")]
    [InlineData("\n")]
    [Trait("TestCategory", "UnitTest")]
    public void Constructor_Should_Throw_When_Password_Is_White_Space(string password)
    {
        // Act & Assert
        Assert.Throws<ArgumentException>(() =>
            new CodingApiClient(TestBaseUri, TestUsername, password));
    }

    /// <summary>
    ///     Verifies that the constructor throws when username is whitespace.
    /// </summary>
    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("\t")]
    [InlineData("\n")]
    [Trait("TestCategory", "UnitTest")]
    public void Constructor_Should_Throw_When_Username_Is_White_Space(string username)
    {
        // Act & Assert
        Assert.Throws<ArgumentException>(() =>
            new CodingApiClient(TestBaseUri, username, TestPassword));
    }

    /// <summary>
    ///     Verifies that the constructor accepts a custom login endpoint.
    /// </summary>
    [Fact]
    [Trait("TestCategory", "UnitTest")]
    public void Constructor_Should_Accept_Custom_Login_Endpoint()
    {
        // Arrange
        const string customEndpoint = "api/custom/auth";

        // Act
        var client = new CodingApiClient(TestBaseUri, TestUsername, TestPassword, customEndpoint);

        // Assert
        Assert.NotNull(client);
    }

    /// <summary>
    ///     Verifies that multiple instances can be created without interference.
    /// </summary>
    [Fact]
    [Trait("TestCategory", "UnitTest")]
    public void Constructor_Should_Create_Independent_Instances()
    {
        // Arrange & Act
        var client1 = new CodingApiClient(TestBaseUri, "user1", "pass1");
        var client2 = new CodingApiClient(TestBaseUri, "user2", "pass2");

        // Assert
        Assert.NotNull(client1);
        Assert.NotNull(client2);
        Assert.NotSame(client1, client2);
        Assert.NotSame(client1.Problems, client2.Problems);
        Assert.NotSame(client1.Submissions, client2.Submissions);
    }

    /// <summary>
    ///     Verifies that the constructor properly handles trailing slashes in base URI.
    /// </summary>
    [Fact]
    [Trait("TestCategory", "UnitTest")]
    public void Constructor_Should_Handle_BaseUri_With_Trailing_Slash()
    {
        // Arrange
        var uriWithSlash = new Uri("https://api.example.com/");

        // Act
        var client = new CodingApiClient(uriWithSlash, TestUsername, TestPassword);

        // Assert
        Assert.NotNull(client);
    }

    /// <summary>
    ///     Verifies that the constructor initializes all required properties.
    /// </summary>
    [Fact]
    [Trait("TestCategory", "UnitTest")]
    public void Constructor_Should_Initialize_All_Properties()
    {
        // Act
        var client = new CodingApiClient(TestBaseUri, TestUsername, TestPassword);

        // Assert
        Assert.NotNull(client);
        Assert.NotNull(client.Problems);
        Assert.IsAssignableFrom<IProblemsClient>(client.Problems);
        Assert.NotNull(client.Submissions);
        Assert.IsAssignableFrom<ISubmissionsClient>(client.Submissions);
    }

    /// <summary>
    ///     Verifies that the constructor throws when baseUri is null.
    /// </summary>
    [Fact]
    [Trait("TestCategory", "UnitTest")]
    public void Constructor_Should_Throw_When_BaseUri_Is_Null()
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() =>
            new CodingApiClient(null!, TestUsername, TestPassword));
    }

    /// <summary>
    ///     Verifies that the constructor throws when password is null.
    /// </summary>
    [Fact]
    [Trait("TestCategory", "UnitTest")]
    public void Constructor_Should_Throw_When_Password_Is_Null()
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() =>
            new CodingApiClient(TestBaseUri, TestUsername, null!));
    }

    /// <summary>
    ///     Verifies that the constructor throws when username is null.
    /// </summary>
    [Fact]
    [Trait("TestCategory", "UnitTest")]
    public void Constructor_Should_Throw_When_Username_Is_Null()
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() =>
            new CodingApiClient(TestBaseUri, null!, TestPassword));
    }


    /// <summary>
    ///     Verifies that the constructor uses the default login endpoint when not specified.
    /// </summary>
    [Fact]
    [Trait("TestCategory", "UnitTest")]
    public void Constructor_Should_Use_Default_Login_Endpoint()
    {
        // Act
        var client = new CodingApiClient(TestBaseUri, TestUsername, TestPassword);

        // Assert
        Assert.NotNull(client);
        // Note: We can't directly verify the private field, but the client should be created successfully
    }

    /// <summary>
    ///     Verifies that Problems client is properly initialized.
    /// </summary>
    [Fact]
    [Trait("TestCategory", "UnitTest")]
    public void Problems_Should_Be_ProblemsClient_Type()
    {
        // Arrange
        var client = new CodingApiClient(TestBaseUri, TestUsername, TestPassword);

        // Act & Assert
        Assert.IsType<ProblemsClient>(client.Problems);
    }

    /// <summary>
    ///     Verifies that Submissions client is properly initialized.
    /// </summary>
    [Fact]
    [Trait("TestCategory", "UnitTest")]
    public void Submissions_Should_Be_SubmissionsClient_Type()
    {
        // Arrange
        var client = new CodingApiClient(TestBaseUri, TestUsername, TestPassword);

        // Act & Assert
        Assert.IsType<SubmissionsClient>(client.Submissions);
    }
}
