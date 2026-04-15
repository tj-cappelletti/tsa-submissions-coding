using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Tsa.Submissions.Coding.Contracts.Events;
using Tsa.Submissions.Coding.WebApi.Authorization;
using Tsa.Submissions.Coding.WebApi.Configuration;
using Tsa.Submissions.Coding.WebApi.Entities;
using Tsa.Submissions.Coding.WebApi.ExtensionMethods;
using Tsa.Submissions.Coding.WebApi.Services;

namespace Tsa.Submissions.Coding.WebApi.Controllers;

[Route("api/event")]
[ApiController]
[Produces("application/json")]
public class EventController : WebApiBaseController
{
    private readonly IEventService _eventService;
    private readonly ILogger<EventController> _logger;
    private readonly EventSettings _eventSettings;
    private readonly IUsersService _usersService;

    public EventController(
        IEventService eventService,
        ILogger<EventController> logger,
        IOptions<EventSettings> eventSettingsOptions,
        IUsersService usersService)
    {
        _eventService = eventService;
        _logger = logger;
        _eventSettings = eventSettingsOptions.Value;
        _usersService = usersService;
    }

    /// <summary>
    ///     Gets the current event status
    /// </summary>
    /// <param name="cancellationToken">The .NET cancellation token</param>
    /// <response code="200">Returns the current event status</response>
    /// <response code="401">Authentication has failed</response>
    [Authorize(Roles = SubmissionRoles.All)]
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(EventResponse))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<EventResponse>> Get(CancellationToken cancellationToken = default)
    {
        var @event = await _eventService.GetCurrentAsync(cancellationToken);

        if (@event == null)
        {
            return Ok(new EventResponse(false, null, null));
        }

        return Ok(@event.ToResponse());
    }

    /// <summary>
    ///     Starts the event
    /// </summary>
    /// <param name="cancellationToken">The .NET cancellation token</param>
    /// <response code="200">The event was started and the updated status is returned</response>
    /// <response code="401">Authentication has failed</response>
    /// <response code="403">You do not have permission to use this endpoint</response>
    [Authorize(Roles = SubmissionRoles.Judge)]
    [HttpPost("start")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(EventResponse))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<EventResponse>> PostStart(CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Starting event with duration {DurationInMinutes} minutes", _eventSettings.DurationInMinutes);

        var @event = await _eventService.StartAsync(_eventSettings.DurationInMinutes, cancellationToken);

        return Ok(@event.ToResponse());
    }

    /// <summary>
    ///     Stops the event
    /// </summary>
    /// <param name="cancellationToken">The .NET cancellation token</param>
    /// <response code="200">The event was stopped and the updated status is returned</response>
    /// <response code="401">Authentication has failed</response>
    /// <response code="403">You do not have permission to use this endpoint</response>
    /// <response code="404">No event exists to stop</response>
    [Authorize(Roles = SubmissionRoles.Judge)]
    [HttpPost("stop")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(EventResponse))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<EventResponse>> PostStop(CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Stopping event");

        var @event = await _eventService.StopAsync(cancellationToken);

        if (@event == null)
        {
            _logger.LogWarning("Attempted to stop an event but no event exists");
            return NotFound();
        }

        return Ok(@event.ToResponse());
    }

    /// <summary>
    ///     Sets a per-user end time override for the current event
    /// </summary>
    /// <param name="userId">The ID of the user whose end time should be overridden</param>
    /// <param name="request">The override request containing the new end time</param>
    /// <param name="cancellationToken">The .NET cancellation token</param>
    /// <response code="200">The override was applied and the updated event status is returned</response>
    /// <response code="400">The user does not exist</response>
    /// <response code="401">Authentication has failed</response>
    /// <response code="403">You do not have permission to use this endpoint</response>
    /// <response code="404">No active event exists</response>
    [Authorize(Roles = SubmissionRoles.Judge)]
    [HttpPut("users/{userId:length(24)}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(EventResponse))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<EventResponse>> PutUserOverride(
        string userId,
        EventUserOverrideRequest request,
        CancellationToken cancellationToken = default)
    {
        var user = await _usersService.GetAsync(userId, cancellationToken);

        if (user == null)
        {
            _logger.LogWarning("User with ID {UserId} not found when setting event end time override", userId.SanitizeForLogging());
            return BadRequest(ApiErrorEntityNotFound("User", userId));
        }

        _logger.LogInformation("Setting event end time override for user {UserId} to {EndTime}", userId.SanitizeForLogging(), request.EndTime);

        var @event = await _eventService.SetUserEndTimeOverrideAsync(userId, request.EndTime, cancellationToken);

        if (@event == null)
        {
            _logger.LogWarning("Attempted to set a user end time override but no event exists");
            return NotFound();
        }

        return Ok(@event.ToResponse());
    }
}
