using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Tsa.Submissions.Coding.WebApi.Authorization;
using Tsa.Submissions.Coding.WebApi.Controllers;
using Tsa.Submissions.Coding.WebApi.Entities;
using Tsa.Submissions.Coding.WebApi.Models;
using Tsa.Submissions.Coding.WebApi.Services;
using Xunit;

namespace Tsa.Submissions.Coding.UnitTests.WebApi.Controllers;

[ExcludeFromCodeCoverage]
public class InitializationControllerTests
{
    private readonly InitializationController _controller;
    private readonly Mock<IUsersService> _usersServiceMock;

    public InitializationControllerTests()
    {
        Mock<ILogger<InitializationController>> loggerMock = new();

        _usersServiceMock = new Mock<IUsersService>();
        _controller = new InitializationController(loggerMock.Object, _usersServiceMock.Object);
    }

    [Fact]
    [Trait("TestCategory", "UnitTest")]
    public async Task GetInitializationStatus_ReturnsOkWithStatusModel_WhenNotInitialized()
    {
        // Arrange
        _usersServiceMock.Setup(service => service.GetAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<User>());

        // Act
        var result = await _controller.GetInitializationStatus();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);

        var statusModel = Assert.IsType<InitializationStatusModel>(okResult.Value);

        Assert.False(statusModel.IsInitialized);
    }

    [Fact]
    [Trait("TestCategory", "UnitTest")]
    public async Task Initialize_ReturnsBadRequest_WhenAlreadyInitialized()
    {
        // Arrange
        _usersServiceMock.Setup(service => service.GetAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync([new User()]);

        var userModel = new UserModel
        {
            UserName = "testuser",
            Password = "password123"
        };

        // Act
        var result = await _controller.Initialize(userModel);

        // Assert
        var badRequestResult = Assert.IsType<ObjectResult>(result);

        Assert.Equal(StatusCodes.Status400BadRequest, badRequestResult.StatusCode);
        Assert.Equal("The application is already initialized", badRequestResult.Value);
    }

    [Fact]
    [Trait("TestCategory", "UnitTest")]
    public async Task Initialize_ReturnsOkWithUserModel_WhenNotInitialized()
    {
        // Arrange
        var userModel = new UserModel
        {
            UserName = "testuser",
            Password = "password123"
        };

        Func<User, bool> validateUser = user =>
            user.UserName == userModel.UserName &&
            user.Role == SubmissionRoles.Judge.ToString();

        _usersServiceMock.Setup(service => service.GetAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<User>());

        _usersServiceMock.Setup(service => service.CreateAsync(It.Is<User>(user => validateUser(user)), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask)
            .Verifiable(Times.Once);

        // Act
        var result = await _controller.Initialize(userModel);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);

        var createdUser = Assert.IsType<UserModel>(okResult.Value);

        Assert.Equal(userModel.UserName, createdUser.UserName);
    }
}
