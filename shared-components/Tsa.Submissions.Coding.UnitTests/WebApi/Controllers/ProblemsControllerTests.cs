using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Tsa.Submissions.Coding.Contracts.Problems;
using Tsa.Submissions.Coding.UnitTests.Data;
using Tsa.Submissions.Coding.UnitTests.Helpers;
using Tsa.Submissions.Coding.UnitTests.Helpers.Problems;
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

        Assert.Equal("api/problems", routeAttribute.Template);
    }

    [Fact]
    [Trait("TestCategory", "UnitTest")]
    public async Task Delete_Should_Return_No_Content()
    {
        // Arrange
        var problemsTestData = new ProblemsTestData();

        var problem = problemsTestData.First(problemTestData => (ProblemDataIssues)problemTestData[1] == ProblemDataIssues.None)[0] as Problem;

        var mockedProblemRequestValidator = new Mock<IValidator<ProblemRequest>>();

        var mockedProblemsService = new Mock<IProblemsService>();
        mockedProblemsService.Setup(problemsService => problemsService.GetAsync(It.IsAny<string>(), default))
            .ReturnsAsync(problem);

        var mockedTestCasesService = new Mock<ITestCasesService>();

        var problemsController =
            new ProblemsController(mockedProblemRequestValidator.Object, mockedProblemsService.Object, mockedTestCasesService.Object);

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
        var mockedProblemRequestValidator = new Mock<IValidator<ProblemRequest>>();
        var mockedProblemsService = new Mock<IProblemsService>();
        var mockedTestCasesService = new Mock<ITestCasesService>();

        var problemsController =
            new ProblemsController(mockedProblemRequestValidator.Object, mockedProblemsService.Object, mockedTestCasesService.Object);
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
        var mockedProblemRequestValidator = new Mock<IValidator<ProblemRequest>>();
        var mockedProblemsService = new Mock<IProblemsService>();
        var mockedTestCasesService = new Mock<ITestCasesService>();

        var problemsController =
            new ProblemsController(mockedProblemRequestValidator.Object, mockedProblemsService.Object, mockedTestCasesService.Object);
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

        var mockedProblemRequestValidator = new Mock<IValidator<ProblemRequest>>();

        var mockedProblemsService = new Mock<IProblemsService>();
        mockedProblemsService.Setup(problemsService => problemsService.GetAsync(It.IsAny<string>(), default))
            .ReturnsAsync(problem);

        var mockedTestCasesService = new Mock<ITestCasesService>();

        var problemsController =
            new ProblemsController(mockedProblemRequestValidator.Object, mockedProblemsService.Object, mockedTestCasesService.Object);

        // Act
        var actionResult = await problemsController.Get("64639f6fcdde06187b09ecae");

        // Assert
        Assert.NotNull(actionResult);
        Assert.NotNull(actionResult.Value);
        Assert.Equal(problem!.ToResponse(), actionResult.Value, new ProblemResponseEqualityComparer());
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

        var mockedProblemRequestValidator = new Mock<IValidator<ProblemRequest>>();

        var mockedProblemsService = new Mock<IProblemsService>();
        mockedProblemsService
            .Setup(problemsService => problemsService.GetAsync(It.Is(problemId, new StringEqualityComparer())!, default))
            .ReturnsAsync(problem);

        var mockedTestCasesService = new Mock<ITestCasesService>();

        var identityMock = new Mock<IIdentity>();
        identityMock.Setup(i => i.Name).Returns("0000-000");

        var claimsPrincipalMock = new Mock<ClaimsPrincipal>();
        claimsPrincipalMock.Setup(cp => cp.Identity).Returns(identityMock.Object);
        claimsPrincipalMock.Setup(cp => cp.IsInRole(It.IsAny<string>())).Returns(false);

        var httpContext = new DefaultHttpContext
        {
            User = claimsPrincipalMock.Object
        };

        var problemsController = new ProblemsController(mockedProblemRequestValidator.Object, mockedProblemsService.Object, mockedTestCasesService.Object)
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
        Assert.Equal(expectedProblemResponse, actionResult.Value, new ProblemResponseEqualityComparer());
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

        var mockedProblemRequestValidator = new Mock<IValidator<ProblemRequest>>();

        var mockedProblemsService = new Mock<IProblemsService>();
        mockedProblemsService
            .Setup(problemsService => problemsService.GetAsync(It.Is(problemId, new StringEqualityComparer())!, default))
            .ReturnsAsync(problem);

        var mockedTestCasesService = new Mock<ITestCasesService>();

        var identityMock = new Mock<IIdentity>();
        identityMock.Setup(i => i.Name).Returns("0000-000");

        var claimsPrincipalMock = new Mock<ClaimsPrincipal>();
        claimsPrincipalMock.Setup(cp => cp.Identity).Returns(identityMock.Object);
        claimsPrincipalMock.Setup(cp => cp.IsInRole(It.IsAny<string>())).Returns(true);

        var httpContext = new DefaultHttpContext
        {
            User = claimsPrincipalMock.Object
        };

        var problemsController = new ProblemsController(mockedProblemRequestValidator.Object, mockedProblemsService.Object, mockedTestCasesService.Object)
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
        Assert.Equal(expectedProblemResponse, actionResult.Value, new ProblemResponseEqualityComparer());
    }

    [Fact]
    [Trait("TestCategory", "UnitTest")]
    public async Task Get_Should_Return_Ok_When_Empty()
    {
        // Arrange
        var emptyProblemsList = new List<Problem>();

        var mockedProblemRequestValidator = new Mock<IValidator<ProblemRequest>>();

        var mockedProblemsService = new Mock<IProblemsService>();
        mockedProblemsService.Setup(problemsService => problemsService.GetAsync(default))
            .ReturnsAsync(emptyProblemsList);

        var mockedTestCasesService = new Mock<ITestCasesService>();

        var problemsController =
            new ProblemsController(mockedProblemRequestValidator.Object, mockedProblemsService.Object, mockedTestCasesService.Object);

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

        var mockedProblemRequestValidator = new Mock<IValidator<ProblemRequest>>();

        var mockedProblemsService = new Mock<IProblemsService>();
        mockedProblemsService.Setup(problemsService => problemsService.GetAsync(default))
            .ReturnsAsync(problemsList);

        var mockedTestCasesService = new Mock<ITestCasesService>();

        var problemsController =
            new ProblemsController(mockedProblemRequestValidator.Object, mockedProblemsService.Object, mockedTestCasesService.Object);

        // Act
        var actionResult = await problemsController.Get();

        // Assert
        Assert.NotNull(actionResult);
        Assert.NotNull(actionResult.Value);
        Assert.NotEmpty(actionResult.Value!);
        Assert.Equal(problemsList.Count, actionResult.Value!.Count);
        Assert.Equal(problemsList.ToResponses(), actionResult.Value, new ProblemListResponseEqualityComparer());
    }

    [Fact]
    [Trait("TestCategory", "UnitTest")]
    public async Task Post_Should_Return_Created()
    {
        // Arrange
        const string expectedDescription = "This is the description";
        const string expectedId = "64639f6fcdde06187b09ecae";
        const bool expectedIsActive = true;
        const string expectedTitle = "This is the title";

        var expectedProblemResponse = new ProblemResponse(expectedId, expectedTitle, expectedDescription, expectedIsActive);

        var newProblem = new ProblemRequest(expectedTitle, expectedDescription, expectedIsActive);

        var expectedEntity = new Problem
        {
            Description = newProblem.Description,
            IsActive = newProblem.IsActive,
            Title = newProblem.Title
        };

        var mockedProblemRequestValidator = new Mock<IValidator<ProblemRequest>>();
        mockedProblemRequestValidator
            .Setup(validator => validator.ValidateAsync(It.Is(newProblem, new ProblemRequestEqualityComparer()), default))
            .ReturnsAsync(new ValidationResult());

        var mockedProblemsService = new Mock<IProblemsService>();
        mockedProblemsService
            .Setup(problemsService => problemsService.CreateAsync(It.Is(expectedEntity, new ProblemEqualityComparer()), default))
            .Callback<Problem, CancellationToken>((problem, _) => { problem.Id = expectedId; })
            .Returns(Task.CompletedTask);

        var mockedTestCasesService = new Mock<ITestCasesService>();

        var problemsController =
            new ProblemsController(mockedProblemRequestValidator.Object, mockedProblemsService.Object, mockedTestCasesService.Object);


        // Act
        var actionResult = await problemsController.Post(newProblem);

        // Assert
        Assert.NotNull(actionResult);

        Assert.IsType<CreatedAtActionResult>(actionResult);

        // Unboxing the ActionResult to access its properties for further assertions
        var createdAtActionResult = (CreatedAtActionResult)actionResult;
        Assert.Equal("Get", createdAtActionResult.ActionName);
        Assert.Equal("64639f6fcdde06187b09ecae", createdAtActionResult.RouteValues!["id"]);
        Assert.NotNull(createdAtActionResult.Value);
        Assert.IsType<ProblemResponse>(createdAtActionResult.Value);

        var problemResponse = (ProblemResponse)createdAtActionResult.Value;
        Assert.Equal(expectedProblemResponse, problemResponse, new ProblemResponseEqualityComparer());
    }

    [Fact]
    [Trait("TestCategory", "UnitTest")]
    public async Task Put_Should_Return_No_Content()
    {
        // Arrange
        var problemsTestData = new ProblemsTestData();

        var problem = problemsTestData.First(problemTestData => (ProblemDataIssues)problemTestData[1] == ProblemDataIssues.None)[0] as Problem;

        var updatedProblem = new ProblemRequest("This is the title", "This is the description", true);

        var expectedEntity = new Problem
        {
            Id = problem!.Id,
            Description = updatedProblem.Description,
            IsActive = updatedProblem.IsActive,
            Title = updatedProblem.Title
        };

        var mockedProblemRequestValidator = new Mock<IValidator<ProblemRequest>>();
        mockedProblemRequestValidator
            .Setup(validator => validator.ValidateAsync(It.Is(updatedProblem, new ProblemRequestEqualityComparer()), default))
            .ReturnsAsync(new ValidationResult());

        var mockedProblemsService = new Mock<IProblemsService>();
        mockedProblemsService.Setup(problemsService => problemsService.GetAsync(It.Is(problem.Id, new StringEqualityComparer())!, default))
            .ReturnsAsync(problem);

        var mockedTestCasesService = new Mock<ITestCasesService>();

        var problemsController =
            new ProblemsController(mockedProblemRequestValidator.Object, mockedProblemsService.Object, mockedTestCasesService.Object);

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
        var updatedProblem = new ProblemRequest("This is the title", "This is the description", true);

        var mockedProblemRequestValidator = new Mock<IValidator<ProblemRequest>>();
        mockedProblemRequestValidator
            .Setup(validator => validator.ValidateAsync(It.Is(updatedProblem, new ProblemRequestEqualityComparer()), default))
            .ReturnsAsync(new ValidationResult());

        var mockedProblemsService = new Mock<IProblemsService>();
        var mockedTestCasesService = new Mock<ITestCasesService>();

        var problemsController =
            new ProblemsController(mockedProblemRequestValidator.Object, mockedProblemsService.Object, mockedTestCasesService.Object);

        // Act
        var actionResult = await problemsController.Put("64639f6fcdde06187b09ecae", updatedProblem);

        // Assert
        Assert.NotNull(actionResult);
        Assert.IsType<NotFoundResult>(actionResult);
    }
}
