using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Tsa.Submissions.Coding.Contracts.Submissions;
using Tsa.Submissions.Coding.Contracts.Users;
using Tsa.Submissions.Coding.UnitTests.Data;
using Tsa.Submissions.Coding.UnitTests.Helpers.Mocks;
using Tsa.Submissions.Coding.UnitTests.Helpers.Problems;
using Tsa.Submissions.Coding.UnitTests.Helpers.Reflection;
using Tsa.Submissions.Coding.UnitTests.Helpers.Submissions;
using Tsa.Submissions.Coding.UnitTests.Helpers.Users;
using Tsa.Submissions.Coding.WebApi.Controllers;
using Tsa.Submissions.Coding.WebApi.Entities;
using Tsa.Submissions.Coding.WebApi.Pagination;
using Tsa.Submissions.Coding.WebApi.Services;
using Xunit;

namespace Tsa.Submissions.Coding.UnitTests.WebApi.Controllers;

[ExcludeFromCodeCoverage]
public class SubmissionsControllerTest : ControllerTestsBase<SubmissionsController>
{
    private static readonly Type ControllerType = typeof(SubmissionsController);

    private static SubmissionsController CreateController(
        IMock<ILogger<SubmissionsController>> logger,
        IMock<IProblemsService> problemsService,
        IMock<IProgrammingLanguagesService> programmingLanguagesService,
        IMock<IValidator<SubmissionCreateRequest>> submissionCreateRequestValidator,
        IMock<ISubmissionsService> submissionsService,
        IMock<ISubmissionsQueueService> submissionsQueueService,
        IMock<IUsersService> usersService)
    {
        return new SubmissionsController(
            logger.Object,
            problemsService.Object,
            programmingLanguagesService.Object,
            submissionCreateRequestValidator.Object,
            submissionsService.Object,
            submissionsQueueService.Object,
            usersService.Object
        );
    }

    private static Submission GetValidSubmission()
    {
        var testData = new SubmissionsTestData();

        return (Submission)testData
            .First(data => (SubmissionDataIssues)data[1] == SubmissionDataIssues.None)[0];
    }

    private static List<Submission> GetValidSubmissions(int take = 5)
    {
        var testData = new SubmissionsTestData();

        return testData
            .Where(data => (SubmissionDataIssues)data[1] == SubmissionDataIssues.None)
            .Take(take)
            .Select(data => (Submission)data[0])
            .ToList();
    }

    private static List<User> GetValidUsers()
    {
        var testData = new UsersTestData();

        return testData
            .Where(data => (UserDataIssues)data[1] == UserDataIssues.None)
            .Select(data => (User)data[0])
            .ToList();
    }

    protected override string[] MethodsForAllRoles =>
    [
        "GetById",
        "GetByUserId"
    ];

    protected override string[] MethodsForJudgesOnly => [];

    protected override string[] MethodsForJudgesOrParticipants => [];

    protected override string[] MethodsForJudgesOrSystem =>
    [
        "Get",
        "Put"
    ];

    protected override string[] MethodsForParticipantsOnly =>
    [
        "Post"
    ];

    protected override string[] MethodsForSystemOnly => [];

    [Fact]
    [Trait("TestCategory", "UnitTest")]
    public override void Controller_Public_Methods_Should_Have_Authorize_Attribute_With_Proper_Roles()
    {
        PublicMethodsHaveAuthorizeAttributeWithProperRoles();
    }

    [Fact]
    [Trait("TestCategory", "UnitTest")]
    public override void Controller_Public_Methods_Should_Have_HttpMethod_Attributes()
    {
        PublicMethodsHaveHttpMethodAttribute();
    }

    [Fact]
    [Trait("TestCategory", "UnitTest")]
    public override void Controller_Should_Have_ApiController_Attribute()
    {
        // Move this to the base class if this logic grows more complex
        // Single line assertion doesn't justify the need for a separate method at this time
        Assert.True(TypeHelpers.ClassHasSingleAttribute<ApiControllerAttribute>(ControllerType));
    }

    [Fact]
    [Trait("TestCategory", "UnitTest")]
    public override void Controller_Should_Have_Expected_Number_Of_Public_Methods()
    {
        ClassHasExpectedNumberOfPublicMethods();
    }

    [Fact]
    [Trait("TestCategory", "UnitTest")]
    public override void Controller_Should_Have_Produces_Attribute()
    {
        HasProducesAttribute("application/json");
    }

    [Fact]
    [Trait("TestCategory", "UnitTest")]
    public override void Controller_Should_Have_Route_Attribute()
    {
        HasRouteAttribute("api/submissions");
    }

