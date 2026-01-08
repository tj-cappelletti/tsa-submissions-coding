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
public class SubmissionsControllerTest
{
    [Fact]
    [Trait("TestCategory", "UnitTest")]
    public void Controller_Public_Methods_Should_Have_Authorize_Attribute_With_Proper_Roles()
    {
        var submissionsControllerType = typeof(SubmissionsController);

        var methodInfos = submissionsControllerType.GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly);

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

                case "GetAll":
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
        var submissionsControllerType = typeof(SubmissionsController);

        var methodInfos = submissionsControllerType.GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly);

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
                case "getall":
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
        var submissionsControllerType = typeof(SubmissionsController);

        var attributes = submissionsControllerType.GetCustomAttributes(typeof(ApiControllerAttribute), false);

        Assert.NotNull(attributes);
        Assert.NotEmpty(attributes);
        Assert.Single(attributes);
    }

    [Fact]
    [Trait("TestCategory", "UnitTest")]
    public void Controller_Should_Have_Produces_Attribute()
    {
        var submissionsControllerType = typeof(SubmissionsController);

        var attributes = submissionsControllerType.GetCustomAttributes(typeof(ProducesAttribute), false);

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
        var submissionsControllerType = typeof(SubmissionsController);

        var attributes = submissionsControllerType.GetCustomAttributes(typeof(RouteAttribute), false);

        Assert.NotNull(attributes);
        Assert.NotEmpty(attributes);
        Assert.Single(attributes);

        var routeAttribute = (RouteAttribute)attributes[0];

        Assert.Equal("api/[controller]", routeAttribute.Template);
    }

    //[Fact]
    //[Trait("TestCategory", "UnitTest")]
    //public async Task Get_By_Id_Should_Return_Failed_Dependency_When_Team_Not_Found()
    //{
    //    // Arrange
    //    var unexpectedMissingResourceApiError = ApiErrorResponseModel.UnexpectedMissingResource;

    //    var submissionsTestData = new SubmissionsTestData();

    //    var submission =
    //        submissionsTestData.First(submissionTestData => (SubmissionDataIssues)submissionTestData[1] == SubmissionDataIssues.None)[0] as Submission;

    //    var submissionId = submission!.Id;

    //    var teamsTestData = new TeamsTestData();
    //    var expectedTeam = teamsTestData
    //        .Where(submissionTestData => (TeamDataIssues)submissionTestData[1] == TeamDataIssues.None)
    //        .Select(submissionTestData => submissionTestData[0])
    //        .Cast<Team>()
    //        .Single(team => team.Id == submission.Team?.Id.AsString);

    //    var mockedSubmissionsService = new Mock<ISubmissionsService>();
    //    mockedSubmissionsService
    //        .Setup(submissionsService => submissionsService.GetAsync(It.Is(submissionId, new StringEqualityComparer())!, default))
    //        .ReturnsAsync(submission);

    //    var mockedTeamsService = new Mock<ITeamsService>();

    //    var identityMock = new Mock<IIdentity>();
    //    identityMock.Setup(identity => identity.Name).Returns(expectedTeam.Participants.First().ParticipantId);

    //    var claimsPrincipalMock = new Mock<ClaimsPrincipal>();
    //    claimsPrincipalMock.Setup(claimsPrincipal => claimsPrincipal.Identity).Returns(identityMock.Object);
    //    claimsPrincipalMock.Setup(claimsPrincipal => claimsPrincipal.IsInRole(It.Is(SubmissionRoles.Judge, new StringEqualityComparer()))).Returns(false);

    //    var httpContext = new DefaultHttpContext
    //    {
    //        User = claimsPrincipalMock.Object
    //    };

    //    var submissionsController = new SubmissionsController(mockedSubmissionsService.Object, mockedTeamsService.Object)
    //    {
    //        ControllerContext = new ControllerContext
    //        {
    //            HttpContext = httpContext
    //        }
    //    };

    //    // Act
    //    var actionResult = await submissionsController.Get(submissionId!);

    //    // Assert
    //    // Assert
    //    Assert.NotNull(actionResult);
    //    Assert.NotNull(actionResult.Result);

    //    var objectResult = actionResult.Result as ObjectResult;

    //    Assert.NotNull(objectResult);
    //    Assert.NotNull(objectResult.Value);

    //    var apiErrorResponseModel = objectResult.Value as ApiErrorResponseModel;
    //    Assert.NotNull(apiErrorResponseModel);
    //    Assert.Equal(unexpectedMissingResourceApiError, apiErrorResponseModel, new ApiErrorResponseModelEqualityComparer());
    //}

    //[Fact]
    //[Trait("TestCategory", "UnitTest")]
    //public async Task Get_By_Id_Should_Return_Not_Found()
    //{
    //    // Arrange
    //    var mockedSubmissionsService = new Mock<ISubmissionsService>();

    //    var mockedTeamsService = new Mock<ITeamsService>();

    //    var submissionsController = new SubmissionsController(mockedSubmissionsService.Object, mockedTeamsService.Object);

    //    // Act
    //    var actionResult = await submissionsController.Get("64639f6fcdde06187b09ecae");

    //    // Assert
    //    Assert.NotNull(actionResult);
    //    //Assert.IsType<NotFoundResult>(actionResult.Result);
    //}

    //[Fact]
    //[Trait("TestCategory", "UnitTest")]
    //public async Task Get_By_Id_Should_Return_Not_Found_For_Participant()
    //{
    //    // Arrange
    //    var submissionsTestData = new SubmissionsTestData();

    //    var submission =
    //        submissionsTestData.First(submissionTestData => (SubmissionDataIssues)submissionTestData[1] == SubmissionDataIssues.None)[0] as Submission;

    //    var submissionId = submission!.Id;

    //    var teamsTestData = new TeamsTestData();
    //    var expectedTeam = teamsTestData
    //        .Where(submissionTestData => (TeamDataIssues)submissionTestData[1] == TeamDataIssues.None)
    //        .Select(submissionTestData => submissionTestData[0])
    //        .Cast<Team>()
    //        .Single(team => team.Id == submission.Team?.Id.AsString);

    //    var participantTeam = teamsTestData
    //        .Where(submissionTestData => (TeamDataIssues)submissionTestData[1] == TeamDataIssues.None)
    //        .Select(submissionTestData => submissionTestData[0])
    //        .Cast<Team>()
    //        .First(team => team.Id != submission.Team?.Id.AsString);

    //    var mockedSubmissionsService = new Mock<ISubmissionsService>();
    //    mockedSubmissionsService
    //        .Setup(submissionsService => submissionsService.GetAsync(It.Is(submissionId, new StringEqualityComparer())!, default))
    //        .ReturnsAsync(submission);

    //    var mockedTeamsService = new Mock<ITeamsService>();
    //    mockedTeamsService
    //        .Setup(teamsService => teamsService.GetAsync(It.Is(expectedTeam.Id, new StringEqualityComparer())!, It.IsAny<CancellationToken>()))
    //        .ReturnsAsync(expectedTeam);

    //    var identityMock = new Mock<IIdentity>();
    //    identityMock.Setup(identity => identity.Name).Returns(participantTeam.Participants.First().ParticipantId);

    //    var claimsPrincipalMock = new Mock<ClaimsPrincipal>();
    //    claimsPrincipalMock.Setup(claimsPrincipal => claimsPrincipal.Identity).Returns(identityMock.Object);
    //    claimsPrincipalMock.Setup(claimsPrincipal => claimsPrincipal.IsInRole(It.Is(SubmissionRoles.Judge, new StringEqualityComparer()))).Returns(false);

    //    var httpContext = new DefaultHttpContext
    //    {
    //        User = claimsPrincipalMock.Object
    //    };

    //    var submissionsController = new SubmissionsController(mockedSubmissionsService.Object, mockedTeamsService.Object)
    //    {
    //        ControllerContext = new ControllerContext
    //        {
    //            HttpContext = httpContext
    //        }
    //    };

    //    // Act
    //    var actionResult = await submissionsController.Get(submissionId!);

    //    // Assert
    //    Assert.NotNull(actionResult);
    //    var notFoundResult = actionResult.Result as NotFoundResult;
    //    Assert.NotNull(notFoundResult);
    //}

    //[Fact]
    //[Trait("TestCategory", "UnitTest")]
    //public async Task Get_By_Id_Should_Return_Ok_For_Judge()
    //{
    //    // Arrange
    //    var submissionsTestData = new SubmissionsTestData();

    //    var submission =
    //        submissionsTestData.First(submissionTestData => (SubmissionDataIssues)submissionTestData[1] == SubmissionDataIssues.None)[0] as Submission;

    //    var expectedSubmissionModel = submission!.ToModel();

    //    var submissionId = submission!.Id;

    //    var mockedSubmissionsService = new Mock<ISubmissionsService>();
    //    mockedSubmissionsService
    //        .Setup(submissionsService => submissionsService.GetAsync(It.Is(submissionId, new StringEqualityComparer())!, default))
    //        .ReturnsAsync(submission);

    //    var mockedTeamsService = new Mock<ITeamsService>();

    //    var identityMock = new Mock<IIdentity>();
    //    identityMock.Setup(identity => identity.Name).Returns("0000-000");

    //    var claimsPrincipalMock = new Mock<ClaimsPrincipal>();
    //    claimsPrincipalMock.Setup(claimsPrincipal => claimsPrincipal.Identity).Returns(identityMock.Object);
    //    claimsPrincipalMock.Setup(claimsPrincipal => claimsPrincipal.IsInRole(It.Is(SubmissionRoles.Judge, new StringEqualityComparer()))).Returns(true);

    //    var httpContext = new DefaultHttpContext
    //    {
    //        User = claimsPrincipalMock.Object
    //    };

    //    var submissionsController = new SubmissionsController(mockedSubmissionsService.Object, mockedTeamsService.Object)
    //    {
    //        ControllerContext = new ControllerContext
    //        {
    //            HttpContext = httpContext
    //        }
    //    };

    //    // Act
    //    var actionResult = await submissionsController.Get(submissionId!);

    //    // Assert
    //    Assert.NotNull(actionResult);
    //    Assert.NotNull(actionResult.Value);
    //    Assert.Equal(expectedSubmissionModel, actionResult.Value, new SubmissionModelEqualityComparer());
    //}

    //[Fact]
    //[Trait("TestCategory", "UnitTest")]
    //public async Task Get_By_Id_Should_Return_Ok_For_Participant()
    //{
    //    // Arrange
    //    var submissionsTestData = new SubmissionsTestData();

    //    var submission =
    //        submissionsTestData.First(submissionTestData => (SubmissionDataIssues)submissionTestData[1] == SubmissionDataIssues.None)[0] as Submission;

    //    var submissionId = submission!.Id;

    //    var expectedSubmissionModel = submission.ToModel();

    //    var teamsTestData = new TeamsTestData();
    //    var expectedTeam = teamsTestData
    //        .Where(submissionTestData => (TeamDataIssues)submissionTestData[1] == TeamDataIssues.None)
    //        .Select(submissionTestData => submissionTestData[0])
    //        .Cast<Team>()
    //        .Single(team => team.Id == submission.Team?.Id.AsString);

    //    var mockedSubmissionsService = new Mock<ISubmissionsService>();
    //    mockedSubmissionsService
    //        .Setup(submissionsService => submissionsService.GetAsync(It.Is(submissionId, new StringEqualityComparer())!, default))
    //        .ReturnsAsync(submission);

    //    var mockedTeamsService = new Mock<ITeamsService>();
    //    mockedTeamsService
    //        .Setup(teamsService => teamsService.GetAsync(It.Is(expectedTeam.Id, new StringEqualityComparer())!, It.IsAny<CancellationToken>()))
    //        .ReturnsAsync(expectedTeam);

    //    var identityMock = new Mock<IIdentity>();
    //    identityMock.Setup(identity => identity.Name).Returns(expectedTeam.Participants.First().ParticipantId);

    //    var claimsPrincipalMock = new Mock<ClaimsPrincipal>();
    //    claimsPrincipalMock.Setup(claimsPrincipal => claimsPrincipal.Identity).Returns(identityMock.Object);
    //    claimsPrincipalMock.Setup(claimsPrincipal => claimsPrincipal.IsInRole(It.Is(SubmissionRoles.Judge, new StringEqualityComparer()))).Returns(false);

    //    var httpContext = new DefaultHttpContext
    //    {
    //        User = claimsPrincipalMock.Object
    //    };

    //    var submissionsController = new SubmissionsController(mockedSubmissionsService.Object, mockedTeamsService.Object)
    //    {
    //        ControllerContext = new ControllerContext
    //        {
    //            HttpContext = httpContext
    //        }
    //    };

    //    // Act
    //    var actionResult = await submissionsController.Get(submissionId!);

    //    // Assert
    //    Assert.NotNull(actionResult);
    //    Assert.NotNull(actionResult.Value);
    //    Assert.Equal(expectedSubmissionModel, actionResult.Value, new SubmissionModelEqualityComparer());
    //}

    //[Fact]
    //[Trait("TestCategory", "UnitTest")]
    //public async Task GetAll_Should_Return_Failed_Dependency_When_Team_Not_Found()
    //{
    //    // Arrange
    //    var unexpectedMissingResourceApiError = ApiErrorResponseModel.UnexpectedMissingResource;

    //    var submissionsTestData = new SubmissionsTestData();

    //    var submissionsList = submissionsTestData
    //        .Where(submissionTestData => (SubmissionDataIssues)submissionTestData[1] == SubmissionDataIssues.None)
    //        .Select(submissionTestData => submissionTestData[0])
    //        .Cast<Submission>()
    //        .ToList();

    //    var teamsTestData = new TeamsTestData();
    //    var expectedTeam = teamsTestData
    //        .Where(submissionTestData => (TeamDataIssues)submissionTestData[1] == TeamDataIssues.None)
    //        .Select(submissionTestData => submissionTestData[0])
    //        .Cast<Team>()
    //        .First();

    //    var expectedTeams = teamsTestData
    //        .Where(submissionTestData => (TeamDataIssues)submissionTestData[1] == TeamDataIssues.None)
    //        .Select(submissionTestData => submissionTestData[0])
    //        .Cast<Team>()
    //        .Where(team => team.Id != expectedTeam.Id)
    //        .ToList();

    //    var mockedSubmissionsService = new Mock<ISubmissionsService>();
    //    mockedSubmissionsService.Setup(submissionsService => submissionsService.GetAsync(default))
    //        .ReturnsAsync(submissionsList);

    //    var mockedTeamsService = new Mock<ITeamsService>();
    //    mockedTeamsService
    //        .Setup(teamsService => teamsService.GetAsync(default))
    //        .ReturnsAsync(expectedTeams);

    //    var identityMock = new Mock<IIdentity>();
    //    identityMock.Setup(identity => identity.Name).Returns(expectedTeam.Participants.First().ParticipantId);

    //    var claimsPrincipalMock = new Mock<ClaimsPrincipal>();
    //    claimsPrincipalMock.Setup(claimsPrincipal => claimsPrincipal.Identity).Returns(identityMock.Object);
    //    claimsPrincipalMock.Setup(claimsPrincipal => claimsPrincipal.IsInRole(It.Is(SubmissionRoles.Judge, new StringEqualityComparer()))).Returns(false);

    //    var httpContext = new DefaultHttpContext
    //    {
    //        User = claimsPrincipalMock.Object
    //    };

    //    var submissionsController = new SubmissionsController(mockedSubmissionsService.Object, mockedTeamsService.Object)
    //    {
    //        ControllerContext = new ControllerContext
    //        {
    //            HttpContext = httpContext
    //        }
    //    };

    //    // Act
    //    var actionResult = await submissionsController.GetAll();

    //    // Assert
    //    Assert.NotNull(actionResult);
    //    Assert.NotNull(actionResult.Result);

    //    var objectResult = actionResult.Result as ObjectResult;

    //    Assert.NotNull(objectResult);
    //    Assert.NotNull(objectResult.Value);

    //    var apiErrorResponseModel = objectResult.Value as ApiErrorResponseModel;
    //    Assert.NotNull(apiErrorResponseModel);
    //    Assert.Equal(unexpectedMissingResourceApiError, apiErrorResponseModel, new ApiErrorResponseModelEqualityComparer());
    //}

    //[Fact]
    //[Trait("TestCategory", "UnitTest")]
    //public async Task GetAll_Should_Return_Ok_When_Empty_For_Judge()
    //{
    //    // Arrange
    //    var emptySubmissionsList = new List<Submission>();

    //    var mockedSubmissionsService = new Mock<ISubmissionsService>();
    //    mockedSubmissionsService.Setup(submissionsService => submissionsService.GetAsync(default))
    //        .ReturnsAsync(emptySubmissionsList);

    //    var mockedTeamsService = new Mock<ITeamsService>();

    //    var identityMock = new Mock<IIdentity>();
    //    identityMock.Setup(identity => identity.Name).Returns("0000-000");

    //    var claimsPrincipalMock = new Mock<ClaimsPrincipal>();
    //    claimsPrincipalMock.Setup(claimsPrincipal => claimsPrincipal.Identity).Returns(identityMock.Object);
    //    claimsPrincipalMock.Setup(claimsPrincipal => claimsPrincipal.IsInRole(It.Is(SubmissionRoles.Judge, new StringEqualityComparer()))).Returns(true);

    //    var httpContext = new DefaultHttpContext
    //    {
    //        User = claimsPrincipalMock.Object
    //    };

    //    var submissionsController = new SubmissionsController(mockedSubmissionsService.Object, mockedTeamsService.Object)
    //    {
    //        ControllerContext = new ControllerContext
    //        {
    //            HttpContext = httpContext
    //        }
    //    };

    //    // Act
    //    var actionResult = await submissionsController.GetAll();

    //    // Assert
    //    Assert.NotNull(actionResult);
    //    Assert.NotNull(actionResult.Value);
    //    Assert.Empty(actionResult.Value);
    //}

    //[Fact]
    //[Trait("TestCategory", "UnitTest")]
    //public async Task GetAll_Should_Return_Ok_When_Empty_For_Participant()
    //{
    //    // Arrange
    //    var emptySubmissionsList = new List<Submission>();

    //    var teamsTestData = new TeamsTestData();
    //    var teams = teamsTestData
    //        .Where(submissionTestData => (TeamDataIssues)submissionTestData[1] == TeamDataIssues.None)
    //        .Select(submissionTestData => submissionTestData[0])
    //        .Cast<Team>()
    //        .ToList();

    //    var team = teams.First();

    //    var mockedSubmissionsService = new Mock<ISubmissionsService>();
    //    mockedSubmissionsService.Setup(submissionsService => submissionsService.GetAsync(default))
    //        .ReturnsAsync(emptySubmissionsList);

    //    var mockedTeamsService = new Mock<ITeamsService>();
    //    mockedTeamsService
    //        .Setup(teamsService => teamsService.GetAsync(default))
    //        .ReturnsAsync(teams);

    //    var identityMock = new Mock<IIdentity>();
    //    identityMock.Setup(identity => identity.Name).Returns(team.Participants.First().ParticipantId);

    //    var claimsPrincipalMock = new Mock<ClaimsPrincipal>();
    //    claimsPrincipalMock.Setup(claimsPrincipal => claimsPrincipal.Identity).Returns(identityMock.Object);
    //    claimsPrincipalMock.Setup(claimsPrincipal => claimsPrincipal.IsInRole(It.Is(SubmissionRoles.Judge, new StringEqualityComparer()))).Returns(false);

    //    var httpContext = new DefaultHttpContext
    //    {
    //        User = claimsPrincipalMock.Object
    //    };

    //    var submissionsController = new SubmissionsController(mockedSubmissionsService.Object, mockedTeamsService.Object)
    //    {
    //        ControllerContext = new ControllerContext
    //        {
    //            HttpContext = httpContext
    //        }
    //    };

    //    // Act
    //    var actionResult = await submissionsController.GetAll();

    //    // Assert
    //    Assert.NotNull(actionResult);
    //    Assert.NotNull(actionResult.Value);
    //    Assert.Empty(actionResult.Value);
    //}

    //[Fact]
    //[Trait("TestCategory", "UnitTest")]
    //public async Task GetAll_Should_Return_Ok_When_Not_Empty_For_Judge()
    //{
    //    // Arrange
    //    var submissionsTestData = new SubmissionsTestData();

    //    var submissionsList = submissionsTestData
    //        .Where(submissionTestData => (SubmissionDataIssues)submissionTestData[1] == SubmissionDataIssues.None)
    //        .Select(submissionTestData => submissionTestData[0])
    //        .Cast<Submission>()
    //        .ToList();

    //    var mockedSubmissionsService = new Mock<ISubmissionsService>();
    //    mockedSubmissionsService.Setup(submissionsService => submissionsService.GetAsync(default))
    //        .ReturnsAsync(submissionsList);

    //    var mockedTeamsService = new Mock<ITeamsService>();

    //    var identityMock = new Mock<IIdentity>();
    //    identityMock.Setup(identity => identity.Name).Returns("0000-000");

    //    var claimsPrincipalMock = new Mock<ClaimsPrincipal>();
    //    claimsPrincipalMock.Setup(claimsPrincipal => claimsPrincipal.Identity).Returns(identityMock.Object);
    //    claimsPrincipalMock.Setup(claimsPrincipal => claimsPrincipal.IsInRole(It.Is(SubmissionRoles.Judge, new StringEqualityComparer()))).Returns(true);

    //    var httpContext = new DefaultHttpContext
    //    {
    //        User = claimsPrincipalMock.Object
    //    };

    //    var submissionsController = new SubmissionsController(mockedSubmissionsService.Object, mockedTeamsService.Object)
    //    {
    //        ControllerContext = new ControllerContext
    //        {
    //            HttpContext = httpContext
    //        }
    //    };

    //    // Act
    //    var actionResult = await submissionsController.GetAll();

    //    // Assert
    //    Assert.NotNull(actionResult);
    //    Assert.NotNull(actionResult.Value);
    //    Assert.NotEmpty(actionResult.Value!);
    //    Assert.Equal(submissionsList.Count, actionResult.Value!.Count);
    //    Assert.Equal(submissionsList.ToModels(), actionResult.Value, new SubmissionModelEqualityComparer());
    //}

    //[Fact]
    //[Trait("TestCategory", "UnitTest")]
    //public async Task GetAll_Should_Return_Ok_When_Not_Empty_For_Participant()
    //{
    //    // Arrange
    //    var submissionsTestData = new SubmissionsTestData();

    //    var submissionsList = submissionsTestData
    //        .Where(submissionTestData => (SubmissionDataIssues)submissionTestData[1] == SubmissionDataIssues.None)
    //        .Select(submissionTestData => submissionTestData[0])
    //        .Cast<Submission>()
    //        .ToList();

    //    var teamsTestData = new TeamsTestData();
    //    var teams = teamsTestData
    //        .Where(submissionTestData => (TeamDataIssues)submissionTestData[1] == TeamDataIssues.None)
    //        .Select(submissionTestData => submissionTestData[0])
    //        .Cast<Team>()
    //        .ToList();

    //    var team = teams.First();

    //    var mockedSubmissionsService = new Mock<ISubmissionsService>();
    //    mockedSubmissionsService.Setup(submissionsService => submissionsService.GetAsync(default))
    //        .ReturnsAsync(submissionsList);

    //    var mockedTeamsService = new Mock<ITeamsService>();
    //    mockedTeamsService
    //        .Setup(teamsService => teamsService.GetAsync(default))
    //        .ReturnsAsync(teams);

    //    var identityMock = new Mock<IIdentity>();
    //    identityMock.Setup(identity => identity.Name).Returns(team.Participants.First().ParticipantId);

    //    var claimsPrincipalMock = new Mock<ClaimsPrincipal>();
    //    claimsPrincipalMock.Setup(claimsPrincipal => claimsPrincipal.Identity).Returns(identityMock.Object);
    //    claimsPrincipalMock.Setup(claimsPrincipal => claimsPrincipal.IsInRole(It.Is(SubmissionRoles.Judge, new StringEqualityComparer()))).Returns(false);

    //    var httpContext = new DefaultHttpContext
    //    {
    //        User = claimsPrincipalMock.Object
    //    };

    //    var submissionsController = new SubmissionsController(mockedSubmissionsService.Object, mockedTeamsService.Object)
    //    {
    //        ControllerContext = new ControllerContext
    //        {
    //            HttpContext = httpContext
    //        }
    //    };

    //    // Act
    //    var actionResult = await submissionsController.GetAll();

    //    // Assert
    //    Assert.NotNull(actionResult);
    //    Assert.NotNull(actionResult.Value);
    //    Assert.NotEmpty(actionResult.Value);
    //    Assert.Equal(submissionsList.Count, actionResult.Value!.Count);
    //    Assert.Equal(submissionsList.ToModels(), actionResult.Value, new SubmissionModelEqualityComparer());
    //}

    //[Fact]
    //[Trait("TestCategory", "UnitTest")]
    //public async Task Post_Should_Return_Created()
    //{
    //    // Arrange
    //    var newSubmission = new SubmissionModel
    //    {
    //        IsFinalSubmission = false,
    //        Language = "Language",
    //        ProblemId = "000000000000000000000001",
    //        Solution = "This is the solution",
    //        TeamId = "000000000000000000000001"
    //    };

    //    var mockedSubmissionsService = new Mock<ISubmissionsService>();

    //    var mockedTeamsService = new Mock<ITeamsService>();

    //    var submissionsController = new SubmissionsController(mockedSubmissionsService.Object, mockedTeamsService.Object);

    //    // Act
    //    var createdAtActionResult = await submissionsController.Post(newSubmission);

    //    // Assert
    //    Assert.NotNull(createdAtActionResult);

    //    Assert.IsType<SubmissionModel>(createdAtActionResult.Value);

    //    mockedSubmissionsService.Verify(
    //        submissionsService => submissionsService.CreateAsync(It.Is(newSubmission.ToEntity(), new SubmissionEqualityComparer(true)), default),
    //        Times.Once);
    //}

    //[Fact]
    //[Trait("TestCategory", "UnitTest")]
    //public async Task Put_Should_Return_No_Content()
    //{
    //    // Arrange
    //    var submissionsTestData = new SubmissionsTestData();

    //    var submission =
    //        submissionsTestData.First(submissionTestData => (SubmissionDataIssues)submissionTestData[1] == SubmissionDataIssues.None)[0] as Submission;

    //    var updatedSubmission = new SubmissionModel
    //    {
    //        Id = submission!.Id,
    //        IsFinalSubmission = true,
    //        Language = "Language",
    //        ProblemId = "000000000000000000000001",
    //        Solution = "This is the solution",
    //        SubmittedOn = submission.SubmittedOn,
    //        TeamId = "000000000000000000000001"
    //    };

    //    var mockedSubmissionsService = new Mock<ISubmissionsService>();
    //    mockedSubmissionsService.Setup(submissionsService => submissionsService.GetAsync(It.Is(submission.Id, new StringEqualityComparer())!, default))
    //        .ReturnsAsync(submission);

    //    var mockedTeamsService = new Mock<ITeamsService>();

    //    var submissionsController = new SubmissionsController(mockedSubmissionsService.Object, mockedTeamsService.Object);

    //    // Act
    //    var actionResult = await submissionsController.Put(submission.Id!, updatedSubmission);

    //    // Assert
    //    Assert.NotNull(actionResult);
    //    Assert.IsType<NoContentResult>(actionResult);

    //    mockedSubmissionsService.Verify(
    //        submissionsService => submissionsService.UpdateAsync(It.Is(updatedSubmission.ToEntity(), new SubmissionEqualityComparer()), default),
    //        Times.Once);
    //}

    //[Fact]
    //[Trait("TestCategory", "UnitTest")]
    //public async Task Put_Should_Return_No_Content_And_Ensure_Properties_Are_Immutable()
    //{
    //    // Arrange
    //    var submissionsTestData = new SubmissionsTestData();

    //    var submission =
    //        submissionsTestData.First(submissionTestData => (SubmissionDataIssues)submissionTestData[1] == SubmissionDataIssues.None)[0] as Submission;

    //    var id = submission!.Id;
    //    const bool isFinalSubmission = true;
    //    const string language = "Language";
    //    const string problemId = "000000000000000000000001";
    //    const string solution = "This is immutable and should not be the expected value";
    //    var submittedOn = submission.SubmittedOn!.Value.AddDays(5);
    //    const string teamId = "000000000000000000000001";

    //    var controlSubmission = new SubmissionModel
    //    {
    //        Id = id,
    //        IsFinalSubmission = isFinalSubmission,
    //        Language = language,
    //        ProblemId = problemId,
    //        Solution = solution,
    //        SubmittedOn = submittedOn,
    //        TeamId = teamId
    //    };

    //    var updatedSubmission = new SubmissionModel
    //    {
    //        Id = id,
    //        IsFinalSubmission = isFinalSubmission,
    //        Language = language,
    //        ProblemId = problemId,
    //        Solution = solution,
    //        SubmittedOn = submittedOn,
    //        TeamId = teamId
    //    };

    //    var mockedSubmissionsService = new Mock<ISubmissionsService>();
    //    mockedSubmissionsService.Setup(submissionsService => submissionsService.GetAsync(It.Is(submission.Id, new StringEqualityComparer())!, default))
    //        .ReturnsAsync(submission);

    //    var mockedTeamsService = new Mock<ITeamsService>();

    //    var submissionsController = new SubmissionsController(mockedSubmissionsService.Object, mockedTeamsService.Object);

    //    // Act
    //    var actionResult = await submissionsController.Put(submission.Id!, updatedSubmission);

    //    // Assert
    //    Assert.NotNull(actionResult);
    //    Assert.IsType<NoContentResult>(actionResult);

    //    mockedSubmissionsService.Verify(
    //        submissionsService => submissionsService.UpdateAsync(It.Is(updatedSubmission.ToEntity(), new SubmissionEqualityComparer()), default),
    //        Times.Once);

    //    Assert.NotEqual(controlSubmission, updatedSubmission, new SubmissionModelEqualityComparer());
    //}

    //[Fact]
    //[Trait("TestCategory", "UnitTest")]
    //public async Task Put_Should_Return_Not_Found()
    //{
    //    // Arrange
    //    var mockedSubmissionsService = new Mock<ISubmissionsService>();

    //    var mockedTeamsService = new Mock<ITeamsService>();

    //    var submissionsController = new SubmissionsController(mockedSubmissionsService.Object, mockedTeamsService.Object);

    //    // Act
    //    var actionResult = await submissionsController.Put("64639f6fcdde06187b09ecae", new SubmissionModel());

    //    // Assert
    //    Assert.NotNull(actionResult);
    //    Assert.IsType<NotFoundResult>(actionResult);
    //}
}
