using System;
using System.Diagnostics.CodeAnalysis;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Tsa.Submissions.Coding.Contracts.Events;
using Tsa.Submissions.Coding.UnitTests.Helpers.Reflection;
using Tsa.Submissions.Coding.WebApi.Authorization;
using Tsa.Submissions.Coding.WebApi.Configuration;
using Tsa.Submissions.Coding.WebApi.Controllers;
using Tsa.Submissions.Coding.WebApi.Entities;
using Tsa.Submissions.Coding.WebApi.Services;
using Xunit;

namespace Tsa.Submissions.Coding.UnitTests.WebApi.Controllers;

[ExcludeFromCodeCoverage]
public class EventControllerTests : ControllerTestsBase<EventController>
{
    private static readonly Type ControllerType = typeof(EventController);

    protected override string[] MethodsForAllRoles =>
    [
        "Get"
    ];

    protected override string[] MethodsForJudgesOnly =>
    [
        "PostStart",
        "PostStop",
        "PutUserOverride"
    ];

    protected override string[] MethodsForJudgesOrParticipants => [];

    protected override string[] MethodsForJudgesOrSystem => [];

    protected override string[] MethodsForParticipantsOnly => [];

    protected override string[] MethodsForSystemOnly => [];

    private static EventController CreateController(
        Mock<IEventService> eventService,
        Mock<ILogger<EventController>> logger,
        IOptions<EventSettings> eventSettings,
        Mock<IUsersService> usersService)
    {
        return new EventController(
            eventService.Object,
            logger.Object,
            eventSettings,
            usersService.Object
        );
    }

    private static IOptions<EventSettings> CreateEventSettings(int durationInMinutes = 120)
    {
        return Options.Create(new EventSettings { DurationInMinutes = durationInMinutes });
    }

