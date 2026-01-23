using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Tsa.Submissions.Coding.Contracts.Problems;
using Tsa.Submissions.Coding.Contracts.TestCases;
using Tsa.Submissions.Coding.WebApi.Authorization;
using Tsa.Submissions.Coding.WebApi.Entities;
using Tsa.Submissions.Coding.WebApi.Services;

namespace Tsa.Submissions.Coding.WebApi.Controllers;

[Route("api/[controller]")]
[ApiController]
[Produces("application/json")]
public class ProblemsController : ControllerBase
{
    private readonly IProblemsService _problemsService;

    public ProblemsController(IProblemsService problemsService)
    {
        _problemsService = problemsService;
    }

    /// <summary>
    ///     Deletes a problem from database
    /// </summary>
    /// <param name="id">The ID of the problem to be removed</param>
    /// <param name="cancellationToken">The .NET cancellation token</param>
    /// <response code="204">Acknowledges the problem was successfully removed</response>
    /// <response code="403">You do not have permission to use this endpoint</response>
    /// <response code="404">The problem to remove does not exist in the database</response>
    [Authorize(Roles = SubmissionRoles.Judge)]
    [HttpDelete("{id:length(24)}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(string id, CancellationToken cancellationToken = default)
    {
        var problem = await _problemsService.GetAsync(id, cancellationToken);

        if (problem == null) return NotFound();

        await _problemsService.RemoveAsync(problem, cancellationToken);

        return NoContent();
    }

    /// <summary>
    ///     Fetches all the problems from the database
    /// </summary>
    /// <param name="expandTestCases">If true, the test sets are returned with the problems, otherwise null is returned</param>
    /// <param name="cancellationToken">The .NET cancellation token</param>
    /// <response code="200">All available problems returned</response>
    [Authorize(Roles = SubmissionRoles.All)]
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<ProblemResponse>))]
    public async Task<ActionResult<IList<ProblemResponse>>> Get(bool expandTestCases = false, CancellationToken cancellationToken = default)
    {
        var problems = await _problemsService.GetAsync(cancellationToken);

        if (problems.Count == 0) return new List<ProblemResponse>(0);

        if (!expandTestCases) return problems.ToResponses().ToList();

        var problemResponses = new List<ProblemResponse>();

        foreach (var problem in problems)
        {
            var testCases = User.IsInRole(SubmissionRoles.Participant)
                ? problem.TestCases.Where(testCase => testCase.IsActive).ToList()
                : problem.TestCases;

            var problemResponse = problem.ToResponse(testCases);
            
            problemResponses.Add(problemResponse);
        }

        return problemResponses;
    }

    /// <summary>
    ///     Fetches a problem from the database
    /// </summary>
    /// <param name="id">The ID of the problem to get</param>
    /// <param name="expandTestCases">If true, the test sets are returned with the problem, otherwise null is returned</param>
    /// <param name="cancellationToken">The .NET cancellation token</param>
    /// <response code="200">Returns the requested problem</response>
    /// <response code="404">The problem does not exist in the database</response>
    [Authorize(Roles = SubmissionRoles.All)]
    [HttpGet("{id:length(24)}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ProblemResponse))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ProblemResponse>> Get(string id, bool expandTestCases = false, CancellationToken cancellationToken = default)
    {
        var problem = await _problemsService.GetAsync(id, cancellationToken);

        if (problem == null) return NotFound();

        if (!expandTestCases) return problem.ToResponse();

        var testCases = User.IsInRole(SubmissionRoles.Participant)
            ? problem.TestCases.Where(testCase => testCase.IsActive).ToList()
            : problem.TestCases;

        return problem.ToResponse(testCases);
    }

    /// <summary>
    ///     Fetches a problem from the database
    /// </summary>
    /// <param name="id">The ID of the problem to get</param>
    /// <param name="cancellationToken">The .NET cancellation token</param>
    /// <response code="200">Returns the requested problem</response>
    /// <response code="404">The problem does not exist in the database</response>
    [Authorize(Roles = SubmissionRoles.All)]
    [HttpGet("{id:length(24)}/test-cases")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<TestCase>))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<IEnumerable<TestCase>>> GetTestSets(string id, CancellationToken cancellationToken = default)
    {
        var problem = await _problemsService.GetAsync(id, cancellationToken);

        if (problem == null) return NotFound();

        var testCases = User.IsInRole(SubmissionRoles.Participant)
            ? problem.TestCases.Where(testCase => testCase.IsActive).ToList()
            : problem.TestCases;

        return testCases;
    }

    /// <summary>
    ///     Creates a new problem
    /// </summary>
    /// <param name="problemRequest">The problem to be created</param>
    /// <param name="cancellationToken">The .NET cancellation token</param>
    /// <response code="201">Returns the requested problem</response>
    /// <response code="400">The problem is not in a valid state and cannot be created</response>
    /// <response code="403">You do not have permission to use this endpoint</response>
    [Authorize(Roles = SubmissionRoles.Judge)]
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ValidationProblemDetails))]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<CreatedAtActionResult> Post(ProblemRequest problemRequest, CancellationToken cancellationToken = default)
    {
        var problem = ToEntity(problemRequest);

        await _problemsService.CreateAsync(problem, cancellationToken);

        var problemResponse = problem.ToResponse();

        return CreatedAtAction(nameof(Get), new { id = problem.Id }, problemResponse);
    }

    /// <summary>
    ///     Updates the specified problem
    /// </summary>
    /// <param name="id">The ID of the problem to update</param>
    /// <param name="updatedProblemRequest">The problem that should replace the one in the database</param>
    /// <param name="cancellationToken">The .NET cancellation token</param>
    /// <response code="204">Acknowledgement that the problem was updated</response>
    /// <response code="400">The problem is not in a valid state and cannot be updated</response>
    /// <response code="404">The problem requested to be updated could not be found</response>
    [Authorize(Roles = SubmissionRoles.Judge)]
    [HttpPut("{id:length(24)}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ValidationProblemDetails))]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Put(string id, ProblemRequest updatedProblemRequest, CancellationToken cancellationToken = default)
    {
        var problem = await _problemsService.GetAsync(id, cancellationToken);

        if (problem == null) return NotFound();

        await _problemsService.UpdateAsync(ToEntity(updatedProblemRequest, id), cancellationToken);

        return NoContent();
    }

    private static Problem ToEntity(ProblemRequest problemRequest, string? id = null)
    {
        return new Problem
        {
            Id = id,
            Title = problemRequest.Title,
            Description = problemRequest.Description,
            IsActive = problemRequest.IsActive
        };
    }
}
