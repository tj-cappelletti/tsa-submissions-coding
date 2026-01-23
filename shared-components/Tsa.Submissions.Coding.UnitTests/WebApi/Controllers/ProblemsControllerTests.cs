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
using Tsa.Submissions.Coding.Contracts.Problems;
using Tsa.Submissions.Coding.UnitTests.Data;
using Tsa.Submissions.Coding.UnitTests.Helpers;
using Tsa.Submissions.Coding.WebApi.Authorization;
using Tsa.Submissions.Coding.WebApi.Controllers;
using Tsa.Submissions.Coding.WebApi.Entities;
using Tsa.Submissions.Coding.WebApi.Services;
using Xunit;

namespace Tsa.Submissions.Coding.UnitTests.WebApi.Controllers;

[ExcludeFromCodeCoverage]
public class ProblemsControllerTests
{
    [Fact]
    [Trait("TestCategory", "UnitTest")]
    public void Controller_Public_Methods_Should_Have_Authorize_Attribute_With_Proper_Roles()
    {
        var problemsControllerType = typeof(ProblemsController);

        var methodInfos = problemsControllerType.GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly);

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
                    Assert.Equal(SubmissionRoles.All, authorizeAttribute.Roles);
                    break;

                case "GetTestSets":
                    Assert.Equal(SubmissionRoles.All, authorizeAttribute.Roles);
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
        var problemsControllerType = typeof(ProblemsController);