    private static DefaultHttpContext CreateJudgeHttpContext()
    {
        var claimsPrincipalMock = new Mock<ClaimsPrincipal>();
        claimsPrincipalMock.Setup(cp => cp.IsInRole(It.IsAny<string>())).Returns(false);

        return new DefaultHttpContext { User = claimsPrincipalMock.Object };
    }

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
        HasRouteAttribute("api/event");
    }

    [Fact]
    [Trait("TestCategory", "UnitTest")]
    public async Task Get_Should_Return_Inactive_Event_When_No_Event_Exists()
    {
        // Arrange
        var mockedEventService = new Mock<IEventService>();
        mockedEventService.Setup(s => s.GetCurrentAsync(default)).ReturnsAsync((Event?)null);

        var mockedLogger = new Mock<ILogger<EventController>>();
        var mockedUsersService = new Mock<IUsersService>();

        var controller = CreateController(mockedEventService, mockedLogger, CreateEventSettings(), mockedUsersService);

        // Act
        var actionResult = await controller.Get();

        // Assert
        Assert.NotNull(actionResult);
        var okResult = Assert.IsType<OkObjectResult>(actionResult.Result);
        var response = Assert.IsType<EventResponse>(okResult.Value);
        Assert.False(response.IsActive);
        Assert.Null(response.StartTime);
        Assert.Null(response.EndTime);
    }

    [Fact]
    [Trait("TestCategory", "UnitTest")]
    public async Task Get_Should_Return_Active_Event_When_Event_Is_Running()
    {
        // Arrange
        var startTime = DateTimeOffset.UtcNow.AddMinutes(-30);
        var endTime = startTime.AddMinutes(120);

        var activeEvent = new Event
        {
            Id = "64639f6fcdde06187b09ecbe",
            IsActive = true,
            StartTime = startTime,
            EndTime = endTime
        };

        var mockedEventService = new Mock<IEventService>();
        mockedEventService.Setup(s => s.GetCurrentAsync(default)).ReturnsAsync(activeEvent);

        var mockedLogger = new Mock<ILogger<EventController>>();
        var mockedUsersService = new Mock<IUsersService>();

        var controller = CreateController(mockedEventService, mockedLogger, CreateEventSettings(), mockedUsersService);

        // Act
        var actionResult = await controller.Get();

        // Assert
        Assert.NotNull(actionResult);
        var okResult = Assert.IsType<OkObjectResult>(actionResult.Result);
        var response = Assert.IsType<EventResponse>(okResult.Value);
        Assert.True(response.IsActive);
        Assert.Equal(startTime, response.StartTime);
        Assert.Equal(endTime, response.EndTime);
    }

    [Fact]
    [Trait("TestCategory", "UnitTest")]
    public async Task Start_Should_Return_Ok_With_Active_Event()
    {
        // Arrange
        const int durationInMinutes = 120;
        var startedEvent = new Event
        {
            Id = "64639f6fcdde06187b09ecbe",
            IsActive = true,
            StartTime = DateTimeOffset.UtcNow,
            EndTime = DateTimeOffset.UtcNow.AddMinutes(durationInMinutes)
        };

        var mockedEventService = new Mock<IEventService>();
        mockedEventService.Setup(s => s.StartAsync(durationInMinutes, default)).ReturnsAsync(startedEvent);

        var mockedLogger = new Mock<ILogger<EventController>>();
        var mockedUsersService = new Mock<IUsersService>();

        var controller = CreateController(mockedEventService, mockedLogger, CreateEventSettings(durationInMinutes), mockedUsersService);
        controller.ControllerContext = new ControllerContext { HttpContext = CreateJudgeHttpContext() };

        // Act
        var actionResult = await controller.PostStart();

        // Assert
        Assert.NotNull(actionResult);
        var okResult = Assert.IsType<OkObjectResult>(actionResult.Result);
        var response = Assert.IsType<EventResponse>(okResult.Value);
        Assert.True(response.IsActive);
        Assert.NotNull(response.StartTime);
        Assert.NotNull(response.EndTime);
    }

    [Fact]
    [Trait("TestCategory", "UnitTest")]
    public async Task Stop_Should_Return_Ok_With_Inactive_Event()
    {
        // Arrange
        var stoppedEvent = new Event
        {
            Id = "64639f6fcdde06187b09ecbe",
            IsActive = false,
            StartTime = DateTimeOffset.UtcNow.AddMinutes(-30),
            EndTime = DateTimeOffset.UtcNow.AddMinutes(90)
        };

        var mockedEventService = new Mock<IEventService>();
        mockedEventService.Setup(s => s.StopAsync(default)).ReturnsAsync(stoppedEvent);

        var mockedLogger = new Mock<ILogger<EventController>>();
        var mockedUsersService = new Mock<IUsersService>();

        var controller = CreateController(mockedEventService, mockedLogger, CreateEventSettings(), mockedUsersService);
        controller.ControllerContext = new ControllerContext { HttpContext = CreateJudgeHttpContext() };

        // Act
        var actionResult = await controller.PostStop();

        // Assert
        Assert.NotNull(actionResult);
        var okResult = Assert.IsType<OkObjectResult>(actionResult.Result);
        var response = Assert.IsType<EventResponse>(okResult.Value);
        Assert.False(response.IsActive);
    }

    [Fact]
    [Trait("TestCategory", "UnitTest")]
    public async Task Stop_Should_Return_Not_Found_When_No_Event_Exists()
    {
        // Arrange
        var mockedEventService = new Mock<IEventService>();
        mockedEventService.Setup(s => s.StopAsync(default)).ReturnsAsync((Event?)null);

        var mockedLogger = new Mock<ILogger<EventController>>();
        var mockedUsersService = new Mock<IUsersService>();

        var controller = CreateController(mockedEventService, mockedLogger, CreateEventSettings(), mockedUsersService);
        controller.ControllerContext = new ControllerContext { HttpContext = CreateJudgeHttpContext() };

        // Act
        var actionResult = await controller.PostStop();

        // Assert
        Assert.NotNull(actionResult);
        Assert.IsType<NotFoundResult>(actionResult.Result);
    }

    [Fact]
    [Trait("TestCategory", "UnitTest")]
    public async Task PutUserOverride_Should_Return_Ok_When_User_And_Event_Exist()
    {
        // Arrange
        const string userId = "64639f6fcdde06187b09eca0";
        var newEndTime = DateTimeOffset.UtcNow.AddMinutes(150);

        var user = new User { Id = userId, UserName = "0000-000", Role = SubmissionRoles.Participant };

        var updatedEvent = new Event
        {
            Id = "64639f6fcdde06187b09ecbe",
            IsActive = true,
            StartTime = DateTimeOffset.UtcNow.AddMinutes(-30),
            EndTime = DateTimeOffset.UtcNow.AddMinutes(90),
            UserOverrides = [new EventUserOverride { UserId = userId, EndTime = newEndTime }]
        };

        var mockedEventService = new Mock<IEventService>();
        mockedEventService.Setup(s => s.SetUserEndTimeOverrideAsync(userId, newEndTime, default)).ReturnsAsync(updatedEvent);

        var mockedUsersService = new Mock<IUsersService>();
        mockedUsersService.Setup(s => s.GetAsync(userId, default)).ReturnsAsync(user);

        var mockedLogger = new Mock<ILogger<EventController>>();

        var controller = CreateController(mockedEventService, mockedLogger, CreateEventSettings(), mockedUsersService);
        controller.ControllerContext = new ControllerContext { HttpContext = CreateJudgeHttpContext() };

        // Act
        var actionResult = await controller.PutUserOverride(userId, new EventUserOverrideRequest { EndTime = newEndTime });

        // Assert
        Assert.NotNull(actionResult);
        var okResult = Assert.IsType<OkObjectResult>(actionResult.Result);
        Assert.IsType<EventResponse>(okResult.Value);
    }

    [Fact]
    [Trait("TestCategory", "UnitTest")]
    public async Task PutUserOverride_Should_Return_Bad_Request_When_User_Does_Not_Exist()
    {
        // Arrange
        const string userId = "64639f6fcdde06187b09eca0";
        var newEndTime = DateTimeOffset.UtcNow.AddMinutes(150);

        var mockedEventService = new Mock<IEventService>();
        var mockedUsersService = new Mock<IUsersService>();
        mockedUsersService.Setup(s => s.GetAsync(userId, default)).ReturnsAsync((User?)null);

        var mockedLogger = new Mock<ILogger<EventController>>();

        var controller = CreateController(mockedEventService, mockedLogger, CreateEventSettings(), mockedUsersService);
        controller.ControllerContext = new ControllerContext { HttpContext = CreateJudgeHttpContext() };

        // Act
        var actionResult = await controller.PutUserOverride(userId, new EventUserOverrideRequest { EndTime = newEndTime });

        // Assert
        Assert.NotNull(actionResult);
        Assert.IsType<BadRequestObjectResult>(actionResult.Result);
    }

    [Fact]
    [Trait("TestCategory", "UnitTest")]
    public async Task PutUserOverride_Should_Return_Not_Found_When_No_Event_Exists()
    {
        // Arrange
        const string userId = "64639f6fcdde06187b09eca0";
        var newEndTime = DateTimeOffset.UtcNow.AddMinutes(150);

        var user = new User { Id = userId, UserName = "0000-000", Role = SubmissionRoles.Participant };

        var mockedEventService = new Mock<IEventService>();
        mockedEventService.Setup(s => s.SetUserEndTimeOverrideAsync(userId, newEndTime, default)).ReturnsAsync((Event?)null);

        var mockedUsersService = new Mock<IUsersService>();
        mockedUsersService.Setup(s => s.GetAsync(userId, default)).ReturnsAsync(user);

        var mockedLogger = new Mock<ILogger<EventController>>();

        var controller = CreateController(mockedEventService, mockedLogger, CreateEventSettings(), mockedUsersService);
        controller.ControllerContext = new ControllerContext { HttpContext = CreateJudgeHttpContext() };

        // Act
        var actionResult = await controller.PutUserOverride(userId, new EventUserOverrideRequest { EndTime = newEndTime });

        // Assert
        Assert.NotNull(actionResult);
        Assert.IsType<NotFoundResult>(actionResult.Result);
    }
}
