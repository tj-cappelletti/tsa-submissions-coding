using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Tsa.Submissions.Coding.WebApi.Authorization;
using Tsa.Submissions.Coding.WebApi.Entities;
using Tsa.Submissions.Coding.WebApi.Models;
using Tsa.Submissions.Coding.WebApi.Services;

namespace Tsa.Submissions.Coding.WebApi.Controllers;

[Route("api/[controller]")]
[ApiController]
[Produces("application/json")]
public class ProblemsController : ControllerBase
{
    private readonly IProblemsService _problemsService;
    private readonly ITestSetsService _testSetsService;

    public ProblemsController(IProblemsService problemsService, ITestSetsService testSetsService)
    {
        _problemsService = problemsService;
        _testSetsService = testSetsService;
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
    /// <param name="expandTestSets">If true, the test sets are returned with the problems, otherwise null is returned</param>
    /// <param name="cancellationToken">The .NET cancellation token</param>
    /// <response code="200">All available problems returned</response>
    [Authorize(Roles = SubmissionRoles.All)]
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<ProblemModel>))]
    public async Task<ActionResult<IList<ProblemModel>>> Get(bool expandTestSets = false, CancellationToken cancellationToken = default)
    {
        var problems = await _problemsService.GetAsync(cancellationToken);

        var problemModels = new List<ProblemModel>();

        if (expandTestSets && problems.Count != 0)
        {
            foreach (var problem in problems)
            {
                var problemModel = problem.ToModel();

                var testSets = await _testSetsService.GetAsync(problem, cancellationToken);

                problemModel.TestSets = User.IsInRole(SubmissionRoles.Participant)
                    ? testSets.Where(testSet => testSet.IsPublic).ToModels()
                    : testSets.ToModels();

                problemModels.Add(problemModel);
            }
        }
        else
        {
            problemModels.AddRange(problems.ToModels());
        }

        return problemModels;
    }

    /// <summary>
    ///     Fetches a problem from the database
    /// </summary>
    /// <param name="id">The ID of the problem to get</param>
    /// <param name="expandTestSets">If true, the test sets are returned with the problem, otherwise null is returned</param>
    /// <param name="cancellationToken">The .NET cancellation token</param>
    /// <response code="200">Returns the requested problem</response>
    /// <response code="404">The problem does not exist in the database</response>
    [Authorize(Roles = SubmissionRoles.All)]
    [HttpGet("{id:length(24)}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ProblemModel))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ProblemModel>> Get(string id, bool expandTestSets = false, CancellationToken cancellationToken = default)
    {
        var problem = await _problemsService.GetAsync(id, cancellationToken);

        if (problem == null) return NotFound();

        if (!expandTestSets) return problem.ToModel();

        var problemModel = problem.ToModel();

        var testSets = await _testSetsService.GetAsync(problem, cancellationToken);

        problemModel.TestSets = User.IsInRole(SubmissionRoles.Participant)
            ? testSets.ToModels().Where(testSetModel => testSetModel.IsPublic).ToList()
            : testSets.ToModels();

        return problemModel;
    }

    /// <summary>
    ///     Fetches a problem from the database
    /// </summary>
    /// <param name="id">The ID of the problem to get</param>
    /// <param name="cancellationToken">The .NET cancellation token</param>
    /// <response code="200">Returns the requested problem</response>
    /// <response code="404">The problem does not exist in the database</response>
    [Authorize(Roles = SubmissionRoles.All)]
    [HttpGet("{id:length(24)}/testsets")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<TestSetModel>))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<IList<TestSetModel>>> GetTestSets(string id, CancellationToken cancellationToken = default)
    {
        var problem = await _problemsService.GetAsync(id, cancellationToken);

        if (problem == null) return NotFound();

        var testSets = await _testSetsService.GetAsync(problem, cancellationToken);

        return User.IsInRole(SubmissionRoles.Participant)
            ? testSets.ToModels().Where(testSetModel => testSetModel.IsPublic).ToList()
            : testSets.ToModels();
    }

    /// <summary>
    ///     Creates a new problem
    /// </summary>
    /// <param name="problemModel">The problem to be created</param>
    /// <param name="cancellationToken">The .NET cancellation token</param>
    /// <response code="201">Returns the requested problem</response>
    /// <response code="400">The problem is not in a valid state and cannot be created</response>
    /// <response code="403">You do not have permission to use this endpoint</response>
    [Authorize(Roles = SubmissionRoles.Judge)]
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ValidationProblemDetails))]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<CreatedAtActionResult> Post(ProblemModel problemModel, CancellationToken cancellationToken = default)
    {
        var problem = problemModel.ToEntity();

        await _problemsService.CreateAsync(problem, cancellationToken);

        problemModel.Id = problem.Id;

        return CreatedAtAction(nameof(Get), new { id = problem.Id }, problemModel);
    }

    /// <summary>
    ///     Updates the specified problem
    /// </summary>
    /// <param name="id">The ID of the problem to update</param>
    /// <param name="updatedProblemModel">The problem that should replace the one in the database</param>
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
    public async Task<IActionResult> Put(string id, ProblemModel updatedProblemModel, CancellationToken cancellationToken = default)
    {
        var problem = await _problemsService.GetAsync(id, cancellationToken);

        if (problem == null) return NotFound();

        updatedProblemModel.Id = problem.Id;

        await _problemsService.UpdateAsync(updatedProblemModel.ToEntity(), cancellationToken);

        return NoContent();
    }
}
