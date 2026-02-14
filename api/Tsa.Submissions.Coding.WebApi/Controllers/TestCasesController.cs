using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Tsa.Submissions.Coding.Contracts;
using Tsa.Submissions.Coding.Contracts.TestCases;
using Tsa.Submissions.Coding.WebApi.Authorization;
using Tsa.Submissions.Coding.WebApi.Entities;
using Tsa.Submissions.Coding.WebApi.Services;

namespace Tsa.Submissions.Coding.WebApi.Controllers;

/// <summary>
///     Manages test cases for coding problems.
///     Test cases are used to validate participant submissions.
/// </summary>
[Route("api/problems/{problemId:length(24)}/test-cases")]
[ApiController]
[Produces("application/json")]
public class TestCasesController : WebApiBaseController
{
    private readonly IProblemsService _problemsService;
    private readonly ITestCasesService _testCasesService;

    public TestCasesController(IProblemsService problemsService, ITestCasesService testCasesService)
    {
        _problemsService = problemsService;
        _testCasesService = testCasesService;
    }

    private static string ComputeSignature(TestCaseRequest testCase)
    {
        var orderedTestCaseInputs = testCase.Inputs
            .OrderBy(testCaseInput => testCaseInput.Index)
            .Select(i => new Dictionary<string, object?>
            {
                ["index"] = i.Index,
                ["dataType"] = i.DataType.ToUpperInvariant(),
                ["isArray"] = i.IsArray,
                ["value"] = i.Value
            })
            .ToList();

        var hashPayload = new Dictionary<string, object?>
        {
            ["inputs"] = orderedTestCaseInputs,
            ["expectedOutput"] = testCase.ExpectedOutput,
            ["outputDataType"] = testCase.OutputDataType.ToUpperInvariant(),
            ["outputIsArray"] = testCase.OutputIsArray
        };

        var jsonSerializerOptions = new JsonSerializerOptions
        {
            Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
            PropertyNameCaseInsensitive = false,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = false
        };

        var serializedHashPayload = JsonSerializer.Serialize(hashPayload, jsonSerializerOptions);

        var hash = SHA256.HashData(Encoding.UTF8.GetBytes(serializedHashPayload));

        return "sha256:" + Convert.ToHexString(hash).ToLowerInvariant();
    }

    /// <summary>
    ///     Deletes a test case from a problem.
    /// </summary>
    /// <param name="problemId">The ID of the problem</param>
    /// <param name="id">The ID of the test case to delete</param>
    /// <param name="cancellationToken">The cancellation token</param>
    /// <response code="204">Acknowledgement that the test case was deleted</response>
    /// <response code="401">Authentication has failed</response>
    /// <response code="403">You do not have permission to use this endpoint</response>
    /// <response code="404">The problem or test case does not exist</response>
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

        var testCase = await _testCasesService.GetAsync(id, cancellationToken);

        if (testCase == null) return NotFound(ApiErrorEntityNotFound(nameof(TestCase), id));

        // Verify the test case belongs to this problem
        if (testCase.ProblemId != problemId)
        {
            return NotFound(ApiErrorEntityNotFound(nameof(TestCase), id));
        }

        await _testCasesService.RemoveAsync(testCase, cancellationToken);

