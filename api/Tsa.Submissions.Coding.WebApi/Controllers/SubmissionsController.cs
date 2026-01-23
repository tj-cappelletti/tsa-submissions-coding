using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using Tsa.Submissions.Coding.Contracts;
using Tsa.Submissions.Coding.Contracts.Messages;
using Tsa.Submissions.Coding.Contracts.Submissions;
using Tsa.Submissions.Coding.WebApi.Authorization;
using Tsa.Submissions.Coding.WebApi.Entities;
using Tsa.Submissions.Coding.WebApi.ExtensionMethods;
using Tsa.Submissions.Coding.WebApi.Services;

namespace Tsa.Submissions.Coding.WebApi.Controllers;

[Route("api/[controller]")]
[ApiController]
[Produces("application/json")]
public class SubmissionsController : WebApiBaseController
{
    private readonly ILogger<SubmissionsController> _logger;
    private readonly IProblemsService _problemsService;
    private readonly ISubmissionsQueueService _submissionsQueueService;
    private readonly ISubmissionsService _submissionsService;
    private readonly IUsersService _usersService;

    public SubmissionsController(
        ILogger<SubmissionsController> logger,
        IProblemsService problemsService,
        ISubmissionsService submissionsService,
        ISubmissionsQueueService submissionsQueueService,
        IUsersService usersService)
    {
        _logger = logger;
        _problemsService = problemsService;
        _submissionsService = submissionsService;
        _submissionsQueueService = submissionsQueueService;
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
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(SubmissionResponse))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<SubmissionResponse>> Get(string id, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Fetching submission with ID {Id}", id.SanitizeForLogging());
        var sanitizedId = id.SanitizeForLogging();

        if (sanitizedId != id)
        {
            _logger.LogWarning("Submission ID {Id} is not valid", sanitizedId);
            return BadRequest(ApiErrorInvalidId());
        }

        var submission = await _submissionsService.GetAsync(id, cancellationToken);

        if (submission == null)
        {
            _logger.LogWarning("Submission with ID {Id} not found", id.SanitizeForLogging());
            return NotFound();
        }

        _logger.LogInformation("Submission with ID {Id} found", id.SanitizeForLogging());

        if (User.IsInRole(SubmissionRoles.Judge) || User.IsInRole(SubmissionRoles.System))
        {
            _logger.LogInformation("User is a judge or system, returning submission with ID {Id}", id.SanitizeForLogging());
            return submission.ToResponse();
        }

        _logger.LogInformation("User is not a judge, checking if they are the owner of the submission with ID {Id}", id.SanitizeForLogging());

        var user = await _usersService.GetByUserNameAsync(User.Identity!.Name!, cancellationToken);

        if (submission.User!.Id.AsString == user!.Id)
        {
            _logger.LogInformation("The user {UserId} is the owner of the submission with ID {SubmissionId}", user.Id, submission.Id);
            return submission.ToResponse();
        }

        _logger.LogWarning("The user {UserId} is not the owner of the submission with ID {SubmissionId}", user.Id, submission.Id);
        return NotFound();
    }

