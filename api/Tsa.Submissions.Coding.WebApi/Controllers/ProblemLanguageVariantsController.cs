using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Tsa.Submissions.Coding.Contracts;
using Tsa.Submissions.Coding.Contracts.ProblemLanguageVariants;
using Tsa.Submissions.Coding.WebApi.Authorization;
using Tsa.Submissions.Coding.WebApi.Entities;
using Tsa.Submissions.Coding.WebApi.Services;

namespace Tsa.Submissions.Coding.WebApi.Controllers;

/// <summary>
///     Manages language-specific variants for coding problems.
///     Each variant defines the workspace, test harness, and reference solution
///     for a specific programming language and version combination.
/// </summary>
[Route("api/problems/{problemId:length(24)}/language-variants")]
[ApiController]
[Produces("application/json")]
public class ProblemLanguageVariantsController : WebApiBaseController
{
    private readonly IValidator<ProblemLanguageVariantRequest> _problemLanguageVariantRequestValidator;
    private readonly IProblemLanguageVariantsService _problemLanguageVariantsService;
    private readonly IProblemsService _problemsService;
    private readonly IProgrammingLanguagesService _programmingLanguagesService;

    public ProblemLanguageVariantsController(
        IProblemLanguageVariantsService problemLanguageVariantsService,
        IValidator<ProblemLanguageVariantRequest> problemLanguageVariantRequestValidator,
        IProblemsService problemsService,
        IProgrammingLanguagesService programmingLanguagesService)
    {
        _problemLanguageVariantsService = problemLanguageVariantsService;
        _problemLanguageVariantRequestValidator = problemLanguageVariantRequestValidator;
        _problemsService = problemsService;
        _programmingLanguagesService = programmingLanguagesService;
    }

    /// <summary>
    ///     Deletes a language variant from a problem.
    /// </summary>
    /// <param name="problemId">The ID of the problem</param>
    /// <param name="id">The ID of the language variant to delete</param>
    /// <param name="cancellationToken">The cancellation token</param>
    /// <response code="204">Acknowledgement that the language variant was deleted</response>
    /// <response code="401">Authentication has failed</response>
    /// <response code="403">You do not have permission to use this endpoint</response>
    /// <response code="404">The problem or language variant does not exist</response>
    [Authorize(Roles = SubmissionRoles.Judge)]
    [HttpDelete("{id:length(24)}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(ApiErrorResponse))]
    [ProducesResponseType(StatusCodes.Status403Forbidden, Type = typeof(ApiErrorResponse))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ApiErrorResponse))]
    public async Task<IActionResult> Delete(string problemId, string id, CancellationToken cancellationToken = default)
    {
        var problem = await _problemsService.GetAsync(problemId, cancellationToken);

        if (problem == null) return NotFound(ApiErrorEntityNotFound(nameof(Problem), problemId));

        var variant = await _problemLanguageVariantsService.GetAsync(id, cancellationToken);

        if (variant == null) return NotFound(ApiErrorEntityNotFound(nameof(ProblemLanguageVariant), id));

        if (variant.ProblemId != problemId)
        {
            return NotFound(ApiErrorEntityNotFound(nameof(ProblemLanguageVariant), id));
        }

        await _problemLanguageVariantsService.RemoveAsync(variant, cancellationToken);

        return NoContent();
    }