        return NoContent();
    }

    /// <summary>
    ///     Gets all test cases for a specific problem.
    ///     Participants only see public test cases; judges see all test cases.
    /// </summary>
    /// <param name="problemId">The ID of the problem</param>
    /// <param name="cancellationToken">The cancellation token</param>
    /// <response code="200">Returns the test cases for the problem</response>
    /// <response code="401">Authentication has failed</response>
    /// <response code="404">The problem does not exist</response>
    [Authorize(Roles = SubmissionRoles.All)]
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<TestCaseResponse>))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(ApiErrorResponse))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ApiErrorResponse))]
    public async Task<ActionResult<IEnumerable<TestCaseResponse>>> Get(string problemId, CancellationToken cancellationToken = default)
    {
        var problem = await _problemsService.GetAsync(problemId, cancellationToken);

        if (problem == null) return NotFound(ApiErrorEntityNotFound(nameof(Problem), problemId));

        var testCases = await _testCasesService.GetByProblemAsync(problem, cancellationToken);

        var filteredTestCases = User.IsInRole(SubmissionRoles.Participant)
            ? testCases.Where(testCase => testCase.IsPublic)
            : testCases;

        return Ok(filteredTestCases.ToResponses().ToList());
    }

    /// <summary>
    ///     Gets a specific test case for a problem.
    ///     Participants can only access public test cases; judges can access all test cases.
    /// </summary>
    /// <param name="problemId">The ID of the problem</param>
    /// <param name="id">The ID of the test case</param>
    /// <param name="cancellationToken">The cancellation token</param>
    /// <response code="200">Returns the requested test case</response>
    /// <response code="401">Authentication has failed</response>
    /// <response code="403">You do not have permission to access this test case</response>
    /// <response code="404">The problem or test case does not exist</response>
    [Authorize(Roles = SubmissionRoles.All)]
    [HttpGet("{id:length(24)}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(TestCaseResponse))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(ApiErrorResponse))]
    [ProducesResponseType(StatusCodes.Status403Forbidden, Type = typeof(ApiErrorResponse))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ApiErrorResponse))]
    public async Task<ActionResult<TestCaseResponse>> Get(string problemId, string id, CancellationToken cancellationToken = default)
    {
        var problem = await _problemsService.GetAsync(problemId, cancellationToken);

        if (problem == null) return NotFound(ApiErrorEntityNotFound(nameof(Problem), problemId));

        var testCase = await _testCasesService.GetAsync(id, cancellationToken);

        if (testCase == null) return NotFound(ApiErrorEntityNotFound(nameof(TestCase), id));

        // Verify the test case belongs to this problem
        if (testCase.ProblemId != problemId)
        {
            return NotFound(ApiErrorEntityNotFound(nameof(TestCase), id));
        }

        // Participants can only see public test cases
        if (User.IsInRole(SubmissionRoles.Participant) && !testCase.IsPublic)
        {
            // Return 404 to avoid revealing the existence of the test case
            return NotFound(ApiErrorEntityNotFound(nameof(TestCase), id));
        }

        return Ok(testCase.ToResponse());
    }

    /// <summary>
    ///     Creates a new test case for a problem.
    /// </summary>
    /// <param name="problemId">The ID of the problem</param>
    /// <param name="testCaseRequest">The test case to create</param>
    /// <param name="cancellationToken">The cancellation token</param>
    /// <response code="201">Returns the created test case</response>
    /// <response code="400">The test case is not in a valid state</response>
    /// <response code="401">Authentication has failed</response>
    /// <response code="403">You do not have permission to use this endpoint</response>
    /// <response code="404">The problem does not exist</response>
    /// <response code="409">A test case with the same signature already exists</response>
    [Authorize(Roles = SubmissionRoles.Judge)]
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(TestCaseResponse))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ValidationProblemDetails))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(ApiErrorResponse))]
    [ProducesResponseType(StatusCodes.Status403Forbidden, Type = typeof(ApiErrorResponse))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ApiErrorResponse))]
    [ProducesResponseType(StatusCodes.Status409Conflict, Type = typeof(ApiErrorResponse))]
    public async Task<ActionResult<TestCaseResponse>> Post(string problemId, TestCaseRequest testCaseRequest,
        CancellationToken cancellationToken = default)
    {
        var problem = await _problemsService.GetAsync(problemId, cancellationToken);

        if (problem == null) return NotFound(ApiErrorEntityNotFound(nameof(Problem), problemId));

        var testCase = ToEntity(testCaseRequest, problemId);

        // Null forgiveness operator is used here because the signature is computed from the test case properties and should never be null.
        // If it is null, that indicates an in issue in the ToEntity and ComputeSignature methods.
        if (await _testCasesService.SignatureExistsAsync(problem, testCase.Signature!, cancellationToken))
        {
            return Conflict(ApiErrorEntityAlreadyExists(nameof(TestCase), testCase.Signature!));
        }

        await _testCasesService.CreateAsync(testCase, cancellationToken);

        return CreatedAtAction(nameof(Get), new { problemId, id = testCase.Id }, testCase.ToResponse());
    }

    /// <summary>
    ///     Updates a test case for a problem.
    /// </summary>
    /// <param name="problemId">The ID of the problem</param>
    /// <param name="id">The ID of the test case to update</param>
    /// <param name="testCaseRequest">The updated test case data</param>
    /// <param name="cancellationToken">The cancellation token</param>
    /// <response code="204">Acknowledgement that the test case was updated</response>
    /// <response code="400">The test case is not in a valid state</response>
    /// <response code="401">Authentication has failed</response>
    /// <response code="403">You do not have permission to use this endpoint</response>
    /// <response code="404">The problem or test case does not exist</response>
    /// <response code="409">A test case with the same signature already exists</response>
    [Authorize(Roles = SubmissionRoles.Judge)]
    [HttpPut("{id:length(24)}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ValidationProblemDetails))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(ApiErrorResponse))]
    [ProducesResponseType(StatusCodes.Status403Forbidden, Type = typeof(ApiErrorResponse))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ApiErrorResponse))]
    [ProducesResponseType(StatusCodes.Status409Conflict, Type = typeof(ApiErrorResponse))]
    public async Task<IActionResult> Put(string problemId, string id, TestCaseRequest testCaseRequest, CancellationToken cancellationToken = default)
    {
        var problem = await _problemsService.GetAsync(problemId, cancellationToken);

        if (problem == null) return NotFound(ApiErrorEntityNotFound(nameof(Problem), problemId));

        var testCase = await _testCasesService.GetAsync(id, cancellationToken);

        if (testCase == null) return NotFound(ApiErrorEntityNotFound(nameof(TestCase), id));

        // Verify the test case belongs to this problem
        if (testCase.ProblemId != problemId)
        {
            return NotFound(ApiErrorEntityNotFound(nameof(TestCase), id));
        }

        var updatedTestCase = ToEntity(testCaseRequest, problemId, id);

        var existingTestCaseWithSignature = await _testCasesService.GetBySignatureAsync(problem, updatedTestCase.Signature!, cancellationToken);

        // Check if the update will result in a duplicate signature that belongs to a different test case for a given problem
        // Test cases must be unique (via their signature) for a given problem
        if (existingTestCaseWithSignature?.Id != id)
        {
            return Conflict(ApiErrorEntityAlreadyExists(nameof(TestCase), updatedTestCase.Signature!));
        }

        await _testCasesService.UpdateAsync(updatedTestCase, cancellationToken);

        return NoContent();
    }

    private static TestCase ToEntity(TestCaseRequest testCaseRequest, string problemId, string? id = null)
    {
        // This should be caught by model validation,
        // but we check again here to avoid potential issues in the ComputeSignature method.
        ArgumentNullException.ThrowIfNull(testCaseRequest);
        ArgumentException.ThrowIfNullOrWhiteSpace(problemId);

        return new TestCase
        {
            Id = id,
            ProblemId = problemId,
            Name = testCaseRequest.Name,
            ExpectedOutput = testCaseRequest.ExpectedOutput,
            Inputs = testCaseRequest.Inputs.Select(ToEntity).ToList(),
            IsActive = testCaseRequest.IsActive,
            IsPublic = testCaseRequest.IsPublic,
            OutputDataType = testCaseRequest.OutputDataType,
            OutputIsArray = testCaseRequest.OutputIsArray,
            Signature = ComputeSignature(testCaseRequest)
        };
    }

    private static TestCaseInput ToEntity(TestCaseInputRequest testCaseInputRequest)
    {
        return new TestCaseInput
        {
            Index = testCaseInputRequest.Index,
            DataType = testCaseInputRequest.DataType,
            IsArray = testCaseInputRequest.IsArray,
            Value = testCaseInputRequest.Value
        };
    }
}
