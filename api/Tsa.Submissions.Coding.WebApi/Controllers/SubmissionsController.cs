using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Tsa.Submissions.Coding.Contracts;
using Tsa.Submissions.Coding.Contracts.Messages;
using Tsa.Submissions.Coding.Contracts.Pagination;
using Tsa.Submissions.Coding.Contracts.Submissions;
using Tsa.Submissions.Coding.Contracts.TestCases;
using Tsa.Submissions.Coding.WebApi.Authorization;
using Tsa.Submissions.Coding.WebApi.Entities;
using Tsa.Submissions.Coding.WebApi.ExtensionMethods;
using Tsa.Submissions.Coding.WebApi.Pagination;
using Tsa.Submissions.Coding.WebApi.Services;

namespace Tsa.Submissions.Coding.WebApi.Controllers;

[Route("api/submissions")]
[ApiController]
[Produces("application/json")]
public class SubmissionsController : WebApiBaseController
{
    private readonly ILogger<SubmissionsController> _logger;
    private readonly IProblemsService _problemsService;
    private readonly IProgrammingLanguagesService _programmingLanguagesService;
    private readonly IValidator<SubmissionCreateRequest> _submissionCreateRequestValidator;
    private readonly ISubmissionsQueueService _submissionsQueueService;
    private readonly ISubmissionsService _submissionsService;
    private readonly IUsersService _usersService;

    public SubmissionsController(
        ILogger<SubmissionsController> logger,
        IProblemsService problemsService,
        IProgrammingLanguagesService programmingLanguagesService,
        IValidator<SubmissionCreateRequest> submissionCreateRequestValidator,
        ISubmissionsService submissionsService,
        ISubmissionsQueueService submissionsQueueService,
        IUsersService usersService)
    {
        _logger = logger;
        _problemsService = problemsService;
        _programmingLanguagesService = programmingLanguagesService;
        _submissionCreateRequestValidator = submissionCreateRequestValidator;
        _submissionsService = submissionsService;
        _submissionsQueueService = submissionsQueueService;
        _usersService = usersService;
    }