    /// <summary>
    ///     Gets all language variants for a specific problem.
    /// </summary>
    /// <param name="problemId">The ID of the problem</param>
    /// <param name="cancellationToken">The cancellation token</param>
    /// <response code="200">Returns the language variants for the problem</response>
    /// <response code="401">Authentication has failed</response>
    /// <response code="404">The problem does not exist</response>
    [Authorize(Roles = SubmissionRoles.All)]
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<ProblemLanguageVariantListResponse>))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(ApiErrorResponse))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ApiErrorResponse))]
    public async Task<ActionResult<IEnumerable<ProblemLanguageVariantListResponse>>> Get(string problemId, CancellationToken cancellationToken = default)
    {
        var problem = await _problemsService.GetAsync(problemId, cancellationToken);

        if (problem == null) return NotFound(ApiErrorEntityNotFound(nameof(Problem), problemId));

        var variants = await _problemLanguageVariantsService.GetByProblemAsync(problem, cancellationToken);

        return Ok(variants.ToListResponses().ToList());
    }

    /// <summary>
    ///     Gets a specific language variant for a problem.
    /// </summary>
    /// <param name="problemId">The ID of the problem</param>
    /// <param name="id">The ID of the language variant</param>
    /// <param name="cancellationToken">The cancellation token</param>
    /// <response code="200">Returns the requested language variant</response>
    /// <response code="401">Authentication has failed</response>
    /// <response code="404">The problem or language variant does not exist</response>
    [Authorize(Roles = SubmissionRoles.All)]
    [HttpGet("{id:length(24)}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ProblemLanguageVariantResponse))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(ApiErrorResponse))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ApiErrorResponse))]
    public async Task<ActionResult<ProblemLanguageVariantResponse>> Get(string problemId, string id, CancellationToken cancellationToken = default)
    {
        var problem = await _problemsService.GetAsync(problemId, cancellationToken);

        if (problem == null) return NotFound(ApiErrorEntityNotFound(nameof(Problem), problemId));

        var variant = await _problemLanguageVariantsService.GetAsync(id, cancellationToken);

        if (variant == null) return NotFound(ApiErrorEntityNotFound(nameof(ProblemLanguageVariant), id));

        if (variant.ProblemId != problemId)
        {
            return NotFound(ApiErrorEntityNotFound(nameof(ProblemLanguageVariant), id));
        }

        return Ok(variant.ToResponse());
    }

    /// <summary>
    ///     Creates a new language variant for a problem.
    /// </summary>
    /// <param name="problemId">The ID of the problem</param>
    /// <param name="problemLanguageVariantRequest">The language variant to create</param>
    /// <param name="cancellationToken">The cancellation token</param>
    /// <response code="201">Returns the created language variant</response>
    /// <response code="400">The language variant is not in a valid state</response>
    /// <response code="401">Authentication has failed</response>
    /// <response code="403">You do not have permission to use this endpoint</response>
    /// <response code="404">The problem or programming language does not exist</response>
    /// <response code="409">A variant for this language and version already exists</response>
    [Authorize(Roles = SubmissionRoles.Judge)]
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(ProblemLanguageVariantResponse))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ValidationProblemDetails))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(ApiErrorResponse))]
    [ProducesResponseType(StatusCodes.Status403Forbidden, Type = typeof(ApiErrorResponse))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ApiErrorResponse))]
    [ProducesResponseType(StatusCodes.Status409Conflict, Type = typeof(ApiErrorResponse))]
    public async Task<IActionResult> Post(string problemId, ProblemLanguageVariantRequest problemLanguageVariantRequest,
        CancellationToken cancellationToken = default)
    {
        var problem = await _problemsService.GetAsync(problemId, cancellationToken);

        if (problem == null) return NotFound(ApiErrorEntityNotFound(nameof(Problem), problemId));

        var validationResult = await ValidateAsync(problemLanguageVariantRequest, _problemLanguageVariantRequestValidator, cancellationToken);

        if (!validationResult.IsValid)
        {
            return BadRequest(validationResult.GetError());
        }

        var programmingLanguage = await _programmingLanguagesService.GetAsync(problemLanguageVariantRequest.ProgrammingLanguageId, cancellationToken);

        if (programmingLanguage == null)
        {
            return NotFound(ApiErrorEntityNotFound(nameof(ProgrammingLanguage), problemLanguageVariantRequest.ProgrammingLanguageId));
        }

        if (await _problemLanguageVariantsService.ExistsForProblemLanguageAndVersionAsync(
                problemId,
                problemLanguageVariantRequest.ProgrammingLanguageId,
                problemLanguageVariantRequest.ProgrammingLanguageVersionTag,
                cancellationToken))
        {
            var entityKey =
                $"{problemId}.{problemLanguageVariantRequest.ProgrammingLanguageId}.{problemLanguageVariantRequest.ProgrammingLanguageVersionTag}";
            return Conflict(ApiErrorEntityAlreadyExists(nameof(ProblemLanguageVariant), entityKey));
        }

        var variant = ToEntity(problemLanguageVariantRequest, problemId);

        await _problemLanguageVariantsService.CreateAsync(variant, cancellationToken);

        var variantResponse = variant.ToResponse();

        return CreatedAtAction(nameof(Get), new { problemId, id = variant.Id }, variantResponse);
    }

    /// <summary>
    ///     Updates a language variant for a problem.
    /// </summary>
    /// <param name="problemId">The ID of the problem</param>
    /// <param name="id">The ID of the language variant to update</param>
    /// <param name="problemLanguageVariantRequest">The updated language variant data</param>
    /// <param name="cancellationToken">The cancellation token</param>
    /// <response code="204">Acknowledgement that the language variant was updated</response>
    /// <response code="400">The language variant is not in a valid state</response>
    /// <response code="401">Authentication has failed</response>
    /// <response code="403">You do not have permission to use this endpoint</response>
    /// <response code="404">The problem or language variant does not exist</response>
    [Authorize(Roles = SubmissionRoles.Judge)]
    [HttpPut("{id:length(24)}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ValidationProblemDetails))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(ApiErrorResponse))]
    [ProducesResponseType(StatusCodes.Status403Forbidden, Type = typeof(ApiErrorResponse))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ApiErrorResponse))]
    public async Task<IActionResult> Put(string problemId, string id, ProblemLanguageVariantRequest problemLanguageVariantRequest,
        CancellationToken cancellationToken = default)
    {
        var problem = await _problemsService.GetAsync(problemId, cancellationToken);

        if (problem == null) return NotFound(ApiErrorEntityNotFound(nameof(Problem), problemId));

        var validationResult = await ValidateAsync(problemLanguageVariantRequest, _problemLanguageVariantRequestValidator, cancellationToken);

        if (!validationResult.IsValid)
        {
            return BadRequest(validationResult.GetError());
        }

        var variant = await _problemLanguageVariantsService.GetAsync(id, cancellationToken);

        if (variant == null) return NotFound(ApiErrorEntityNotFound(nameof(ProblemLanguageVariant), id));

        if (variant.ProblemId != problemId)
        {
            return NotFound(ApiErrorEntityNotFound(nameof(ProblemLanguageVariant), id));
        }

        await _problemLanguageVariantsService.UpdateAsync(ToEntity(problemLanguageVariantRequest, problemId, id), cancellationToken);

        return NoContent();
    }

    private static ProblemLanguageVariant ToEntity(ProblemLanguageVariantRequest request, string problemId, string? id = null)
    {
        return new ProblemLanguageVariant
        {
            BaselineMetrics = request.BaselineMetrics == null
                ? null
                : new CodeMetrics
                {
                    CyclomaticComplexity = request.BaselineMetrics.CyclomaticComplexity,
                    ExecutionTimeInMs = request.BaselineMetrics.ExecutionTimeInMs,
                    LinesOfCode = request.BaselineMetrics.LinesOfCode
                },
            Id = id,
            IsActive = request.IsActive,
            ProblemId = problemId,
            ProgrammingLanguageId = request.ProgrammingLanguageId,
            ProgrammingLanguageVersionTag = request.ProgrammingLanguageVersionTag,
            ReferenceSolution = request.ReferenceSolution,
            StarterCode = request.StarterCode,
            TestHarnessCode = request.TestHarnessCode,
            WorkspaceFiles = request.WorkspaceFiles.Select(wf => new WorkspaceFile
            {
                Contents = wf.Contents,
                IsTemplate = wf.IsTemplate,
                Path = wf.Path,
                Source = wf.Source,
                Type = wf.Type
            }).ToList()
        };
    }
}
