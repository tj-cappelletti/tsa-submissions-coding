using System.Diagnostics.CodeAnalysis;
using System.Net;
using Tsa.Submissions.Coding.WebApi.Exceptions;
using Xunit;

namespace Tsa.Submissions.Coding.UnitTests.WebApi.Exceptions;

[ExcludeFromCodeCoverage]
public class RequiredEntityNotFoundExceptionTests
{
    [Fact]
    [Trait("TestCategory", "UnitTest")]
    public void Constructor_Should_Set_Attributes()
    {
        // Arrange
        var entityName = "SampleEntity";

        // Act
        var requiredEntityNotFoundException = new RequiredEntityNotFoundException(entityName);

        // Assert
        Assert.Equal(entityName, requiredEntityNotFoundException.EntityName);
        Assert.Equal(HttpStatusCode.NotFound, requiredEntityNotFoundException.HttpStatusCode);
    }

    [Fact]
    [Trait("TestCategory", "UnitTest")]
    public void ToApiErrorResponseModel_Should_Return_ApiErrorResponseModel()
    {
        // Arrange
        var errorCode = 2000;
        var entityName = "SampleEntity";
        var expectedMessage = $"Could not locate required resource for the `{entityName}` entity.";

        var requiredEntityNotFoundException = new RequiredEntityNotFoundException(entityName);

        // Act
        var apiErrorResponseModel = requiredEntityNotFoundException.ToApiErrorResponseModel();

        // Assert
        Assert.Equal(errorCode, apiErrorResponseModel.ErrorCode);
        Assert.Equal(expectedMessage, apiErrorResponseModel.Message);
    }
}
