using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using Tsa.Submissions.Coding.Contracts.TestSets;
using Tsa.Submissions.Coding.WebApi.Authorization;
using Tsa.Submissions.Coding.WebApi.Entities;
using Tsa.Submissions.Coding.WebApi.Exceptions;
using Tsa.Submissions.Coding.WebApi.Services;

namespace Tsa.Submissions.Coding.WebApi.Controllers;

[Route("api/[controller]")]
[ApiController]
[Produces("application/json")]
public class TestSetsController : ControllerBase
{
    private readonly IProblemsService _problemsService;
    private readonly ITestSetsService _testSetsService;

    public TestSetsController(IProblemsService problemsService, ITestSetsService testSetsService)
    {
        _problemsService = problemsService;
        _testSetsService = testSetsService;
    }

    /// <summary>
    ///     Deletes a testSet from database
    /// </summary>
    /// <param name="id">The ID of the testSet to be removed</param>
    /// <param name="cancellationToken">The .NET cancellation token</param>
    /// <response code="204">Acknowledges the testSet was successfully removed</response>
    /// <response code="403">You do not have permission to use this endpoint</response>
    /// <response code="404">The testSet to remove does not exist in the database</response>
    [Authorize(Roles = SubmissionRoles.Judge)]
    [HttpDelete("{id:length(24)}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(string id, CancellationToken cancellationToken = default)
    {
        var testSet = await _testSetsService.GetAsync(id, cancellationToken);

        if (testSet == null) return NotFound();

        await _testSetsService.RemoveAsync(testSet, cancellationToken);

        return NoContent();
    }

    private async Task EnsureProblemExists(string problemId, string attributeName)
    {
        if (!await _problemsService.ExistsAsync(problemId))
        {
            throw new RequiredEntityNotFoundException(attributeName);
        }
    }

    /// <summary>
    ///     Fetches all the testSets from the database
    /// </summary>
    /// <param name="cancellationToken">The .NET cancellation token</param>
    /// <response code="200">All available testSets returned</response>
    [Authorize(Roles = SubmissionRoles.All)]
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<TestSetResponse>))]
    public async Task<ActionResult<IList<TestSetResponse>>> Get(CancellationToken cancellationToken = default)
    {
        var testSets = await _testSetsService.GetAsync(cancellationToken);

        return User.IsInRole(SubmissionRoles.Participant)
            ? testSets.Where(testSetModel => testSetModel.IsPublic).ToResponses().ToList()
            : testSets.ToResponses().ToList();
    }

    /// <summary>
    ///     Fetches a testSet from the database
    /// </summary>
    /// <param name="id">The ID of the testSet to get</param>
    /// <param name="cancellationToken">The .NET cancellation token</param>
    /// <response code="200">Returns the requested testSet</response>
    /// <response code="404">The testSet does not exist in the database</response>
    [Authorize(Roles = SubmissionRoles.All)]
    [HttpGet("{id:length(24)}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(TestSetResponse))]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<TestSetResponse>> Get(string id, CancellationToken cancellationToken = default)
    {
        var testSet = await _testSetsService.GetAsync(id, cancellationToken);

        if (testSet == null) return NotFound();

        if (User.IsInRole(SubmissionRoles.Participant) && !testSet.IsPublic)
        {
            // Return 404 to obfuscate the data
            return NotFound();
        }

        return testSet.ToResponse();
    }

    /// <summary>
    ///     Creates a new testSet
    /// </summary>
    /// <param name="testSetRequest">The testSet to be created</param>
    /// <param name="cancellationToken">The .NET cancellation token</param>
    /// <response code="201">Returns the requested testSet</response>
    /// <response code="400">The testSet is not in a valid state and cannot be created</response>
    /// <response code="403">You do not have permission to use this endpoint</response>
    [Authorize(Roles = SubmissionRoles.Judge)]
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ValidationProblemDetails))]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<CreatedAtActionResult> Post(TestSetRequest testSetRequest, CancellationToken cancellationToken = default)
    {
        // The ProblemModelValidator will ensure ProblemId is not null
        await EnsureProblemExists(testSetRequest.ProblemId, nameof(TestSetResponse.ProblemId));

        var testSet = ToEntity(testSetRequest);

        await _testSetsService.CreateAsync(testSet, cancellationToken);

        return CreatedAtAction(nameof(Get), new { id = testSet.Id }, testSet.ToResponse());
    }

    /// <summary>
    ///     Updates the specified testSet
    /// </summary>
    /// <param name="id">The ID of the testSet to update</param>
    /// <param name="updatedTestSetModel">The testSet that should replace the one in the database</param>
    /// <param name="cancellationToken">The .NET cancellation token</param>
    /// <response code="204">Acknowledgement that the testSet was updated</response>
    /// <response code="400">The testSet is not in a valid state and cannot be updated</response>
    /// <response code="404">The testSet requested to be updated could not be found</response>
    [Authorize(Roles = SubmissionRoles.Judge)]
    [HttpPut("{id:length(24)}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ValidationProblemDetails))]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Put(string id, TestSetRequest updatedTestSetModel, CancellationToken cancellationToken = default)
    {
        // The ProblemModelValidator will ensure ProblemId is not null
        await EnsureProblemExists(updatedTestSetModel.ProblemId, nameof(TestSetRequest.ProblemId));

        var testSet = await _testSetsService.GetAsync(id, cancellationToken);

        if (testSet == null) return NotFound();

        await _testSetsService.UpdateAsync(ToEntity(updatedTestSetModel, id), cancellationToken);

        return NoContent();
    }

    public static TestSet ToEntity(TestSetRequest testSetRequest, string? id = null)
    {
        var testSet = new TestSet
        {
            Id = id,
            IsPublic = testSetRequest.IsPublic,
            Name = testSetRequest.Name,
            Problem = new MongoDBRef(ProblemsService.MongoDbCollectionName, testSetRequest.ProblemId)
        };

        if (testSetRequest.Inputs.Count == 0) return testSet;

        testSet.Inputs = new List<TestSetValue>();

        foreach (var input in testSetRequest.Inputs)
        {
            testSet.Inputs.Add(new TestSetValue
            {
                DataType = input.DataType,
                Index = input.Index,
                IsArray = input.IsArray,
                ValueAsJson = input.ValueAsJson
            });
        }

        return testSet;
    }
}
