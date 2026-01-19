using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Tsa.Submissions.Coding.UnitTests.Data;
using Tsa.Submissions.Coding.UnitTests.Helpers;
using Tsa.Submissions.Coding.WebApi.Authorization;
using Tsa.Submissions.Coding.WebApi.Controllers;
using Tsa.Submissions.Coding.WebApi.Entities;
using Tsa.Submissions.Coding.WebApi.Models;
using Tsa.Submissions.Coding.WebApi.Services;
using Xunit;

namespace Tsa.Submissions.Coding.UnitTests.WebApi.Controllers;

[ExcludeFromCodeCoverage]
public class UsersControllerTests
{
    [Fact]
    [Trait("TestCategory", "UnitTest")]
    public void Controller_Public_Methods_Should_Have_Authorize_Attribute_With_Proper_Roles()
    {
        var usersControllerType = typeof(UsersController);

        var methodInfos = usersControllerType.GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly);

        Assert.NotEmpty(methodInfos);

        foreach (var methodInfo in methodInfos)
        {
            var attributes = methodInfo.GetCustomAttributes(typeof(AuthorizeAttribute), false);

            Assert.NotNull(attributes);
            Assert.NotEmpty(attributes);
            Assert.Single(attributes);

            var authorizeAttribute = (AuthorizeAttribute)attributes[0];

            switch (methodInfo.Name)
            {
                case "Delete":
                    Assert.Equal(SubmissionRoles.Judge, authorizeAttribute.Roles);
                    break;

                case "Get":
                    Assert.Equal(methodInfo.GetParameters().Length == 1 ? SubmissionRoles.Judge : SubmissionRoles.All, authorizeAttribute.Roles);
                    break;

                case "Post":
                    Assert.Equal(SubmissionRoles.Judge, authorizeAttribute.Roles);
                    break;

                case "Put":
                    Assert.Equal(SubmissionRoles.Judge, authorizeAttribute.Roles);
                    break;

                default:
                    Assert.Fail($"A test case for the method `{methodInfo.Name}` does not exist");
                    break;
            }
        }
    }

    [Fact]
    [Trait("TestCategory", "UnitTest")]
    public void Controller_Public_Methods_Should_Have_Http_Method_Attribute()
    {
        var usersControllerType = typeof(UsersController);

        var methodInfos = usersControllerType.GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly);

        Assert.NotEmpty(methodInfos);

        foreach (var methodInfo in methodInfos)
        {
            // Needs to be nullable so the compiler sees it's initialized
            // The Assert.Fail doesn't tell it that the line it's being used
            // will only ever be hit if it's initialized
            Type? attributeType = null;

            switch (methodInfo.Name.ToLower())
            {
                case "delete":
                    attributeType = typeof(HttpDeleteAttribute);
                    break;
                case "get":
                    attributeType = typeof(HttpGetAttribute);
                    break;
                case "head":
                    attributeType = typeof(HttpHeadAttribute);
                    break;
                case "options":
                    attributeType = typeof(HttpOptionsAttribute);
                    break;
                case "patch":
                    attributeType = typeof(HttpPatchAttribute);
                    break;
                case "post":
                    attributeType = typeof(HttpPostAttribute);
                    break;
                case "put":
                    attributeType = typeof(HttpPutAttribute);
                    break;
                default:
                    Assert.Fail("Unsupported public HTTP operation");
                    break;
            }

            var attributes = methodInfo.GetCustomAttributes(attributeType, false);

            Assert.NotNull(attributes);
            Assert.NotEmpty(attributes);
            Assert.Single(attributes);
        }
    }

    [Fact]
    [Trait("TestCategory", "UnitTest")]
    public void Controller_Should_Have_ApiController_Attribute()
    {
        var usersControllerType = typeof(UsersController);

        var attributes = usersControllerType.GetCustomAttributes(typeof(ApiControllerAttribute), false);

        Assert.NotNull(attributes);
        Assert.NotEmpty(attributes);
        Assert.Single(attributes);
    }

    [Fact]
    [Trait("TestCategory", "UnitTest")]
    public void Controller_Should_Have_Produces_Attribute()
    {
        var usersControllerType = typeof(UsersController);

        var attributes = usersControllerType.GetCustomAttributes(typeof(ProducesAttribute), false);

        Assert.NotNull(attributes);
        Assert.NotEmpty(attributes);
        Assert.Single(attributes);

        var producesAttribute = (ProducesAttribute)attributes[0];

        Assert.Contains("application/json", producesAttribute.ContentTypes);
    }

    [Fact]
    [Trait("TestCategory", "UnitTest")]
    public void Controller_Should_Have_Route_Attribute()
    {
        var usersControllerType = typeof(UsersController);

        var attributes = usersControllerType.GetCustomAttributes(typeof(RouteAttribute), false);

        Assert.NotNull(attributes);
        Assert.NotEmpty(attributes);
        Assert.Single(attributes);

        var routeAttribute = (RouteAttribute)attributes[0];

        Assert.Equal("api/[controller]", routeAttribute.Template);
    }

    [Fact]
    [Trait("TestCategory", "UnitTest")]
    public async Task Delete_Should_Return_No_Content()
    {
        // Arrange
        var usersTestData = new UsersTestData();

        var user = (User)usersTestData.First(userTestData => (UserDataIssues)userTestData[1] == UserDataIssues.None)[0];

        var mockedCacheService = new Mock<ICacheService>();
        var mockedUsersService = new Mock<IUsersService>();
        mockedUsersService.Setup(usersService => usersService.GetAsync(It.Is(user.Id!, new StringEqualityComparer()), default))
            .ReturnsAsync(user);

        var usersController = new UsersController(mockedCacheService.Object, mockedUsersService.Object);

        // Act
        var actionResult = await usersController.Delete(user.Id!);

        // Assert
        Assert.NotNull(actionResult);
        Assert.IsType<NoContentResult>(actionResult);
    }

    [Fact]
    [Trait("TestCategory", "UnitTest")]
    public async Task Delete_Should_Return_Not_Found()
    {
        // Arrange
        var mockedCacheService = new Mock<ICacheService>();
        var mockedUsersService = new Mock<IUsersService>();

        var usersController = new UsersController(mockedCacheService.Object, mockedUsersService.Object);

        // Act
        var actionResult = await usersController.Delete("64639f6fcdde06187b09ecae");

        // Assert
        Assert.NotNull(actionResult);
        Assert.IsType<NotFoundObjectResult>(actionResult);
    }

    [Fact]
    [Trait("TestCategory", "UnitTest")]
    public async Task Get_By_Id_Should_Return_Not_Found()
    {
        // Arrange
        var mockedCacheService = new Mock<ICacheService>();
        var mockedUsersService = new Mock<IUsersService>();

        var usersController = new UsersController(mockedCacheService.Object, mockedUsersService.Object);

        // Act
        var actionResult = await usersController.Get("64639f6fcdde06187b09ecae");

        // Assert
        Assert.NotNull(actionResult);
        Assert.IsType<NotFoundObjectResult>(actionResult);
    }

    [Fact]
    [Trait("TestCategory", "UnitTest")]
    public async Task Get_By_Id_Should_Return_Not_Found_For_Participant_Role()
    {
        // Arrange
        var usersTestData = new UsersTestData();

        var user = usersTestData.First(userTestData => (UserDataIssues)userTestData[1] == UserDataIssues.None)[0] as User;

        var mockedCacheService = new Mock<ICacheService>();
        var mockedUsersService = new Mock<IUsersService>();
        mockedUsersService.Setup(usersService => usersService.GetAsync(It.Is(user!.Id!, new StringEqualityComparer()), default))
            .ReturnsAsync(user);

        var identityMock = new Mock<IIdentity>();
        identityMock.Setup(i => i.Name).Returns("0000-000");

        var claimsPrincipalMock = new Mock<ClaimsPrincipal>();
        claimsPrincipalMock.Setup(cp => cp.Identity).Returns(identityMock.Object);
        claimsPrincipalMock.Setup(cp => cp.IsInRole(It.IsAny<string>())).Returns(true);

        var httpContext = new DefaultHttpContext
        {
            User = claimsPrincipalMock.Object
        };

        var usersController = new UsersController(mockedCacheService.Object, mockedUsersService.Object)
        {
            ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            }
        };

        // Act
        var actionResult = await usersController.Get(user!.Id!);

        // Assert
        Assert.NotNull(actionResult);
        Assert.IsType<NotFoundObjectResult>(actionResult);
    }

    [Fact]
    [Trait("TestCategory", "UnitTest")]
    public async Task Get_By_Id_Should_Return_Ok_For_Judge_Role()
    {
        // Arrange
        var usersTestData = new UsersTestData();

        var user = usersTestData.First(userTestData => (UserDataIssues)userTestData[1] == UserDataIssues.None)[0] as User;

        var expectedUserModel = user!.ToModel();

        var mockedCacheService = new Mock<ICacheService>();
        var mockedUsersService = new Mock<IUsersService>();
        mockedUsersService.Setup(usersService => usersService.GetAsync(It.Is(user!.Id!, new StringEqualityComparer()), default))
            .ReturnsAsync(user);

        var identityMock = new Mock<IIdentity>();
        identityMock.Setup(i => i.Name).Returns("judge01");

        var claimsPrincipalMock = new Mock<ClaimsPrincipal>();
        claimsPrincipalMock.Setup(cp => cp.Identity).Returns(identityMock.Object);
        claimsPrincipalMock.Setup(cp => cp.IsInRole(It.Is(SubmissionRoles.Participant, new StringEqualityComparer()))).Returns(false);

        var httpContext = new DefaultHttpContext
        {
            User = claimsPrincipalMock.Object
        };

        var usersController = new UsersController(mockedCacheService.Object, mockedUsersService.Object)
        {
            ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            }
        };

        // Act
        var actionResult = await usersController.Get(user!.Id!);

        // Assert
        Assert.NotNull(actionResult);
        Assert.IsType<OkObjectResult>(actionResult);

        var okObjectResult = (OkObjectResult)actionResult;

        Assert.Equal(expectedUserModel, okObjectResult.Value as UserModel, new UserModelEqualityComparer());
    }

    [Fact]
    [Trait("TestCategory", "UnitTest")]
    public async Task Get_By_Id_Should_Return_Ok_For_Participant_Role()
    {
        // Arrange
        var usersTestData = new UsersTestData();

        var user = (User)usersTestData.First(userTestData => (UserDataIssues)userTestData[1] == UserDataIssues.None)[0];

        var expectedUserModel = user.ToModel();

        var mockedCacheService = new Mock<ICacheService>();
        var mockedUsersService = new Mock<IUsersService>();
        mockedUsersService.Setup(usersService => usersService.GetAsync(It.Is(user.Id!, new StringEqualityComparer()), default))
            .ReturnsAsync(user);

        var identityMock = new Mock<IIdentity>();
        identityMock.Setup(i => i.Name).Returns(user.UserName);

        var claimsPrincipalMock = new Mock<ClaimsPrincipal>();
        claimsPrincipalMock.Setup(cp => cp.Identity).Returns(identityMock.Object);
        claimsPrincipalMock.Setup(cp => cp.IsInRole(It.Is(SubmissionRoles.Participant, new StringEqualityComparer()))).Returns(true);

        var httpContext = new DefaultHttpContext
        {
            User = claimsPrincipalMock.Object
        };

        var usersController = new UsersController(mockedCacheService.Object, mockedUsersService.Object)
        {
            ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            }
        };

        // Act
        var actionResult = await usersController.Get(user.Id!);

        // Assert
        Assert.NotNull(actionResult);
        Assert.IsType<OkObjectResult>(actionResult);

        var okObjectResult = (OkObjectResult)actionResult;

        Assert.Equal(expectedUserModel, okObjectResult.Value as UserModel, new UserModelEqualityComparer());
    }

    [Fact]
    [Trait("TestCategory", "UnitTest")]
    public async Task Get_Should_Return_Ok_When_Empty()
    {
        // Arrange
        var emptyUsersList = new List<User>();

        var mockedCacheService = new Mock<ICacheService>();
        var mockedUsersService = new Mock<IUsersService>();
        mockedUsersService.Setup(usersService => usersService.GetAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(emptyUsersList);

        var usersController = new UsersController(mockedCacheService.Object, mockedUsersService.Object);

        // Act
        var actionResult = await usersController.Get();

        // Assert
        Assert.NotNull(actionResult);
        Assert.IsType<OkObjectResult>(actionResult);

        var okObjectResult = (OkObjectResult)actionResult;

        Assert.NotNull(okObjectResult.Value);
        Assert.IsType<List<UserModel>>(okObjectResult.Value);
        Assert.Empty((List<UserModel>)okObjectResult.Value);
    }

    [Fact]
    [Trait("TestCategory", "UnitTest")]
    public async Task Get_Should_Return_Ok_When_Not_Empty()
    {
        // Arrange
        var usersTestData = new UsersTestData();

        var users = usersTestData.Where(userTestData => (UserDataIssues)userTestData[1] == UserDataIssues.None)
            .Select(userTestData => (User)userTestData[0])
            .ToList();

        var mockedCacheService = new Mock<ICacheService>();
        var mockedUsersService = new Mock<IUsersService>();
        mockedUsersService.Setup(usersService => usersService.GetAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(users);

        var expectedUsers = users.ToModels();

        var usersController = new UsersController(mockedCacheService.Object, mockedUsersService.Object);

        // Act
        var actionResult = await usersController.Get();

        // Assert
        Assert.NotNull(actionResult);
        Assert.IsType<OkObjectResult>(actionResult);

        var okObjectResult = (OkObjectResult)actionResult;

        Assert.NotNull(okObjectResult.Value);
        Assert.IsType<List<UserModel>>(okObjectResult.Value);

        var actualUsers = (List<UserModel>)okObjectResult.Value;

        Assert.Equal(expectedUsers, actualUsers, new UserModelEqualityComparer());
    }

    [Fact]
    [Trait("TestCategory", "UnitTest")]
    public async Task Post_Batch_Should_Return_Created()
    {
        // Arrange
        var usersTestData = new UsersTestData();

        var expectedUsers = usersTestData
            .Where(userTestData => (UserDataIssues)userTestData[1] == UserDataIssues.None && ((User)userTestData[0]).Team != null)
            .Select(userTestData => (User)userTestData[0])
            .ToList();

        var mockedCacheService = new Mock<ICacheService>();

        var mockedUsersService = new Mock<IUsersService>();

        var userModels = new List<UserModel>();

        foreach (var user in expectedUsers)
        {
            mockedUsersService.Setup(usersService => usersService.CreateAsync(It.Is(user, new UserEqualityComparer(true)), It.IsAny<CancellationToken>()))
                .Callback((User createdUser, CancellationToken _) => createdUser.Id = user.Id);

            var userModel = user.ToModel();
            userModel.Id = null;
            userModel.Password = "user'spassword";
            userModel.Team = new TeamModel
            {
                CompetitionLevel = user.Team!.CompetitionLevel.ToString(),
                SchoolNumber = user.Team.SchoolNumber,
                TeamNumber = user.Team.TeamNumber
            };
            userModels.Add(userModel);
        }

        var usersController = new UsersController(mockedCacheService.Object, mockedUsersService.Object);

        // Act
        var actionResult = await usersController.Post(userModels.ToArray());

        // Assert
        Assert.NotNull(actionResult);
        Assert.IsType<CreatedResult>(actionResult);

        var createdAtActionResult = (CreatedResult)actionResult;

        Assert.NotNull(createdAtActionResult.Value);
        Assert.IsType<BatchOperationModel<UserModel>>(createdAtActionResult.Value);

        var batchOperation = (BatchOperationModel<UserModel>)createdAtActionResult.Value;

        Assert.Equal(BatchOperationResult.Success.ToString(), batchOperation.Result, new StringEqualityComparer());

        Assert.NotEmpty(batchOperation.CreatedItems);
        Assert.Empty(batchOperation.DeletedItems);
        Assert.Empty(batchOperation.FailedItems);

        var actualUsers = batchOperation.CreatedItems;

        Assert.Equal(expectedUsers.ToModels(), actualUsers, new UserModelEqualityComparer());
    }

    [Fact]
    [Trait("TestCategory", "UnitTest")]
    public async Task Post_Should_Return_Conflict_When_User_Already_Exists()
    {
        // Arrange
        var usersTestData = new UsersTestData();
        var expectedUser = (User)usersTestData.First(userTestData =>
            (UserDataIssues)userTestData[1] == UserDataIssues.None && ((User)userTestData[0]).Role == SubmissionRoles.Participant)[0];

        var expectedApiErrorResponseModel = ApiErrorResponseModel.EntityAlreadyExists(nameof(User), expectedUser.Id!);

        var userModel = expectedUser.ToModel();
        userModel.Id = null;

        var mockedCacheService = new Mock<ICacheService>();
        var mockedUsersService = new Mock<IUsersService>();
        mockedUsersService.Setup(usersService =>
                usersService.GetByUserNameAsync(It.Is(expectedUser.UserName, new StringEqualityComparer()), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedUser)
            .Verifiable(Times.Once);

        mockedUsersService.Setup(usersService =>
                usersService.CreateAsync(It.Is(expectedUser, new UserEqualityComparer(true)), It.IsAny<CancellationToken>()))
            .Callback((User user, CancellationToken _) => user.Id = expectedUser.Id);

        var usersController = new UsersController(mockedCacheService.Object, mockedUsersService.Object);

        // Act
        var actionResult = await usersController.Post(userModel);

        // Assert
        Assert.NotNull(actionResult);
        Assert.IsType<ConflictObjectResult>(actionResult);

        var conflictObjectResult = (ConflictObjectResult)actionResult;
        Assert.IsType<ApiErrorResponseModel>(conflictObjectResult.Value);

        var actualApiErrorResponseModel = (ApiErrorResponseModel)conflictObjectResult.Value;
        Assert.Equal(expectedApiErrorResponseModel, actualApiErrorResponseModel, new ApiErrorResponseModelEqualityComparer());
    }

    [Fact]
    [Trait("TestCategory", "UnitTest")]
    public async Task Post_Should_Return_Created_Participant_By_School()
    {
        // Arrange
        var usersTestData = new UsersTestData();

        var expectedUser = (User)usersTestData.First(userTestData =>
            (UserDataIssues)userTestData[1] == UserDataIssues.None && ((User)userTestData[0]).Role == SubmissionRoles.Participant)[0];

        var userModel = expectedUser.ToModel();
        userModel.Id = null;
        userModel.Password = "user'spassword";
        userModel.Team = new TeamModel
        {
            CompetitionLevel = expectedUser.Team!.CompetitionLevel.ToString(),
            SchoolNumber = expectedUser.Team.SchoolNumber,
            TeamNumber = expectedUser.Team.TeamNumber
        };

        var mockedCacheService = new Mock<ICacheService>();

        var mockedUsersService = new Mock<IUsersService>();
        mockedUsersService.Setup(usersService =>
                usersService.CreateAsync(It.Is(expectedUser, new UserEqualityComparer(true)), It.IsAny<CancellationToken>()))
            .Callback((User user, CancellationToken _) => user.Id = expectedUser.Id);

        var usersController = new UsersController(mockedCacheService.Object, mockedUsersService.Object);

        // Act
        var actionResult = await usersController.Post(userModel);

        // Assert
        Assert.NotNull(actionResult);
        Assert.IsType<CreatedAtActionResult>(actionResult);

        var createdAtActionResult = (CreatedAtActionResult)actionResult;

        Assert.Equal("Get", createdAtActionResult.ActionName);
        Assert.NotNull(createdAtActionResult.RouteValues);
        Assert.True(createdAtActionResult.RouteValues.ContainsKey("id"));
        Assert.Equal(expectedUser.Id, createdAtActionResult.RouteValues["id"]);
        Assert.Equal(expectedUser.ToModel(), createdAtActionResult.Value as UserModel, new UserModelEqualityComparer());
    }

    [Fact]
    [Trait("TestCategory", "UnitTest")]
    public async Task Post_Should_Return_Created_Participant_By_TeamId()
    {
        // Arrange
        var usersTestData = new UsersTestData();

        var expectedUser = (User)usersTestData.First(userTestData =>
            (UserDataIssues)userTestData[1] == UserDataIssues.None && ((User)userTestData[0]).Role == SubmissionRoles.Participant)[0];

        var userModel = expectedUser.ToModel();
        userModel.Id = null;
        userModel.Password = "user'spassword";
        userModel.Team = new TeamModel
        {
            CompetitionLevel = expectedUser.Team!.CompetitionLevel.ToString(),
            SchoolNumber = expectedUser.Team.SchoolNumber,
            TeamNumber = expectedUser.Team.TeamNumber
        };

        var mockedCacheService = new Mock<ICacheService>();

        var mockedUsersService = new Mock<IUsersService>();
        mockedUsersService.Setup(usersService =>
                usersService.CreateAsync(It.Is(expectedUser, new UserEqualityComparer(true)), It.IsAny<CancellationToken>()))
            .Callback((User user, CancellationToken _) => user.Id = expectedUser.Id);

        var usersController = new UsersController(mockedCacheService.Object, mockedUsersService.Object);

        // Act
        var actionResult = await usersController.Post(userModel);

        // Assert
        Assert.NotNull(actionResult);
        Assert.IsType<CreatedAtActionResult>(actionResult);

        var createdAtActionResult = (CreatedAtActionResult)actionResult;

        Assert.Equal("Get", createdAtActionResult.ActionName);
        Assert.NotNull(createdAtActionResult.RouteValues);
        Assert.True(createdAtActionResult.RouteValues.ContainsKey("id"));
        Assert.Equal(expectedUser.Id, createdAtActionResult.RouteValues["id"]);
        Assert.Equal(expectedUser.ToModel(), createdAtActionResult.Value as UserModel, new UserModelEqualityComparer());
    }

    [Fact]
    [Trait("TestCategory", "UnitTest")]
    public async Task Put_Should_Return_NoContent_By_School()
    {
        // Arrange
        var usersTestData = new UsersTestData();

        var expectedUser = (User)usersTestData.First(userTestData =>
            (UserDataIssues)userTestData[1] == UserDataIssues.None && ((User)userTestData[0]).Role == SubmissionRoles.Participant)[0];

        var userModel = expectedUser.ToModel();
        userModel.Id = null;

        var mockedCacheService = new Mock<ICacheService>();

        var mockedUsersService = new Mock<IUsersService>();
        mockedUsersService.Setup(usersService => usersService.GetAsync(It.Is(userModel.Id!, new StringEqualityComparer()), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedUser);
        mockedUsersService.Setup(usersService =>
                usersService.UpdateAsync(It.Is(expectedUser, new UserEqualityComparer(true)), It.IsAny<CancellationToken>()))
            .Callback((User user, CancellationToken _) => user.Id = expectedUser.Id);

        var usersController = new UsersController(mockedCacheService.Object, mockedUsersService.Object);

        // Act
        var actionResult = await usersController.Put(userModel.Id!, userModel);

        // Assert
        Assert.NotNull(actionResult);
        Assert.IsType<NoContentResult>(actionResult);
    }

    [Fact]
    [Trait("TestCategory", "UnitTest")]
    public async Task Put_Should_Return_NoContent_By_TeamId()
    {
        // Arrange
        var usersTestData = new UsersTestData();

        var expectedUser = (User)usersTestData.First(userTestData =>
            (UserDataIssues)userTestData[1] == UserDataIssues.None && ((User)userTestData[0]).Role == SubmissionRoles.Participant)[0];

        var userModel = expectedUser.ToModel();

        var mockedCacheService = new Mock<ICacheService>();

        var mockedUsersService = new Mock<IUsersService>();
        mockedUsersService.Setup(usersService => usersService.GetAsync(It.Is(userModel.Id!, new StringEqualityComparer()), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedUser);
        mockedUsersService.Setup(usersService =>
                usersService.UpdateAsync(It.Is(expectedUser, new UserEqualityComparer(true)), It.IsAny<CancellationToken>()))
            .Verifiable(Times.Once);

        var usersController = new UsersController(mockedCacheService.Object, mockedUsersService.Object);

        // Act
        var actionResult = await usersController.Put(userModel.Id!, userModel);

        // Assert
        Assert.NotNull(actionResult);
        Assert.IsType<NoContentResult>(actionResult);
    }
}
