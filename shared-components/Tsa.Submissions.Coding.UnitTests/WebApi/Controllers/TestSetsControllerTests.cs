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
using Tsa.Submissions.Coding.WebApi.Exceptions;
using Tsa.Submissions.Coding.WebApi.Models;
using Tsa.Submissions.Coding.WebApi.Services;
using Xunit;

namespace Tsa.Submissions.Coding.UnitTests.WebApi.Controllers;

[ExcludeFromCodeCoverage]
public class TestSetsControllerTests
{
    [Fact]
    [Trait("TestCategory", "UnitTest")]
    public void Controller_Public_Methods_Should_Have_Authorize_Attribute_With_Proper_Roles()
    {
        var testSetsController = typeof(TestSetsController);

        var methodInfos = testSetsController.GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly);

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
        var testSetsController = typeof(TestSetsController);

        var methodInfos = testSetsController.GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly);

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
        var testSetsController = typeof(TestSetsController);

        var attributes = testSetsController.GetCustomAttributes(typeof(ApiControllerAttribute), false);

        Assert.NotNull(attributes);
        Assert.NotEmpty(attributes);
        Assert.Single(attributes);
    }

    [Fact]
    [Trait("TestCategory", "UnitTest")]
    public void Controller_Should_Have_Produces_Attribute()
    {
        var testSetsController = typeof(TestSetsController);

        var attributes = testSetsController.GetCustomAttributes(typeof(ProducesAttribute), false);

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
        var testSetsController = typeof(TestSetsController);

        var attributes = testSetsController.GetCustomAttributes(typeof(RouteAttribute), false);

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
        var testSetsTestData = new TestSetsTestData();

        var testSet = testSetsTestData.First(_ => (TestSetDataIssues)_[1] == TestSetDataIssues.None)[0] as TestSet;

        var mockedProblemsService = new Mock<IProblemsService>();

        var mockedTestSetsService = new Mock<ITestSetsService>();
        mockedTestSetsService
            .Setup(_ => _.GetAsync(It.IsAny<string>(), default))
            .ReturnsAsync(testSet);

        var problemsController = new TestSetsController(mockedProblemsService.Object, mockedTestSetsService.Object);

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

        var mockedTestSetsService = new Mock<ITestSetsService>();

        var problemsController = new TestSetsController(mockedProblemsService.Object, mockedTestSetsService.Object);

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

        var mockedTestSetsService = new Mock<ITestSetsService>();

        var problemsController = new TestSetsController(mockedProblemsService.Object, mockedTestSetsService.Object);

        // Act
        var actionResult = await problemsController.Get("64639f6fcdde06187b09ecae");

        // Assert
        Assert.NotNull(actionResult);
        Assert.IsType<NotFoundResult>(actionResult.Result);
    }

    [Fact]
    [Trait("TestCategory", "UnitTest")]
    public async Task Get_By_Id_Should_Return_Not_Found_For_Participant_When_Not_Is_Public()
    {
        // Arrange
        var testSetsTestData = new TestSetsTestData();

        var testSet = testSetsTestData.First(_ =>
            !((TestSet)_[0]).IsPublic &&
            (TestSetDataIssues)_[1] == TestSetDataIssues.None)[0] as TestSet;

        var mockedProblemsService = new Mock<IProblemsService>();

        var mockedTestSetsService = new Mock<ITestSetsService>();
        mockedTestSetsService.Setup(_ => _.GetAsync(It.IsAny<string>(), default))
            .ReturnsAsync(testSet);

        var identityMock = new Mock<IIdentity>();
        identityMock.Setup(i => i.Name).Returns("0000-000");

        var claimsPrincipalMock = new Mock<ClaimsPrincipal>();
        claimsPrincipalMock.Setup(cp => cp.Identity).Returns(identityMock.Object);
        claimsPrincipalMock.Setup(cp => cp.IsInRole(It.IsAny<string>())).Returns(true);

        var httpContext = new DefaultHttpContext
        {
            User = claimsPrincipalMock.Object
        };

        var problemsController = new TestSetsController(mockedProblemsService.Object, mockedTestSetsService.Object)
        {
            ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            }
        };

        // Act
        var actionResult = await problemsController.Get("64639f6fcdde06187b09ecae");

        // Assert
        Assert.NotNull(actionResult.Result as NotFoundResult);
    }

    [Fact]
    [Trait("TestCategory", "UnitTest")]
    public async Task Get_By_Id_Should_Return_Ok_For_Judge()
    {
        // Arrange
        var testSetsTestData = new TestSetsTestData();

        var testSet = testSetsTestData.First(_ => (TestSetDataIssues)_[1] == TestSetDataIssues.None)[0] as TestSet;

        var mockedProblemsService = new Mock<IProblemsService>();

        var mockedTestSetsService = new Mock<ITestSetsService>();
        mockedTestSetsService.Setup(_ => _.GetAsync(It.IsAny<string>(), default))
            .ReturnsAsync(testSet);

        var identityMock = new Mock<IIdentity>();
        identityMock.Setup(i => i.Name).Returns("0000-000");

        var claimsPrincipalMock = new Mock<ClaimsPrincipal>();
        claimsPrincipalMock.Setup(cp => cp.Identity).Returns(identityMock.Object);
        claimsPrincipalMock.Setup(cp => cp.IsInRole(It.IsAny<string>())).Returns(false);

        var httpContext = new DefaultHttpContext
        {
            User = claimsPrincipalMock.Object
        };

        var problemsController = new TestSetsController(mockedProblemsService.Object, mockedTestSetsService.Object)
        {
            ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            }
        };

        // Act
        var actionResult = await problemsController.Get("64639f6fcdde06187b09ecae");

        // Assert
        Assert.NotNull(actionResult);
        Assert.NotNull(actionResult.Value);

        var testSetModel = actionResult.Value;

        Assert.Equal(testSet!.Id, testSetModel.Id);
        Assert.Equal(testSet.Inputs?.Count, testSetModel.Inputs?.Count);
        Assert.Equal(testSet.IsPublic, testSetModel.IsPublic);
        Assert.Equal(testSet.Name, testSetModel.Name);
        Assert.Equal(testSet.Problem?.Id.AsString, testSetModel.ProblemId);
    }

    [Fact]
    [Trait("TestCategory", "UnitTest")]
    public async Task Get_By_Id_Should_Return_Ok_For_Participant_When_Is_Public()
    {
        // Arrange
        var testSetsTestData = new TestSetsTestData();

        var testSet = testSetsTestData.First(_ =>
            ((TestSet)_[0]).IsPublic &&
            (TestSetDataIssues)_[1] == TestSetDataIssues.None)[0] as TestSet;

        var mockedProblemsService = new Mock<IProblemsService>();

        var mockedTestSetsService = new Mock<ITestSetsService>();
        mockedTestSetsService.Setup(_ => _.GetAsync(It.IsAny<string>(), default))
            .ReturnsAsync(testSet);

        var identityMock = new Mock<IIdentity>();
        identityMock.Setup(i => i.Name).Returns("0000-000");

        var claimsPrincipalMock = new Mock<ClaimsPrincipal>();
        claimsPrincipalMock.Setup(cp => cp.Identity).Returns(identityMock.Object);
        claimsPrincipalMock.Setup(cp => cp.IsInRole(It.IsAny<string>())).Returns(true);

        var httpContext = new DefaultHttpContext
        {
            User = claimsPrincipalMock.Object
        };

        var problemsController = new TestSetsController(mockedProblemsService.Object, mockedTestSetsService.Object)
        {
            ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            }
        };

        // Act
        var actionResult = await problemsController.Get("64639f6fcdde06187b09ecae");

        // Assert
        Assert.NotNull(actionResult);
        Assert.NotNull(actionResult.Value);

        var testSetModel = actionResult.Value;

        Assert.Equal(testSet!.Id, testSetModel.Id);
        Assert.Equal(testSet.Inputs?.Count, testSetModel.Inputs?.Count);
        Assert.Equal(testSet.IsPublic, testSetModel.IsPublic);
        Assert.Equal(testSet.Name, testSetModel.Name);
        Assert.Equal(testSet.Problem?.Id.AsString, testSetModel.ProblemId);
    }

    [Fact]
    [Trait("TestCategory", "UnitTest")]
    public async Task Get_Should_Return_Ok_When_Empty_For_Judge()
    {
        // Arrange
        var emptyTestSetList = new List<TestSet>();

        var mockedProblemsService = new Mock<IProblemsService>();

        var mockedTestSetsService = new Mock<ITestSetsService>();
        mockedTestSetsService.Setup(_ => _.GetAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(emptyTestSetList);

        var identityMock = new Mock<IIdentity>();
        identityMock.Setup(i => i.Name).Returns("0000-000");

        var claimsPrincipalMock = new Mock<ClaimsPrincipal>();
        claimsPrincipalMock.Setup(cp => cp.Identity).Returns(identityMock.Object);
        claimsPrincipalMock.Setup(cp => cp.IsInRole(It.IsAny<string>())).Returns(false);

        var httpContext = new DefaultHttpContext
        {
            User = claimsPrincipalMock.Object
        };

        var problemsController = new TestSetsController(mockedProblemsService.Object, mockedTestSetsService.Object)
        {
            ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            }
        };

        // Act
        var actionResult = await problemsController.Get();

        // Assert
        Assert.NotNull(actionResult);
        Assert.NotNull(actionResult.Value);
        Assert.Empty(actionResult.Value!);
    }

    [Fact]
    [Trait("TestCategory", "UnitTest")]
    public async Task Get_Should_Return_Ok_When_Empty_For_Participant()
    {
        // Arrange
        var emptyTestSetList = new List<TestSet>();

        var mockedProblemsService = new Mock<IProblemsService>();

        var mockedTestSetsService = new Mock<ITestSetsService>();
        mockedTestSetsService.Setup(_ => _.GetAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(emptyTestSetList);

        var identityMock = new Mock<IIdentity>();
        identityMock.Setup(i => i.Name).Returns("0000-000");

        var claimsPrincipalMock = new Mock<ClaimsPrincipal>();
        claimsPrincipalMock.Setup(cp => cp.Identity).Returns(identityMock.Object);
        claimsPrincipalMock.Setup(cp => cp.IsInRole(It.IsAny<string>())).Returns(true);

        var httpContext = new DefaultHttpContext
        {
            User = claimsPrincipalMock.Object
        };

        var problemsController = new TestSetsController(mockedProblemsService.Object, mockedTestSetsService.Object)
        {
            ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            }
        };

        // Act
        var actionResult = await problemsController.Get();

        // Assert
        Assert.NotNull(actionResult);
        Assert.NotNull(actionResult.Value);
        Assert.Empty(actionResult.Value!);
    }

    [Fact]
    [Trait("TestCategory", "UnitTest")]
    public async Task Get_Should_Return_Ok_When_Not_Empty_For_Judge()
    {
        // Arrange
        var testSetsTestData = new TestSetsTestData();

        var testSetList = testSetsTestData
            .Where(_ => (TestSetDataIssues)_[1] == TestSetDataIssues.None)
            .Select(_ => _[0])
            .Cast<TestSet>()
            .ToList();

        var mockedProblemsService = new Mock<IProblemsService>();

        var mockedTestSetsService = new Mock<ITestSetsService>();
        mockedTestSetsService.Setup(_ => _.GetAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(testSetList);

        var identityMock = new Mock<IIdentity>();
        identityMock.Setup(i => i.Name).Returns("0000-000");

        var claimsPrincipalMock = new Mock<ClaimsPrincipal>();
        claimsPrincipalMock.Setup(cp => cp.Identity).Returns(identityMock.Object);
        claimsPrincipalMock.Setup(cp => cp.IsInRole(It.IsAny<string>())).Returns(false);

        var httpContext = new DefaultHttpContext
        {
            User = claimsPrincipalMock.Object
        };

        var problemsController = new TestSetsController(mockedProblemsService.Object, mockedTestSetsService.Object)
        {
            ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            }
        };

        // Act
        var actionResult = await problemsController.Get();

        // Assert
        Assert.NotNull(actionResult);
        Assert.NotNull(actionResult.Value);
        Assert.NotEmpty(actionResult.Value!);
        Assert.Equal(testSetList.Count, actionResult.Value!.Count);
    }

    [Fact]
    [Trait("TestCategory", "UnitTest")]
    public async Task Get_Should_Return_Ok_When_Not_Empty_For_Participant()
    {
        // Arrange
        var testSetsTestData = new TestSetsTestData();

        var testSetList = testSetsTestData
            .Where(_ => (TestSetDataIssues)_[1] == TestSetDataIssues.None)
            .Select(_ => _[0])
            .Cast<TestSet>()
            .ToList();

        var mockedProblemsService = new Mock<IProblemsService>();

        var mockedTestSetsService = new Mock<ITestSetsService>();
        mockedTestSetsService.Setup(_ => _.GetAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(testSetList);

        var identityMock = new Mock<IIdentity>();
        identityMock.Setup(i => i.Name).Returns("0000-000");

        var claimsPrincipalMock = new Mock<ClaimsPrincipal>();
        claimsPrincipalMock.Setup(cp => cp.Identity).Returns(identityMock.Object);
        claimsPrincipalMock.Setup(cp => cp.IsInRole(It.IsAny<string>())).Returns(true);

        var httpContext = new DefaultHttpContext
        {
            User = claimsPrincipalMock.Object
        };

        var problemsController = new TestSetsController(mockedProblemsService.Object, mockedTestSetsService.Object)
        {
            ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            }
        };

        // Act
        var actionResult = await problemsController.Get();

        // Assert
        Assert.NotNull(actionResult);
        Assert.NotNull(actionResult.Value);
        Assert.NotEmpty(actionResult.Value!);
        Assert.Equal(testSetList.Count(_ => _.IsPublic), actionResult.Value!.Count);
    }

    [Fact]
    [Trait("TestCategory", "UnitTest")]
    public async Task Post_Should_Return_Created()
    {
        // Arrange
        var newTestSet = new TestSetModel
        {
            Id = "000000000000000000000001",
            Inputs =
            [
                new()
                {
                    IsArray = false,
                    DataType = "string",
                    Index = 0,
                    ValueAsJson = "{ \"value\": \"string a\" }"
                }
            ],
            IsPublic = true,
            Name = "Test Set #1",
            ProblemId = "000000000000000000000001"
        };

        Func<TestSet, bool> validateTestSet = testSetToValidate =>
        {
            var idsMatch = testSetToValidate.Id == newTestSet.Id;
            var isPublicMatch = testSetToValidate.IsPublic == newTestSet.IsPublic;
            var namesMatch = testSetToValidate.Name == newTestSet.Name;
            var problemsMatch = testSetToValidate.Problem?.Id.AsString == newTestSet.ProblemId;

            var inputsMatch = testSetToValidate.Inputs is { Count: 1 } &&
                              testSetToValidate.Inputs[0].IsArray == newTestSet.Inputs[0].IsArray &&
                              testSetToValidate.Inputs[0].DataType == newTestSet.Inputs[0].DataType &&
                              testSetToValidate.Inputs[0].Index == newTestSet.Inputs[0].Index &&
                              testSetToValidate.Inputs[0].ValueAsJson == newTestSet.Inputs[0].ValueAsJson;


            return idsMatch && isPublicMatch && namesMatch && problemsMatch && inputsMatch;
        };

        var mockedProblemsService = new Mock<IProblemsService>();
        mockedProblemsService
            .Setup(_ => _.ExistsAsync(It.Is(newTestSet.ProblemId, new StringEqualityComparer()), default))
            .ReturnsAsync(true);

        var mockedTestSetsService = new Mock<ITestSetsService>();

        var problemsController = new TestSetsController(mockedProblemsService.Object, mockedTestSetsService.Object);

        // Act
        var createdAtActionResult = await problemsController.Post(newTestSet);

        // Assert
        Assert.NotNull(createdAtActionResult);

        Assert.IsType<TestSetModel>(createdAtActionResult.Value);

        mockedTestSetsService.Verify(_ => _.CreateAsync(It.Is<TestSet>(testSet => validateTestSet(testSet)), default), Times.Once);
    }

    [Fact]
    [Trait("TestCategory", "UnitTest")]
    public async Task Post_Should_Throw_Exception_When_Problem_Is_Not_Found()
    {
        // Arrange
        var newTestSet = new TestSetModel
        {
            Id = "000000000000000000000001",
            Inputs =
            [
                new()
                {
                    IsArray = false,
                    DataType = "string",
                    Index = 0,
                    ValueAsJson = "{ \"value\": \"string a\" }"
                }
            ],
            IsPublic = true,
            Name = "Test Set #1",
            ProblemId = "000000000000000000000001"
        };

        var mockedProblemsService = new Mock<IProblemsService>();
        mockedProblemsService
            .Setup(_ => _.ExistsAsync(It.Is(newTestSet.ProblemId, new StringEqualityComparer()), default))
            .ReturnsAsync(false);

        var mockedTestSetsService = new Mock<ITestSetsService>();

        var problemsController = new TestSetsController(mockedProblemsService.Object, mockedTestSetsService.Object);

        // Act and Assert
        await Assert.ThrowsAsync<RequiredEntityNotFoundException>(() => problemsController.Post(newTestSet));
    }

    [Fact]
    [Trait("TestCategory", "UnitTest")]
    public async Task Put_Should_Return_No_Content()
    {
        // Arrange
        var testSetsTestData = new TestSetsTestData();

        var testSet = testSetsTestData.First(_ => (TestSetDataIssues)_[1] == TestSetDataIssues.None)[0] as TestSet;

        var testSetModel = testSet!.ToModel();

        Func<TestSet, bool> validateTestSet = testSetToValidate =>
        {
            var idsMatch = testSetToValidate.Id == testSet!.Id;
            var isPublicMatch = testSetToValidate.IsPublic == testSet.IsPublic;
            var namesMatch = testSetToValidate.Name == testSet.Name;
            var problemsMatch = testSetToValidate.Problem?.Id.AsString == testSet.Problem?.Id.AsString;

            var inputsMatch = true;

            if (testSetToValidate.Inputs?.Count != testSet.Inputs?.Count) return false;

            foreach (var testSetInput in testSet.Inputs!)
            {
                var testSetInputModel = testSetToValidate.Inputs!.SingleOrDefault(_ => _.Index == testSetInput.Index);

                if (testSetInputModel == null) return false;

                inputsMatch = testSetInputModel.IsArray == testSetInput.IsArray &&
                              testSetInputModel.DataType == testSetInput.DataType &&
                              testSetInputModel.Index == testSetInput.Index &&
                              testSetInputModel.ValueAsJson == testSetInput.ValueAsJson;
            }

            return idsMatch && isPublicMatch && namesMatch && problemsMatch && inputsMatch;
        };

        var mockedProblemsService = new Mock<IProblemsService>();
        mockedProblemsService
            .Setup(_ => _.ExistsAsync(It.Is(testSetModel.ProblemId, new StringEqualityComparer())!, default))
            .ReturnsAsync(true);

        var mockedTestSetsService = new Mock<ITestSetsService>();
        mockedTestSetsService
            .Setup(_ => _.GetAsync(It.Is(testSet!.Id!, new StringEqualityComparer()), default))
            .ReturnsAsync(testSet);

        var problemsController = new TestSetsController(mockedProblemsService.Object, mockedTestSetsService.Object);

        // Act
        var actionResult = await problemsController.Put(testSet!.Id!, testSetModel);

        // Assert
        Assert.NotNull(actionResult);
        Assert.IsType<NoContentResult>(actionResult);

        mockedTestSetsService.Verify(_ => _.UpdateAsync(It.Is<TestSet>(testSetToValidate => validateTestSet(testSetToValidate)), default), Times.Once);
    }

    [Fact]
    [Trait("TestCategory", "UnitTest")]
    public async Task Put_Should_Return_Not_Found()
    {
        // Arrange
        var problemId = Guid.NewGuid().ToString();

        var mockedProblemsService = new Mock<IProblemsService>();
        mockedProblemsService
            .Setup(_ => _.ExistsAsync(It.Is(problemId, new StringEqualityComparer())!, default))
            .ReturnsAsync(true);

        var mockedTestSetsService = new Mock<ITestSetsService>();

        var problemsController = new TestSetsController(mockedProblemsService.Object, mockedTestSetsService.Object);

        // Act
        var actionResult = await problemsController.Put("64639f6fcdde06187b09ecae", new TestSetModel { ProblemId = problemId });

        // Assert
        Assert.NotNull(actionResult);
        Assert.IsType<NotFoundResult>(actionResult);
    }

    [Fact]
    [Trait("TestCategory", "UnitTest")]
    public async Task Put_Should_Throw_Exception_When_Problem_Is_Not_Found()
    {
        // Arrange
        var testSetsTestData = new TestSetsTestData();

        var testSet = testSetsTestData.First(_ => (TestSetDataIssues)_[1] == TestSetDataIssues.None)[0] as TestSet;

        var testSetModel = testSet!.ToModel();

        var mockedProblemsService = new Mock<IProblemsService>();
        mockedProblemsService
            .Setup(_ => _.ExistsAsync(It.Is(testSetModel.ProblemId, new StringEqualityComparer())!, default))
            .ReturnsAsync(false);

        var mockedTestSetsService = new Mock<ITestSetsService>();

        var problemsController = new TestSetsController(mockedProblemsService.Object, mockedTestSetsService.Object);

        // Act and Assert
        await Assert.ThrowsAsync<RequiredEntityNotFoundException>(() => problemsController.Put(testSet!.Id!, testSetModel));
    }
}