        var methodInfos = problemsControllerType.GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly);

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
                case "gettestsets":
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
        var problemsControllerType = typeof(ProblemsController);

        var attributes = problemsControllerType.GetCustomAttributes(typeof(ApiControllerAttribute), false);

        Assert.NotNull(attributes);
        Assert.NotEmpty(attributes);
        Assert.Single(attributes);
    }

    [Fact]
    [Trait("TestCategory", "UnitTest")]
    public void Controller_Should_Have_Produces_Attribute()
    {
        var problemsControllerType = typeof(ProblemsController);

        var attributes = problemsControllerType.GetCustomAttributes(typeof(ProducesAttribute), false);

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
        var problemsControllerType = typeof(ProblemsController);

        var attributes = problemsControllerType.GetCustomAttributes(typeof(RouteAttribute), false);

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
        var problemsTestData = new ProblemsTestData();

        var problem = problemsTestData.First(problemTestData => (ProblemDataIssues)problemTestData[1] == ProblemDataIssues.None)[0] as Problem;

        var mockedProblemsService = new Mock<IProblemsService>();
        mockedProblemsService.Setup(problemsService => problemsService.GetAsync(It.IsAny<string>(), default))
            .ReturnsAsync(problem);

        var problemsController = new ProblemsController(mockedProblemsService.Object);

        // Act
        var actionResult = await problemsController.Delete("64639f6fcdde06187b09ecae");

        // Assert
        Assert.NotNull(actionResult);
        Assert.IsType<NoContentResult>(actionResult);
    }

    [Fact]
    [Trait("TestCategory", "UnitTest")]
    public async Task Delete_Should_Return_Not_Found()
    {
        // Arrange
        var mockedProblemsService = new Mock<IProblemsService>();

        var problemsController = new ProblemsController(mockedProblemsService.Object);

        // Act
        var actionResult = await problemsController.Delete("64639f6fcdde06187b09ecae");

        // Assert
        Assert.NotNull(actionResult);
        Assert.IsType<NotFoundResult>(actionResult);
    }

    [Fact]
    [Trait("TestCategory", "UnitTest")]
    public async Task Get_By_Id_Should_Return_Not_Found()
    {
        // Arrange
        var mockedProblemsService = new Mock<IProblemsService>();

        var problemsController = new ProblemsController(mockedProblemsService.Object);

        // Act
        var actionResult = await problemsController.Get("64639f6fcdde06187b09ecae");

        // Assert
        Assert.NotNull(actionResult);
        Assert.IsType<NotFoundResult>(actionResult.Result);
    }

    [Fact]
    [Trait("TestCategory", "UnitTest")]
    public async Task Get_By_Id_Should_Return_Ok()
    {
        // Arrange
        var problemsTestData = new ProblemsTestData();

        var problem = problemsTestData.First(problemTestData => (ProblemDataIssues)problemTestData[1] == ProblemDataIssues.None)[0] as Problem;

        var mockedProblemsService = new Mock<IProblemsService>();
        mockedProblemsService.Setup(problemsService => problemsService.GetAsync(It.IsAny<string>(), default))
            .ReturnsAsync(problem);

        var problemsController = new ProblemsController(mockedProblemsService.Object);

        // Act
        var actionResult = await problemsController.Get("64639f6fcdde06187b09ecae");

        // Assert
        Assert.NotNull(actionResult);
        Assert.NotNull(actionResult.Value);
        Assert.Equal(problem!.ToResponse(), actionResult.Value, new ProblemModelEqualityComparer());
    }

    [Fact]
    [Trait("TestCategory", "UnitTest")]
    public async Task Get_By_Id_Should_Return_Ok_With_Test_Sets_Expanded_For_Judge()
    {
        // Arrange
        var problemsTestData = new ProblemsTestData();

        var problem = problemsTestData.First(problemTestData => (ProblemDataIssues)problemTestData[1] == ProblemDataIssues.None)[0] as Problem;

        var problemId = problem!.Id;

        var expectedProblemResponse = problem.ToResponse();

        var mockedProblemsService = new Mock<IProblemsService>();
        mockedProblemsService
            .Setup(problemsService => problemsService.GetAsync(It.Is(problemId, new StringEqualityComparer())!, default))
            .ReturnsAsync(problem);

        var identityMock = new Mock<IIdentity>();
        identityMock.Setup(i => i.Name).Returns("0000-000");

        var claimsPrincipalMock = new Mock<ClaimsPrincipal>();
        claimsPrincipalMock.Setup(cp => cp.Identity).Returns(identityMock.Object);
        claimsPrincipalMock.Setup(cp => cp.IsInRole(It.IsAny<string>())).Returns(false);

        var httpContext = new DefaultHttpContext
        {
            User = claimsPrincipalMock.Object
        };

        var problemsController = new ProblemsController(mockedProblemsService.Object)
        {
            ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            }
        };

        // Act
        var actionResult = await problemsController.Get(problemId!, true);

        // Assert
        Assert.NotNull(actionResult);
        Assert.NotNull(actionResult.Value);
        Assert.Equal(expectedProblemResponse, actionResult.Value, new ProblemModelEqualityComparer());
    }

    [Fact]
    [Trait("TestCategory", "UnitTest")]
    public async Task Get_By_Id_Should_Return_Ok_With_Test_Sets_Expanded_For_Participant()
    {
        // Arrange
        var problemsTestData = new ProblemsTestData();

        var problem = problemsTestData.First(problemTestData => (ProblemDataIssues)problemTestData[1] == ProblemDataIssues.None)[0] as Problem;

        var problemId = problem!.Id;

        var expectedProblemResponse = problem.ToResponse();

        var mockedProblemsService = new Mock<IProblemsService>();
        mockedProblemsService
            .Setup(problemsService => problemsService.GetAsync(It.Is(problemId, new StringEqualityComparer())!, default))
            .ReturnsAsync(problem);

        var identityMock = new Mock<IIdentity>();
        identityMock.Setup(i => i.Name).Returns("0000-000");

        var claimsPrincipalMock = new Mock<ClaimsPrincipal>();
        claimsPrincipalMock.Setup(cp => cp.Identity).Returns(identityMock.Object);
        claimsPrincipalMock.Setup(cp => cp.IsInRole(It.IsAny<string>())).Returns(true);

        var httpContext = new DefaultHttpContext
        {
            User = claimsPrincipalMock.Object
        };

        var problemsController = new ProblemsController(mockedProblemsService.Object)
        {
            ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            }
        };

        // Act
        var actionResult = await problemsController.Get(problemId!, true);

        // Assert
        Assert.NotNull(actionResult);
        Assert.NotNull(actionResult.Value);
        Assert.Equal(expectedProblemResponse, actionResult.Value, new ProblemModelEqualityComparer());
    }

    [Fact]
    [Trait("TestCategory", "UnitTest")]
    public async Task Get_Should_Return_Ok_When_Empty()
    {
        // Arrange
        var emptyProblemsList = new List<Problem>();

        var mockedProblemsService = new Mock<IProblemsService>();
        mockedProblemsService.Setup(problemsService => problemsService.GetAsync(default))
            .ReturnsAsync(emptyProblemsList);

        var problemsController = new ProblemsController(mockedProblemsService.Object);

        // Act
        var actionResult = await problemsController.Get();

        // Assert
        Assert.NotNull(actionResult);
        Assert.NotNull(actionResult.Value);
        Assert.Empty(actionResult.Value!);
    }

    [Fact]
    [Trait("TestCategory", "UnitTest")]
    public async Task Get_Should_Return_Ok_When_Not_Empty()
    {
        // Arrange
        var problemsTestData = new ProblemsTestData();

        var problemsList = problemsTestData
            .Where(problemTestData => (ProblemDataIssues)problemTestData[1] == ProblemDataIssues.None)
            .Select(problemTestData => problemTestData[0])
            .Cast<Problem>()
            .ToList();

        var mockedProblemsService = new Mock<IProblemsService>();
        mockedProblemsService.Setup(problemsService => problemsService.GetAsync(default))
            .ReturnsAsync(problemsList);

        var problemsController = new ProblemsController(mockedProblemsService.Object);

        // Act
        var actionResult = await problemsController.Get();

        // Assert
        Assert.NotNull(actionResult);
        Assert.NotNull(actionResult.Value);
        Assert.NotEmpty(actionResult.Value!);
        Assert.Equal(problemsList.Count, actionResult.Value!.Count);
        Assert.Equal(problemsList.ToResponses(), actionResult.Value, new ProblemModelEqualityComparer());
    }

    [Fact]
    [Trait("TestCategory", "UnitTest")]
    public async Task Get_Should_Return_Ok_With_Test_Sets_Expanded_For_Judge()
    {
        // Arrange
        var problemsTestData = new ProblemsTestData();

        var problemsList = problemsTestData
            .Where(problemTestData => (ProblemDataIssues)problemTestData[1] == ProblemDataIssues.None)
            .Select(problemTestData => problemTestData[0])
            .Cast<Problem>()
            .ToList();

        var expectedProblemResponses = new List<ProblemResponse>(problemsList.Count);
        expectedProblemResponses.AddRange(problemsList.Select(problem => problem.ToResponse()));

        var mockedProblemsService = new Mock<IProblemsService>();
        mockedProblemsService.Setup(problemsService => problemsService.GetAsync(default))
            .ReturnsAsync(problemsList);

        var identityMock = new Mock<IIdentity>();
        identityMock.Setup(i => i.Name).Returns("0000-000");

        var claimsPrincipalMock = new Mock<ClaimsPrincipal>();
        claimsPrincipalMock.Setup(cp => cp.Identity).Returns(identityMock.Object);
        claimsPrincipalMock.Setup(cp => cp.IsInRole(It.IsAny<string>())).Returns(false);

        var httpContext = new DefaultHttpContext
        {
            User = claimsPrincipalMock.Object
        };

        var problemsController = new ProblemsController(mockedProblemsService.Object)
        {
            ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            }
        };

        // Act
        var actionResult = await problemsController.Get(true);

        // Assert
        Assert.NotNull(actionResult);
        Assert.NotNull(actionResult.Value);
        Assert.NotEmpty(actionResult.Value!);
        Assert.Equal(expectedProblemResponses.Count, actionResult.Value!.Count);
        Assert.Equal(expectedProblemResponses, actionResult.Value, new ProblemModelEqualityComparer());
    }

    [Fact]
    [Trait("TestCategory", "UnitTest")]
    public async Task Get_Should_Return_Ok_With_Test_Sets_Expanded_For_Participant()
    {
        // Arrange
        var problemsTestData = new ProblemsTestData();

        var problemsList = problemsTestData
            .Where(problemTestData => (ProblemDataIssues)problemTestData[1] == ProblemDataIssues.None)
            .Select(problemTestData => problemTestData[0])
            .Cast<Problem>()
            .ToList();

        var expectedProblemModels = problemsList.ToResponses().ToList();

        var mockedProblemsService = new Mock<IProblemsService>();
        mockedProblemsService.Setup(problemsService => problemsService.GetAsync(default))
            .ReturnsAsync(problemsList);

        var identityMock = new Mock<IIdentity>();
        identityMock.Setup(i => i.Name).Returns("0000-000");

        var claimsPrincipalMock = new Mock<ClaimsPrincipal>();
        claimsPrincipalMock.Setup(cp => cp.Identity).Returns(identityMock.Object);
        claimsPrincipalMock.Setup(cp => cp.IsInRole(It.IsAny<string>())).Returns(true);

        var httpContext = new DefaultHttpContext
        {
            User = claimsPrincipalMock.Object
        };

        var problemsController = new ProblemsController(mockedProblemsService.Object)
        {
            ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            }
        };

        // Act
        var actionResult = await problemsController.Get(true);

        // Assert
        Assert.NotNull(actionResult);
        Assert.NotNull(actionResult.Value);
        Assert.NotEmpty(actionResult.Value!);
        Assert.Equal(expectedProblemModels.Count, actionResult.Value!.Count);
        Assert.Equal(expectedProblemModels, actionResult.Value, new ProblemModelEqualityComparer());
    }

    [Fact]
    [Trait("TestCategory", "UnitTest")]
    public async Task Post_Should_Return_Created()
    {
        // Arrange
        var newProblem = new ProblemRequest("This is the description", true, "This is the title");

        var expectedEntity = new Problem
        {
            Description = newProblem.Description,
            IsActive = newProblem.IsActive,
            Title = newProblem.Title
        };

        var mockedProblemsService = new Mock<IProblemsService>();
        mockedProblemsService
            .Setup(problemsService => problemsService.CreateAsync(It.Is(expectedEntity, new ProblemEqualityComparer()), default))
            .Callback<Problem, CancellationToken>((problem, _) =>
            {
                problem.Id = "64639f6fcdde06187b09ecae";
            })
            .Returns(Task.CompletedTask);

        var problemsController = new ProblemsController(mockedProblemsService.Object);


        // Act
        var createdAtActionResult = await problemsController.Post(newProblem);

        // Assert
        Assert.NotNull(createdAtActionResult);

        Assert.IsType<ProblemResponse>(createdAtActionResult.Value);
    }

    [Fact]
    [Trait("TestCategory", "UnitTest")]
    public async Task Put_Should_Return_No_Content()
    {
        // Arrange
        var problemsTestData = new ProblemsTestData();

        var problem = problemsTestData.First(problemTestData => (ProblemDataIssues)problemTestData[1] == ProblemDataIssues.None)[0] as Problem;

        var updatedProblem = new ProblemRequest("This is the description", true, "This is the title");

        var expectedEntity = new Problem
        {
            Id = problem!.Id,
            Description = updatedProblem.Description,
            IsActive = updatedProblem.IsActive,
            Title = updatedProblem.Title
        };

        var mockedProblemsService = new Mock<IProblemsService>();
        mockedProblemsService.Setup(problemsService => problemsService.GetAsync(It.Is(problem.Id, new StringEqualityComparer())!, default))
            .ReturnsAsync(problem);

        var problemsController = new ProblemsController(mockedProblemsService.Object);

        // Act
        var actionResult = await problemsController.Put(problem.Id!, updatedProblem);

        // Assert
        Assert.NotNull(actionResult);
        Assert.IsType<NoContentResult>(actionResult);

        mockedProblemsService.Verify(
            problemsService => problemsService.UpdateAsync(It.Is(expectedEntity, new ProblemEqualityComparer()), default), Times.Once);
    }

    [Fact]
    [Trait("TestCategory", "UnitTest")]
    public async Task Put_Should_Return_Not_Found()
    {
        // Arrange
        var mockedProblemsService = new Mock<IProblemsService>();

        var problemsController = new ProblemsController(mockedProblemsService.Object);

        var updatedProblem = new ProblemRequest("This is the description", true, "This is the title");

        // Act
        var actionResult = await problemsController.Put("64639f6fcdde06187b09ecae", updatedProblem);

        // Assert
        Assert.NotNull(actionResult);
        Assert.IsType<NotFoundResult>(actionResult);
    }
}