    [Fact]
    [Trait("TestCategory", "UnitTest")]
    public async Task Get_Should_Return_Ok_When_Empty()
    {
        // Arrange
        const int defaultPageSize = 20;

        var expectedCursorPagination = new CursorPagination
        {
            Cursor = null,
            PageSize = defaultPageSize,
            SortOrder = PaginationSortOrder.Descending
        };

        var pagedSubmissionResult = new PagedResult<Submission>
        {
            HasNextPage = false,
            Items = [],
            NextCursor = null,
            PageSize = defaultPageSize
        };

        var mockedLogger = new Mock<ILogger<SubmissionsController>>();

        var mockedProblemsService = new MockedProblemsServiceBuilder().Build();

        var mockedProgrammingLanguagesService = new MockedProgrammingLanguagesServiceBuilder().Build();

        var mockedSubmissionCreateRequestValidator = new MockedSubmissionCreateRequestValidator().Build();

        var mockedSubmissionsService = new MockedSubmissionsServiceBuilder()
            .WithGetPagedByIdCursorAsync(expectedCursorPagination, pagedSubmissionResult, Times.Once())
            .Build();

        var mockedSubmissionsQueueService = new MockedSubmissionsQueueServiceBuilder().Build();

        var mockedUsersService = new MockedUsersServiceBuilder()
            .WithGetAsync([], Times.Once())
            .Build();

        var controller = CreateController(
            mockedLogger,
            mockedProblemsService,
            mockedProgrammingLanguagesService,
            mockedSubmissionCreateRequestValidator,
            mockedSubmissionsService,
            mockedSubmissionsQueueService,
            mockedUsersService
        );

        // Act
        var actionResult = await controller.Get(); // Using default parameters to test the empty case

        // Assert
        Assert.NotNull(actionResult);
        Assert.NotNull(actionResult.Value);
        Assert.False(actionResult.Value.HasNextPage);
        Assert.Empty(actionResult.Value.Items);
    }

    [Fact]
    [Trait("TestCategory", "UnitTest")]
    public async Task Get_Should_Return_Ok_When_Results_Has_Less_Pages_Then_Requested()
    {
        // Arrange
        const int defaultPageSize = 20;

        var expectedCursorPagination = new CursorPagination
        {
            Cursor = null,
            PageSize = defaultPageSize,
            SortOrder = PaginationSortOrder.Descending
        };

        var submissions = GetValidSubmissions(2);
        var users = GetValidUsers();

        var pagedSubmissionResult = new PagedResult<Submission>
        {
            HasNextPage = false,
            Items = submissions,
            NextCursor = null,
            PageSize = defaultPageSize
        };

        var submissionListResponses = new List<SubmissionListResponse>();

        foreach (var submission in submissions)
        {
            var user = users.First(entity => entity.Id == submission.UserId);

            submissionListResponses.Add(new SubmissionListResponse(
                submission.Id!,
                submission.ProblemId!,
                submission.ProgrammingLanguageId!,
                submission.ProgrammingLanguageVersionTag!,
                submission.SubmittedOn!.Value,
                submission.EvaluatedOn,
                new UserResponse(
                    user.Id!,
                    user.UserName!,
                    user.Role!,
                    new TeamResponse(user.Team!.CompetitionLevel.ToString(), user.Team.SchoolNumber, user.Team.TeamNumber),
                    user.Participants
                )
            ));
        }

        var mockedLogger = new Mock<ILogger<SubmissionsController>>();

        var mockedProblemsService = new MockedProblemsServiceBuilder().Build();

        var mockedProgrammingLanguagesService = new MockedProgrammingLanguagesServiceBuilder().Build();

        var mockedSubmissionCreateRequestValidator = new MockedSubmissionCreateRequestValidator().Build();

        var mockedSubmissionsService = new MockedSubmissionsServiceBuilder()
            .WithGetPagedByIdCursorAsync(expectedCursorPagination, pagedSubmissionResult, Times.Once())
            .Build();

        var mockedSubmissionsQueueService = new MockedSubmissionsQueueServiceBuilder().Build();

        var mockedUsersService = new MockedUsersServiceBuilder()
            .WithGetAsync(users, Times.Once())
            .Build();

        var controller = CreateController(
            mockedLogger,
            mockedProblemsService,
            mockedProgrammingLanguagesService,
            mockedSubmissionCreateRequestValidator,
            mockedSubmissionsService,
            mockedSubmissionsQueueService,
            mockedUsersService
        );

        // Act
        var actionResult = await controller.Get(); // Using default parameters to test the empty case

        // Assert
        Assert.NotNull(actionResult);
        Assert.NotNull(actionResult.Value);
        Assert.False(actionResult.Value.HasNextPage);
        Assert.NotEmpty(actionResult.Value.Items);
        Assert.Equal(submissionListResponses, actionResult.Value.Items, new SubmissionListResponseEqualityComparer());
        Assert.Equal(defaultPageSize, actionResult.Value.PageSize);
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
