using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Tsa.Submissions.Coding.Contracts;
using Tsa.Submissions.Coding.Contracts.Languages;
using Tsa.Submissions.Coding.WebApi.Authorization;
using Tsa.Submissions.Coding.WebApi.Entities;
using Tsa.Submissions.Coding.WebApi.Services;

namespace Tsa.Submissions.Coding.WebApi.Controllers;

/// <summary>
///     Manages programming languages and their versions for the coding competition platform.
/// </summary>
[Route("api/programming-languages")]
[ApiController]
[Produces("application/json")]
public class ProgrammingLanguagesController : WebApiBaseController
{
    private readonly IValidator<ProgrammingLanguageRequest> _programmingLanguageRequestValidator;
    private readonly IProgrammingLanguagesService _programmingLanguagesService;
    private readonly IValidator<ProgrammingLanguageVersionRequest> _programmingLanguageVersionRequestValidator;

    /// <summary>
    ///     Initializes a new instance of the <see cref="ProgrammingLanguagesController" /> class.
    /// </summary>
    /// <param name="programmingLanguagesService">The programming languages service</param>
    /// <param name="programmingLanguageRequestValidator">The validator for programming language requests</param>
    /// <param name="programmingLanguageVersionRequestValidator">The validator for programming language version requests</param>
    public ProgrammingLanguagesController(
        IProgrammingLanguagesService programmingLanguagesService,
        IValidator<ProgrammingLanguageRequest> programmingLanguageRequestValidator,
        IValidator<ProgrammingLanguageVersionRequest> programmingLanguageVersionRequestValidator)
    {
        _programmingLanguagesService = programmingLanguagesService;
        _programmingLanguageRequestValidator = programmingLanguageRequestValidator;
        _programmingLanguageVersionRequestValidator = programmingLanguageVersionRequestValidator;
    }

    /// <summary>
    ///     Deletes a programming language from the system.
    /// </summary>
    /// <param name="id">The ID of the programming language to delete</param>
    /// <param name="cancellationToken">The cancellation token</param>
    /// <returns>No content if successful</returns>
    /// <response code="204">The programming language was successfully deleted</response>
    /// <response code="401">Authentication has failed</response>
    /// <response code="403">You do not have permission to use this endpoint</response>
    /// <response code="404">The programming language does not exist</response>
    [Authorize(Roles = SubmissionRoles.Judge)]
    [HttpDelete("{id:length(24)}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(ApiErrorResponse))]
    [ProducesResponseType(StatusCodes.Status403Forbidden, Type = typeof(ApiErrorResponse))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ApiErrorResponse))]
    public async Task<IActionResult> Delete(string id, CancellationToken cancellationToken = default)
    {
        var programmingLanguage = await _programmingLanguagesService.GetAsync(id, cancellationToken);

        if (programmingLanguage == null) return NotFound();

        await _programmingLanguagesService.RemoveAsync(programmingLanguage, cancellationToken);

        return NoContent();
    }