    /// <summary>
    ///     Fetches a paginated list of submissions from the database
    /// </summary>
    /// <param name="cursor">The cursor for pagination (ID of the last submission from previous page)</param>
    /// <param name="pageSize">The number of items per page (default: 20, max: 100)</param>
    /// <param name="sortOrder">The order to sort the ID of the submissions on</param>
    /// <param name="cancellationToken">The .NET cancellation token</param>
    /// <response code="200">All available submissions returned</response>
    [Authorize(Roles = SubmissionRoles.All)]
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(PaginatedResponse<SubmissionListResponse>))]
    public async Task<ActionResult<PaginatedResponse<SubmissionListResponse>>> Get(
        [FromQuery]string? cursor = null,
        [FromQuery]int pageSize = 20,
        [FromQuery]string sortOrder = "desc",
        CancellationToken cancellationToken = default)
    {
        PaginationSortOrder paginationSortOrder;

        if (string.Equals(sortOrder, "asc", StringComparison.InvariantCultureIgnoreCase))
        {
            paginationSortOrder = PaginationSortOrder.Ascending;
        }
        else if (string.Equals(sortOrder, "desc", StringComparison.InvariantCultureIgnoreCase))
        {
            paginationSortOrder = PaginationSortOrder.Descending;
        }
        else
        {
            // Ignore bad values for sortOrder and default to descending
            paginationSortOrder = PaginationSortOrder.Descending;
        }

        var pagination = new CursorPagination
        {
            Cursor = cursor,
            PageSize = pageSize,
            SortOrder = paginationSortOrder
        };

        // Null-forgiveness operator is used below since all those values are required to be at this stage
        // If missing, let an exception bubble up

        var user = await _usersService.GetByUserNameAsync(User.Identity!.Name!, cancellationToken);

        var submissions = User.IsInRole(SubmissionRoles.Judge) || User.IsInRole(SubmissionRoles.System)
            ? await _submissionsService.GetPagedByIdCursorAsync(pagination, cancellationToken)
            : await _submissionsService.GetPagedByUserIdCursorAsync(user!.Id!, pagination, cancellationToken);

        // May need to optimize this pulls the full problem which can be large
        // This data is also cached at the API level, so it shouldn't be a problem for now
        // TODO: Load test this and optimize if necessary (e.g. only pull problem titles instead of full problems)
        // TODO: Establish a metrics dashboard to monitor the performance of this endpoint and identify bottlenecks like this
        var problems = await _problemsService.GetAsync(cancellationToken);

        var programmingLanguages = await _programmingLanguagesService.GetAsync(cancellationToken);

        var users = await _usersService.GetAsync(cancellationToken);

        return submissions.ToPaginatedResponse(problems, programmingLanguages, users);
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
    public async Task<ActionResult<SubmissionResponse>> GetById(string id, CancellationToken cancellationToken = default)
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

        var user = await _usersService.GetByUserNameAsync(User.Identity!.Name!, cancellationToken);

        // Null-forgiving operator is used for entries on Submission because the values are required
        // If they are missing, there is a data corruption and we need exceptions to throw
        if (User.IsInRole(SubmissionRoles.Judge) ||
            User.IsInRole(SubmissionRoles.System) ||
            submission.UserId == user!.Id)
        {
            var programmingLanguage = await _programmingLanguagesService.GetAsync(submission.ProgrammingLanguageId!, cancellationToken);
            var problem = await _problemsService.GetAsync(submission.ProblemId!, cancellationToken);

            _logger.LogInformation(
                "The user {UserName} is authorized to view Submission {Id}; [IsJudge:{IsJudge}, IsSystem:{IsSystem}, IsOwner:{IsOwner}]",
                User.Identity.Name.SanitizeForLogging(),
                id.SanitizeForLogging(),
                User.IsInRole(SubmissionRoles.Judge),
                User.IsInRole(SubmissionRoles.System),
                submission.UserId == user!.Id);


            return submission.ToResponse(programmingLanguage!, problem!, user);
        }

        _logger.LogWarning(
            "The user {UserName} is not authorized to view Submission {Id}; [IsJudge:{IsJudge}, IsSystem:{IsSystem}, IsOwner:{IsOwner}]",
            User.Identity.Name.SanitizeForLogging(),
            id.SanitizeForLogging(),
            User.IsInRole(SubmissionRoles.Judge),
            User.IsInRole(SubmissionRoles.System),
            submission.UserId == user.Id);
        return NotFound();
    }

    /// <summary>
    ///     Fetches a paginated list of submissions from the database for the given user
    /// </summary>
    /// <param name="userId">The ID of the user to fetch submissions for</param>
    /// <param name="cursor">The cursor for pagination (ID of the last submission from previous page)</param>
    /// <param name="pageSize">The number of items per page (default: 20, max: 100)</param>
    /// <param name="sortOrder">The order to sort the ID of the submissions on</param>
    /// <param name="cancellationToken">The .NET cancellation token</param>
    /// <response code="200">All available submissions returned</response>
    [Authorize(Roles = SubmissionRoles.All)]
    [HttpGet("users/{userId:length(24)}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(PaginatedResponse<SubmissionListResponse>))]
    [ProducesResponseType(StatusCodes.Status424FailedDependency, Type = typeof(ApiErrorResponse))]
    public async Task<ActionResult<PaginatedResponse<SubmissionListResponse>>> GetByUserId(
        string userId,
        [FromQuery]string? cursor = null,
        [FromQuery]int pageSize = 20,
        [FromQuery]string sortOrder = "desc",
        CancellationToken cancellationToken = default)
    {
        if (User.IsInRole(SubmissionRoles.Participant))
        {
            var user = await _usersService.GetByUserNameAsync(User.Identity!.Name!, cancellationToken);

            if (user!.Id != userId)
            {
                _logger.LogWarning("User {UserName} attempted to access submissions for user ID {UserId}", User.Identity.Name.SanitizeForLogging(),
                    userId.SanitizeForLogging());
                return Forbid();
            }
        }

        PaginationSortOrder paginationSortOrder;

        if (string.Equals(sortOrder, "asc", StringComparison.InvariantCultureIgnoreCase))
        {
            paginationSortOrder = PaginationSortOrder.Ascending;
        }
        else if (string.Equals(sortOrder, "desc", StringComparison.InvariantCultureIgnoreCase))
        {
            paginationSortOrder = PaginationSortOrder.Descending;
        }
        else
        {
            // Ignore bad values for sortOrder and default to descending
            paginationSortOrder = PaginationSortOrder.Descending;
        }

        var pagination = new CursorPagination
        {
            Cursor = cursor,
            PageSize = pageSize,
            SortOrder = paginationSortOrder
        };

        var submissions = await _submissionsService.GetPagedByUserIdCursorAsync(userId, pagination, cancellationToken);

        // May need to optimize this pulls the full problem which can be large
        // This data is also cached at the API level, so it shouldn't be a problem for now
        // TODO: Load test this and optimize if necessary (e.g. only pull problem titles instead of full problems)
        // TODO: Establish a metrics dashboard to monitor the performance of this endpoint and identify bottlenecks like this
        var problems = await _problemsService.GetAsync(cancellationToken);

        var programmingLanguages = await _programmingLanguagesService.GetAsync(cancellationToken);

        var users = await _usersService.GetAsync(cancellationToken);

        return submissions.ToPaginatedResponse(problems, programmingLanguages, users);
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

        var validationResult = await ValidateAsync(submissionCreateRequest, _submissionCreateRequestValidator, cancellationToken);

        if (!validationResult.IsValid)
        {
            return BadRequest(validationResult.GetError());
        }

        _logger.LogInformation(
            "Creating submission for problem ID {ProblemId} for user {UserName}",
            submissionCreateRequest.ProblemId.SanitizeForLogging(),
            User.Identity?.Name.SanitizeForLogging() ?? "Unknown");

        var user = await _usersService.GetByUserNameAsync(User.Identity!.Name!, cancellationToken);

        if (user == null)
        {
            _logger.LogWarning("User {UserName} not found", User.Identity?.Name?.SanitizeForLogging() ?? "Unknown");
            return Forbid();
        }

        var problem = await _problemsService.GetAsync(submissionCreateRequest.ProblemId, cancellationToken);

        if (problem == null)
        {
            _logger.LogWarning("Problem with ID {ProblemId} not found", submissionCreateRequest.ProblemId.SanitizeForLogging());
            return BadRequest(ApiErrorEntityNotFound("Problem", submissionCreateRequest.ProblemId.SanitizeForLogging()));
        }

        var programmingLanguage = await _programmingLanguagesService.GetAsync(submissionCreateRequest.ProgrammingLanguageId, cancellationToken);

        if (programmingLanguage == null)
        {
            _logger.LogWarning(
                "Programming language with ID {ProgrammingLanguageId} not found",
                submissionCreateRequest.ProgrammingLanguageId.SanitizeForLogging());
            return BadRequest(ApiErrorEntityNotFound("Programming Language", submissionCreateRequest.ProgrammingLanguageId.SanitizeForLogging()));
        }

        var programmingLanguageVersion =
            programmingLanguage.Versions.SingleOrDefault(v => v.VersionTag == submissionCreateRequest.ProgrammingLanguageVersionTag);

        if (programmingLanguageVersion == null)
        {
            _logger.LogWarning(
                "Programming language version with tag {ProgrammingLanguageVersionTag} for programming language ID {ProgrammingLanguageId} not found",
                submissionCreateRequest.ProgrammingLanguageVersionTag.SanitizeForLogging(),
                submissionCreateRequest.ProgrammingLanguageId.SanitizeForLogging());
            return BadRequest(ApiErrorEntityNotFound("Programming Language Version",
                submissionCreateRequest.ProgrammingLanguageVersionTag.SanitizeForLogging()));
        }

        var submission = new Submission
        {
            ProgrammingLanguageId = programmingLanguage.Id,
            ProgrammingLanguageVersionTag = programmingLanguageVersion.VersionTag,
            ProblemId = problem.Id,
            Solution = submissionCreateRequest.Solution,
            SubmittedOn = submittedOn,
            UserId = user.Id
        };

        _logger.LogInformation("Creating the submission for problem ID {ProblemId} by user {UserName}",
            submission.ProblemId.SanitizeForLogging(),
            User.Identity?.Name.SanitizeForLogging() ?? "Unknown");
        await _submissionsService.CreateAsync(submission, cancellationToken);

        // Null-forgiving operator is used here because the ID, ProblemId, and UserId are set when the submission is created
        var submissionMessage = new SubmissionMessage(
            submission.ProblemId!,
            submission.Id!,
            submittedOn,
            submission.UserId!);

        _logger.LogInformation("Enqueuing submission message for submission ID {SubmissionId}", submission.Id.SanitizeForLogging());
        await _submissionsQueueService.EnqueueSubmissionAsync(submissionMessage, cancellationToken);

        return CreatedAtAction(nameof(Get), new { id = submission.Id }, submission.ToResponse(programmingLanguage, problem, user));
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

        submission.TestCaseResults.AddRange(ToEntityList(submissionModifyRequest.TestCaseResults));

        await _submissionsService.UpdateAsync(submission, cancellationToken);

        return NoContent();
    }

    private static TestCaseResult ToEntity(TestCaseResultRequest testCaseResultRequest)
    {
        return new TestCaseResult
        {
            ActualOutput = testCaseResultRequest.ActualOutput,
            ExecutionTime = testCaseResultRequest.ExecutionTime,
            Message = testCaseResultRequest.Message,
            Passed = testCaseResultRequest.Passed,
            TestCaseId = testCaseResultRequest.TestCaseId,
            TimedOut = testCaseResultRequest.TimedOut
        };
    }

    private static IEnumerable<TestCaseResult> ToEntityList(IEnumerable<TestCaseResultRequest> testCaseResultRequests)
    {
        return testCaseResultRequests.Select(ToEntity);
    }
}
