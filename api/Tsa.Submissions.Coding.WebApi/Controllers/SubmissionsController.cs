using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Tsa.Submissions.Coding.WebApi.Authorization;
using Tsa.Submissions.Coding.WebApi.Entities;
using Tsa.Submissions.Coding.WebApi.ExtensionMethods;
using Tsa.Submissions.Coding.WebApi.Models;
using Tsa.Submissions.Coding.WebApi.Services;

namespace Tsa.Submissions.Coding.WebApi.Controllers;

[Route("api/[controller]")]
[ApiController]
[Produces("application/json")]
public class SubmissionsController : ControllerBase
{
    private readonly ILogger<SubmissionsController> _logger;
    private readonly ISubmissionsService _submissionsService;
    private readonly IUsersService _usersService;

    public SubmissionsController(ILogger<SubmissionsController> logger, ISubmissionsService submissionsService, IUsersService usersService)
    {
        _logger = logger;
        _submissionsService = submissionsService;
        _usersService = usersService;
    }

    /// <summary>
    ///     Fetches a submission from the database
    /// </summary>
    /// <param name="id">The ID of the submission to get</param>
    /// <param name="cancellationToken">The .NET cancellation token</param>
    /// <response code="200">Returns the requested submission</response>
    /// <response code="404">The submission does not exist in the database</response>
    [Authorize(Roles = SubmissionRoles.All)]
    [HttpGet("{id:length(24)}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(SubmissionModel))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<SubmissionModel>> Get(string id, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Fetching submission with ID {Id}", id.SanitizeForLogging());
        var sanitizedId = id.SanitizeForLogging();

        if (sanitizedId != id)
        {
            _logger.LogWarning("Submission ID {Id} is not valid", sanitizedId);
            return BadRequest(ApiErrorResponseModel.InvalidId);
        }

        var submission = await _submissionsService.GetAsync(id, cancellationToken);

        if (submission == null)
        {
            _logger.LogWarning("Submission with ID {Id} not found", id.SanitizeForLogging());
            return NotFound();
        }

        _logger.LogInformation("Submission with ID {Id} found", id.SanitizeForLogging());

        if (User.IsInRole(SubmissionRoles.Judge))
        {
            _logger.LogInformation("User is a judge, returning submission with ID {Id}", id.SanitizeForLogging());
            return submission.ToModel();
        }

        _logger.LogInformation("User is not a judge, checking if they are the owner of the submission with ID {Id}", id.SanitizeForLogging());

        var user = await _usersService.GetByUserNameAsync(User.Identity!.Name!, cancellationToken);

        return submission.User!.Id.AsString == user!.Id
            ? submission.ToModel()
            : NotFound();
    }

    /// <summary>
    ///     Fetches all the submissions from the database
    /// </summary>
    /// <param name="problemId">The ID of the problem to filter submissions by</param>
    /// <param name="cancellationToken">The .NET cancellation token</param>
    /// <response code="200">All available submissions returned</response>
    [Authorize(Roles = SubmissionRoles.All)]
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<SubmissionModel>))]
    [ProducesResponseType(StatusCodes.Status424FailedDependency, Type = typeof(ApiErrorResponseModel))]
    public async Task<ActionResult<IList<SubmissionModel>>> GetAll(
        [FromQuery] string? problemId = null,
        CancellationToken cancellationToken = default)
    {
        //TODO: Add pagination
        var submissions = string.IsNullOrEmpty(problemId)
            ? await _submissionsService.GetAsync(cancellationToken)
            : await _submissionsService.GetByProblemIdAsync(problemId, cancellationToken);

        if (User.IsInRole(SubmissionRoles.Judge)) return submissions.ToModels();

        var user = await _usersService.GetByUserNameAsync(User.Identity!.Name!, cancellationToken);

        return submissions
            // Team is required, if null, we are in a bad state
            .Where(submission => submission.User!.Id.AsString == user!.Id)
            .ToList()
            .ToModels();
    }

    /// <summary>
    ///     Creates a new submission
    /// </summary>
    /// <param name="submissionModel">The submission to be created</param>
    /// <param name="cancellationToken">The .NET cancellation token</param>
    /// <response code="201">Returns the requested submission</response>
    /// <response code="400">The submission is not in a valid state and cannot be created</response>
    /// <response code="403">You do not have permission to use this endpoint</response>
    [Authorize(Roles = SubmissionRoles.Judge)]
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ValidationProblemDetails))]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<CreatedAtActionResult> Post(SubmissionModel submissionModel, CancellationToken cancellationToken = default)
    {
        var submission = submissionModel.ToEntity();
        submission.Id = null;
        submission.SubmittedOn = DateTime.UtcNow;

        await _submissionsService.CreateAsync(submission, cancellationToken);

        submissionModel.Id = submission.Id;

        return CreatedAtAction(nameof(Get), new { id = submission.Id }, submissionModel);
    }

    /// <summary>
    ///     Updates the specified submission
    /// </summary>
    /// <param name="id">The ID of the submission to update</param>
    /// <param name="updatedSubmissionModel">The submission that should replace the one in the database</param>
    /// <param name="cancellationToken">The .NET cancellation token</param>
    /// <response code="204">Acknowledgement that the submission was updated</response>
    /// <response code="400">The submission is not in a valid state and cannot be updated</response>
    /// <response code="404">The submission requested to be updated could not be found</response>
    [Authorize(Roles = SubmissionRoles.Judge)]
    [HttpPut("{id:length(24)}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ValidationProblemDetails))]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Put(string id, SubmissionModel updatedSubmissionModel, CancellationToken cancellationToken = default)
    {
        var submission = await _submissionsService.GetAsync(id, cancellationToken);

        if (submission == null) return NotFound();

        updatedSubmissionModel.Id = submission.Id;

        // The following values are immutable and should not be updated
        updatedSubmissionModel.SubmittedOn = submission.SubmittedOn;
        updatedSubmissionModel.Solution = submission.Solution;

        await _submissionsService.UpdateAsync(updatedSubmissionModel.ToEntity(), cancellationToken);

        return NoContent();
    }
}