    /// <summary>
    ///     Deletes a programming language version from the system.
    /// </summary>
    /// <param name="id">The ID of the programming language to delete</param>
    /// <param name="programmingLanguageVersionRequest">The request containing the version to delete</param>
    /// <param name="cancellationToken">The cancellation token</param>
    /// <returns>No content if successful</returns>
    /// <response code="204">The programming language version was successfully deleted</response>
    /// <response code="401">Authentication has failed</response>
    /// <response code="403">You do not have permission to use this endpoint</response>
    /// <response code="404">The programming language does not exist</response>
    [Authorize(Roles = SubmissionRoles.Judge)]
    [HttpDelete("{id:length(24)}/versions")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(ApiErrorResponse))]
    [ProducesResponseType(StatusCodes.Status403Forbidden, Type = typeof(ApiErrorResponse))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ApiErrorResponse))]
    public async Task<IActionResult> DeleteProgrammingLanguageVersion(string id, ProgrammingLanguageVersionRequest programmingLanguageVersionRequest,
        CancellationToken cancellationToken = default)
    {
        var programmingLanguage = await _programmingLanguagesService.GetAsync(id, cancellationToken);

        if (programmingLanguage == null) return NotFound();

        var programmingLanguageVersion =
            programmingLanguage.Versions.FirstOrDefault(languageVersion => languageVersion.VersionTag == programmingLanguageVersionRequest.VersionTag);

        if (programmingLanguageVersion == null) return NotFound();

        programmingLanguage.Versions.Remove(programmingLanguageVersion);

        await _programmingLanguagesService.UpdateAsync(programmingLanguage, cancellationToken);

        return NoContent();
    }

    /// <summary>
    ///     Gets all programming languages available in the system.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token</param>
    /// <returns>A list of all programming languages</returns>
    /// <response code="200">Returns all available programming languages</response>
    /// <response code="401">Authentication has failed</response>
    [Authorize(Roles = SubmissionRoles.All)]
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<ProgrammingLanguageResponse>))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(ApiErrorResponse))]
    public async Task<ActionResult<IList<ProgrammingLanguageResponse>>> Get(CancellationToken cancellationToken = default)
    {
        var programmingLanguages = await _programmingLanguagesService.GetAsync(cancellationToken);

        return programmingLanguages.Count == 0
            ? []
            : programmingLanguages.ToResponses().ToList();
    }

    /// <summary>
    ///     Gets a specific programming language by its ID.
    /// </summary>
    /// <param name="id">The ID of the programming language to retrieve</param>
    /// <param name="cancellationToken">The cancellation token</param>
    /// <returns>The requested programming language</returns>
    /// <response code="200">Returns the requested programming language</response>
    /// <response code="401">Authentication has failed</response>
    /// <response code="404">The programming language does not exist</response>
    [Authorize(Roles = SubmissionRoles.All)]
    [HttpGet("{id:length(24)}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ProgrammingLanguageResponse))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(ApiErrorResponse))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ApiErrorResponse))]
    public async Task<ActionResult<ProgrammingLanguageResponse>> Get(string id, CancellationToken cancellationToken = default)
    {
        var programmingLanguage = await _programmingLanguagesService.GetAsync(id, cancellationToken);

        return programmingLanguage == null
            ? NotFound()
            : Ok(programmingLanguage.ToResponse());
    }

    /// <summary>
    ///     Gets all versions for a specific programming language.
    /// </summary>
    /// <param name="id">The ID of the programming language</param>
    /// <param name="cancellationToken">The cancellation token</param>
    /// <returns>A list of versions for the programming language</returns>
    /// <response code="200">Returns all versions for the programming language</response>
    /// <response code="401">Authentication has failed</response>
    /// <response code="404">The programming language does not exist</response>
    [Authorize(Roles = SubmissionRoles.All)]
    [HttpGet("{id:length(24)}/versions")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<ProgrammingLanguageVersionResponse>))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(ApiErrorResponse))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ApiErrorResponse))]
    public async Task<ActionResult<IList<ProgrammingLanguageVersionResponse>>> GetVersions(string id, CancellationToken cancellationToken = default)
    {
        var programmingLanguage = await _programmingLanguagesService.GetAsync(id, cancellationToken);

        return programmingLanguage == null
            ? NotFound()
            : Ok(programmingLanguage.Versions.ToResponses().ToList());
    }

    /// <summary>
    ///     Creates a new programming language in the system.
    /// </summary>
    /// <param name="programmingLanguageRequest">The programming language to create</param>
    /// <param name="cancellationToken">The cancellation token</param>
    /// <returns>The created programming language</returns>
    /// <response code="201">Returns the newly created programming language</response>
    /// <response code="400">The programming language request is invalid</response>
    /// <response code="401">Authentication has failed</response>
    /// <response code="403">You do not have permission to use this endpoint</response>
    /// <response code="409">A programming language with the same name already exists</response>
    [Authorize(Roles = SubmissionRoles.Judge)]
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(ProgrammingLanguageResponse))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ValidationProblemDetails))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(ApiErrorResponse))]
    [ProducesResponseType(StatusCodes.Status403Forbidden, Type = typeof(ApiErrorResponse))]
    [ProducesResponseType(StatusCodes.Status409Conflict, Type = typeof(ApiErrorResponse))]
    public async Task<IActionResult> Post(ProgrammingLanguageRequest programmingLanguageRequest, CancellationToken cancellationToken = default)
    {
        var validationResult = await ValidateAsync(programmingLanguageRequest, _programmingLanguageRequestValidator, cancellationToken);

        if (!validationResult.IsValid)
        {
            return validationResult.GetError();
        }

        var existingProgrammingLanguages = await _programmingLanguagesService.GetAsync(cancellationToken);

        if (existingProgrammingLanguages.Any(pl => pl.Name == programmingLanguageRequest.Name))
        {
            return Conflict(ApiErrorEntityAlreadyExists(nameof(ProgrammingLanguage), programmingLanguageRequest.Name));
        }

        var programmingLanguage = ToEntity(programmingLanguageRequest);

        await _programmingLanguagesService.CreateAsync(programmingLanguage, cancellationToken);

        var programmingLanguageResponse = programmingLanguage.ToResponse();

        return CreatedAtAction(nameof(Get), new { id = programmingLanguage.Id }, programmingLanguageResponse);
    }

    /// <summary>
    ///     Adds a new version to an existing programming language.
    /// </summary>
    /// <param name="id">The ID of the programming language</param>
    /// <param name="programmingLanguageVersionRequest">The version to add</param>
    /// <param name="cancellationToken">The cancellation token</param>
    /// <returns>The updated programming language with the new version</returns>
    /// <response code="201">Returns the programming language with the newly added version</response>
    /// <response code="400">The version request is invalid</response>
    /// <response code="401">Authentication has failed</response>
    /// <response code="403">You do not have permission to use this endpoint</response>
    /// <response code="404">The programming language does not exist</response>
    /// <response code="409">A version with the same display name or version tag already exists</response>
    /// <remarks>
    ///     If the new version is marked as default, all other versions will be set to non-default.
    /// </remarks>
    [Authorize(Roles = SubmissionRoles.Judge)]
    [HttpPost("{id:length(24)}/versions")]
    [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(ProgrammingLanguageResponse))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ValidationProblemDetails))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(ApiErrorResponse))]
    [ProducesResponseType(StatusCodes.Status403Forbidden, Type = typeof(ApiErrorResponse))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ApiErrorResponse))]
    [ProducesResponseType(StatusCodes.Status409Conflict, Type = typeof(ApiErrorResponse))]
    public async Task<IActionResult> PostProgrammingLanguageVersion(string id, ProgrammingLanguageVersionRequest programmingLanguageVersionRequest,
        CancellationToken cancellationToken = default)
    {
        var programmingLanguage = await _programmingLanguagesService.GetAsync(id, cancellationToken);

        if (programmingLanguage == null) return NotFound();

        var validationResult = await ValidateAsync(programmingLanguageVersionRequest, _programmingLanguageVersionRequestValidator, cancellationToken);

        if (!validationResult.IsValid)
        {
            return validationResult.GetError();
        }

        var programmingLanguageVersion = ToEntity(programmingLanguageVersionRequest);

        if (programmingLanguage.Versions.Any(v => v.DisplayName == programmingLanguageVersion.DisplayName) ||
            programmingLanguage.Versions.Any(v => v.VersionTag == programmingLanguageVersion.VersionTag))
        {
            var entityKey = $"{programmingLanguageVersionRequest.DisplayName}.{programmingLanguageVersionRequest.VersionTag}";

            return Conflict(ApiErrorEntityAlreadyExists(nameof(ProgrammingLanguageVersion), entityKey));
        }

        if (programmingLanguageVersion.IsDefault)
        {
            foreach (var version in programmingLanguage.Versions)
            {
                version.IsDefault = false;
            }
        }

        programmingLanguage.Versions.Add(programmingLanguageVersion);

        await _programmingLanguagesService.UpdateAsync(programmingLanguage, cancellationToken);

        var programmingLanguageResponse = programmingLanguage.ToResponse();

        return CreatedAtAction(nameof(Get), new { id = programmingLanguage.Id }, programmingLanguageResponse);
    }

    /// <summary>
    ///     Updates an existing programming language.
    /// </summary>
    /// <param name="id">The ID of the programming language to update</param>
    /// <param name="programmingLanguageRequest">The updated programming language data</param>
    /// <param name="cancellationToken">The cancellation token</param>
    /// <returns>No content if successful</returns>
    /// <response code="204">The programming language was successfully updated</response>
    /// <response code="400">The programming language request is invalid</response>
    /// <response code="401">Authentication has failed</response>
    /// <response code="403">You do not have permission to use this endpoint</response>
    /// <response code="404">The programming language does not exist</response>
    /// <remarks>
    ///     This operation updates the programming language properties but does not modify its versions.
    ///     Use the versions endpoint to manage language versions.
    /// </remarks>
    [Authorize(Roles = SubmissionRoles.Judge)]
    [HttpPut("{id:length(24)}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ValidationProblemDetails))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(ApiErrorResponse))]
    [ProducesResponseType(StatusCodes.Status403Forbidden, Type = typeof(ApiErrorResponse))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ApiErrorResponse))]
    public async Task<IActionResult> Put(string id, ProgrammingLanguageRequest programmingLanguageRequest, CancellationToken cancellationToken = default)
    {
        var validationResult = await ValidateAsync(programmingLanguageRequest, _programmingLanguageRequestValidator, cancellationToken);

        if (!validationResult.IsValid)
        {
            return validationResult.GetError();
        }

        var programmingLanguage = await _programmingLanguagesService.GetAsync(id, cancellationToken);

        if (programmingLanguage == null) return NotFound();

        programmingLanguage.FileExtension = programmingLanguageRequest.FileExtension;
        programmingLanguage.Identifier = programmingLanguageRequest.Identifier;
        programmingLanguage.IsEnabled = programmingLanguageRequest.IsEnabled;
        programmingLanguage.Name = programmingLanguageRequest.Name;

        await _programmingLanguagesService.UpdateAsync(programmingLanguage, cancellationToken);

        return NoContent();
    }

    /// <summary>
    ///     Updates an existing version of a programming language.
    /// </summary>
    /// <param name="id">The ID of the programming language</param>
    /// <param name="programmingLanguageVersionRequest">The updated version data</param>
    /// <param name="cancellationToken">The cancellation token</param>
    /// <returns>No content if successful</returns>
    /// <response code="204">The programming language version was successfully updated</response>
    /// <response code="400">The version request is invalid</response>
    /// <response code="401">Authentication has failed</response>
    /// <response code="403">You do not have permission to use this endpoint</response>
    /// <response code="404">The programming language or version does not exist</response>
    /// <remarks>
    ///     The version is identified by its version tag from the request.
    ///     If the updated version is marked as default, all other versions will be set to non-default.
    /// </remarks>
    [Authorize(Roles = SubmissionRoles.Judge)]
    [HttpPut("{id:length(24)}/versions")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ValidationProblemDetails))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(ApiErrorResponse))]
    [ProducesResponseType(StatusCodes.Status403Forbidden, Type = typeof(ApiErrorResponse))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ApiErrorResponse))]
    public async Task<IActionResult> PutProgrammingLanguageVersion(string id, ProgrammingLanguageVersionRequest programmingLanguageVersionRequest,
        CancellationToken cancellationToken = default)
    {
        var programmingLanguage = await _programmingLanguagesService.GetAsync(id, cancellationToken);

        if (programmingLanguage == null) return NotFound();

        var programmingLanguageVersion = programmingLanguage.Versions.FirstOrDefault(v => v.VersionTag == programmingLanguageVersionRequest.VersionTag);

        if (programmingLanguageVersion == null) return NotFound();

        var validationResult = await ValidateAsync(programmingLanguageVersionRequest, _programmingLanguageVersionRequestValidator, cancellationToken);

        if (!validationResult.IsValid)
        {
            return validationResult.GetError();
        }

        if (programmingLanguageVersionRequest.IsDefault)
        {
            foreach (var version in programmingLanguage.Versions)
            {
                version.IsDefault = false;
            }
        }

        programmingLanguageVersion.DisplayName = programmingLanguageVersionRequest.DisplayName;
        programmingLanguageVersion.IsDefault = programmingLanguageVersionRequest.IsDefault;
        programmingLanguageVersion.VersionTag = programmingLanguageVersionRequest.VersionTag;

        await _programmingLanguagesService.UpdateAsync(programmingLanguage, cancellationToken);

        return NoContent();
    }

    /// <summary>
    ///     Converts a <see cref="ProgrammingLanguageRequest" /> to a <see cref="ProgrammingLanguage" /> entity.
    /// </summary>
    /// <param name="programmingLanguageRequest">The request to convert</param>
    /// <returns>A new <see cref="ProgrammingLanguage" /> entity</returns>
    private static ProgrammingLanguage ToEntity(ProgrammingLanguageRequest programmingLanguageRequest)
    {
        return new ProgrammingLanguage
        {
            FileExtension = programmingLanguageRequest.FileExtension,
            Identifier = programmingLanguageRequest.Identifier,
            IsEnabled = programmingLanguageRequest.IsEnabled,
            Name = programmingLanguageRequest.Name,
            Versions = []
        };
    }

    /// <summary>
    ///     Converts a <see cref="ProgrammingLanguageVersionRequest" /> to a <see cref="ProgrammingLanguageVersion" /> entity.
    /// </summary>
    /// <param name="programmingLanguageVersionRequest">The request to convert</param>
    /// <returns>A new <see cref="ProgrammingLanguageVersion" /> entity</returns>
    private static ProgrammingLanguageVersion ToEntity(ProgrammingLanguageVersionRequest programmingLanguageVersionRequest)
    {
        return new ProgrammingLanguageVersion
        {
            DisplayName = programmingLanguageVersionRequest.DisplayName,
            IsDefault = programmingLanguageVersionRequest.IsDefault,
            VersionTag = programmingLanguageVersionRequest.VersionTag
        };
    }
}
