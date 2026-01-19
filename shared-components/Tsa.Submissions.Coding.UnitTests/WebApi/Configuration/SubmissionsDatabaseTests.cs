using System.Diagnostics.CodeAnalysis;
using Tsa.Submissions.Coding.WebApi.Configuration;
using Xunit;

namespace Tsa.Submissions.Coding.UnitTests.WebApi.Configuration;

[ExcludeFromCodeCoverage]
public class SubmissionsDatabaseTests
{
    [Theory]
    [Trait("TestCategory", "UnitTest")]
    [InlineData(SubmissionsDatabaseProperties.Host)]
    [InlineData(SubmissionsDatabaseProperties.LoginDatabase)]
    [InlineData(SubmissionsDatabaseProperties.Name)]
    [InlineData(SubmissionsDatabaseProperties.Password)]
    [InlineData(SubmissionsDatabaseProperties.Port)]
    [InlineData(SubmissionsDatabaseProperties.Username)]
    public void IsValid_Should_Return_False_When_Invalid_Property(SubmissionsDatabaseProperties property)
    {
        // Arrange
        var database = new SubmissionsDatabase();

        if (property != SubmissionsDatabaseProperties.Host)
        {
            database.Host = "localhost";
        }

        if (property != SubmissionsDatabaseProperties.LoginDatabase)
        {
            database.LoginDatabase = "login_db";
        }

        if (property != SubmissionsDatabaseProperties.Name)
        {
            database.Name = "db_name";
        }

        if (property != SubmissionsDatabaseProperties.Password)
        {
            database.Password = "password";
        }

        database.Port = property != SubmissionsDatabaseProperties.Port ? 5432 : 0; // Set to an invalid port

        if (property != SubmissionsDatabaseProperties.Username)
        {
            database.Username = "username";
        }

        // Act
        var result = database.IsValid();

        // Assert
        Assert.False(result);
    }

    public enum SubmissionsDatabaseProperties
    {
        Host,
        LoginDatabase,
        Name,
        Password,
        Port,
        Username
    }

    [Fact]
    [Trait("TestCategory", "UnitTest")]
    public void IsValid_Should_Return_True()
    {
        // Arrange
        var database = new SubmissionsDatabase
        {
            Host = "localhost",
            LoginDatabase = "login_db",
            Name = "db_name",
            Password = "password",
            Port = 5432,
            Username = "username"
        };

        // Act
        var result = database.IsValid();

        // Assert
        Assert.True(result);
    }
}