    /// <summary>
    ///     Fetches all the submissions from the database
    /// </summary>
    /// <param name="problemId">The ID of the problem to filter submissions by</param>
    /// <param name="cancellationToken">The .NET cancellation token</param>
    /// <response code="200">All available submissions returned</response>
    [Authorize(Roles = SubmissionRoles.All)]
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<SubmissionResponse>))]
    [ProducesResponseType(StatusCodes.Status424FailedDependency, Type = typeof(ApiErrorResponse))]
    public async Task<ActionResult<IList<SubmissionResponse>>> GetAll(
        [FromQuery]string? problemId = null,
        CancellationToken cancellationToken = default)
    {
        //TODO: Add pagination
        var submissions = string.IsNullOrEmpty(problemId)
            ? await _submissionsService.GetAsync(cancellationToken)
            : await _submissionsService.GetByProblemIdAsync(problemId, cancellationToken);

        if (User.IsInRole(SubmissionRoles.Judge)) return submissions.ToResponses().ToList();

        var user = await _usersService.GetByUserNameAsync(User.Identity!.Name!, cancellationToken);

        return submissions
            .Where(submission => submission.User!.Id.AsString == user!.Id)
            .ToResponses()
            .ToList();
    }

    /// <summary>
    ///     Creates a new submission
    /// </summary>
    /// <param name="submissionCreateRequest">The submission to be created</param>
    /// <param name="cancellationToken">The .NET cancellation token</param>
    /// <response code="201">Returns the requested submission</response>
    /// <response code="400">The submission is not in a valid state and cannot be created</response>
    /// <response code="403">You do not have permission to use this endpoint</response>
    [Authorize(Roles = SubmissionRoles.Participant)]
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ValidationProblemDetails))]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> Post(SubmissionCreateRequest submissionCreateRequest, CancellationToken cancellationToken = default)
    {
        var submittedOn = DateTimeOffset.UtcNow;

        _logger.LogInformation(
            "Creating submission for problem ID {ProblemId} for user {UserName}",
            submissionCreateRequest.ProblemId.SanitizeForLogging(),
            User.Identity?.Name.SanitizeForLogging() ?? "Unknown");

        if (string.IsNullOrWhiteSpace(User.Identity?.Name))
        {
            _logger.LogWarning("User identity name is null or whitespace");
            return Forbid();
        }

        var user = await _usersService.GetByUserNameAsync(User.Identity.Name, cancellationToken);

        if (user == null)
        {
            _logger.LogWarning("User {UserName} not found", User.Identity.Name.SanitizeForLogging());
            return Forbid();
        }

        var problem = await _problemsService.GetAsync(submissionCreateRequest.ProblemId, cancellationToken);

        if (problem == null)
        {
            _logger.LogWarning("Problem with ID {ProblemId} not found", submissionCreateRequest.ProblemId.SanitizeForLogging());
            return BadRequest(ApiErrorEntityNotFound("Problem", submissionCreateRequest.ProblemId.SanitizeForLogging()));
        }

        var submission = new Submission
        {
            Language = new ProgrammingLanguage
            {
                Name = submissionCreateRequest.Language.Name,
                Version = submissionCreateRequest.Language.Version
            },
            Problem = new MongoDBRef(ProblemsService.MongoDbCollectionName, problem.Id),
            Solution = submissionCreateRequest.Solution,
            SubmittedOn = submittedOn,
            User = new MongoDBRef(UsersService.MongoDbCollectionName, user.Id)
        };

        await _submissionsService.CreateAsync(submission, cancellationToken);

        var submissionMessage = new SubmissionMessage(
            submission.Problem.Id.AsString,
            submission.Id!,
            submittedOn,
            submission.User.Id.AsString);

        await _submissionsQueueService.EnqueueSubmissionAsync(submissionMessage, cancellationToken);

        return CreatedAtAction(nameof(Get), new { id = submission.Id }, submission.ToResponse());
    }

    /// <summary>
    ///     Updates the specified submission
    /// </summary>
    /// <param name="id">The ID of the submission to update</param>
    /// <param name="submissionModifyRequest">The submission that should replace the one in the database</param>
    /// <param name="cancellationToken">The .NET cancellation token</param>
    /// <response code="204">Acknowledgement that the submission was updated</response>
    /// <response code="400">The submission is not in a valid state and cannot be updated</response>
    /// <response code="404">The submission requested to be updated could not be found</response>
    [Authorize(Roles = SubmissionRoles.JudgeOrSystem)]
    [HttpPut("{id:length(24)}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ValidationProblemDetails))]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Put(string id, SubmissionModifyRequest submissionModifyRequest, CancellationToken cancellationToken = default)
    {
        var submission = await _submissionsService.GetAsync(id, cancellationToken);

        if (submission == null) return NotFound();

        if (submission.TestCaseResults.Count != 0)
        {
            _logger.LogWarning("Submission {SubmissionId} already has been evaluated and cannot be modified.", id);
            return BadRequest(ApiErrorSubmissionAlreadyEvaluated());
        }

        submission.EvaluatedOn = submissionModifyRequest.EvaluatedOn;

        submission.TestCaseResults.AddRange(submissionModifyRequest.TestCaseResults);

        await _submissionsService.UpdateAsync(submission, cancellationToken);

        return NoContent();
    }
}
