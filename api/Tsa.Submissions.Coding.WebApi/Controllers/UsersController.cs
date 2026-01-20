using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Tsa.Submissions.Coding.Contracts.Users;
using Tsa.Submissions.Coding.WebApi.Authorization;
using Tsa.Submissions.Coding.WebApi.Entities;
using Tsa.Submissions.Coding.WebApi.Models;
using Tsa.Submissions.Coding.WebApi.Services;

namespace Tsa.Submissions.Coding.WebApi.Controllers;

[Route("api/[controller]")]
[ApiController]
[Produces("application/json")]
public class UsersController : ControllerBase
{
    private const string UserIdCacheKey = "user_id";
    private const string UsersCacheKey = "users";
    private readonly TimeSpan _cacheExpiration = TimeSpan.FromHours(2);

    private readonly ICacheService _cacheService;
    private readonly IUsersService _usersService;

    public UsersController(ICacheService cacheService, IUsersService usersService)
    {
        _cacheService = cacheService;
        _usersService = usersService;
    }

    private NotFoundObjectResult CreateUserNotFoundError(string id)
    {
        return NotFound(ApiErrorResponseModel.EntityNotFound(nameof(Entities.User), id));
    }

    /// <summary>
    ///     Deletes a user in the database
    /// </summary>
    /// <param name="id">The ID of the user to delete</param>
    /// <param name="cancellationToken">The .NET cancellation token</param>
    /// <response code="204">Acknowledges the user was successfully removed</response>
    /// <response code="401">Authentication has failed</response>
    /// <response code="403">You do not have permission to use this endpoint</response>
    /// <response code="404">The user to remove does not exist in the database</response>
    [Authorize(Roles = SubmissionRoles.Judge)]
    [HttpDelete("{id:length(24)}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(ApiErrorResponseModel))]
    [ProducesResponseType(StatusCodes.Status403Forbidden, Type = typeof(ApiErrorResponseModel))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ApiErrorResponseModel))]
    public async Task<IActionResult> Delete(string id, CancellationToken cancellationToken = default)
    {
        var user = await _usersService.GetAsync(id, cancellationToken);

        if (user == null) return CreateUserNotFoundError(id);

        await _usersService.RemoveAsync(user, cancellationToken);

        await _cacheService.RemoveAsync($"{UserIdCacheKey}:{id}", cancellationToken);
        await _cacheService.RemoveAsync(UsersCacheKey, cancellationToken);

        return NoContent();
    }

    private static string? ExtractErrorMessageFromActionResult(IActionResult actionResult)
    {
        return actionResult switch
        {
            BadRequestObjectResult badRequest => ((ValidationProblemDetails)badRequest.Value!).Detail,
            ConflictObjectResult conflict => ((ApiErrorResponseModel)conflict.Value!).Message,
            _ => actionResult.ToString()
        };
    }

    /// <summary>
    ///     Fetches all the users from the database
    /// </summary>
    /// <param name="cancellationToken">The .NET cancellation token</param>
    /// <response code="200">All available users returned</response>
    /// <response code="401">Authentication has failed</response>
    /// <response code="403">You do not have permission to use this endpoint</response>
    [Authorize(Roles = SubmissionRoles.Judge)]
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<UserResponse>))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(ApiErrorResponseModel))]
    [ProducesResponseType(StatusCodes.Status403Forbidden, Type = typeof(ApiErrorResponseModel))]
    public async Task<IActionResult> Get(CancellationToken cancellationToken = default)
    {
        var users = await GetUsersFromCache(cancellationToken);

        return Ok(users.ToResponses());
    }

    /// <summary>
    ///     Fetches a problem from the database
    /// </summary>
    /// <param name="id">The ID of the user to get</param>
    /// <param name="cancellationToken">The .NET cancellation token</param>
    /// <response code="200">Returns the requested user</response>
    /// <response code="401">Authentication has failed</response>
    /// <response code="403">You do not have permission to use this endpoint</response>
    /// <response code="404">The user does not exist in your context</response>
    [Authorize(Roles = SubmissionRoles.All)]
    [HttpGet("{id:length(24)}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(UserResponse))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(ApiErrorResponseModel))]
    [ProducesResponseType(StatusCodes.Status403Forbidden, Type = typeof(ApiErrorResponseModel))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ApiErrorResponseModel))]
    public async Task<IActionResult> Get(string id, CancellationToken cancellationToken = default)
    {
        var user = await GetUserFromCache(id, cancellationToken);

        if (user == null) return CreateUserNotFoundError(id);

        if (User.IsInRole(SubmissionRoles.Participant) && User.Identity!.Name != user.UserName)
        {
            return CreateUserNotFoundError(id);
        }

        return Ok(user.ToResponse());
    }

    private async Task<T?> GetOrSetCacheAsync<T>(string cacheKey, Func<CancellationToken, Task<T?>> fetchFromService, CancellationToken cancellationToken)
    {
        var cachedData = await _cacheService.GetAsync<T>(cacheKey, cancellationToken);

        if (cachedData != null) return cachedData;

        var data = await fetchFromService(cancellationToken);

        if (data != null)
        {
            await _cacheService.SetAsync(cacheKey, data, _cacheExpiration, cancellationToken);
        }

        return data;
    }

    private async Task<User?> GetUserFromCache(string id, CancellationToken cancellationToken)
    {
        return await GetOrSetCacheAsync(
            $"{UserIdCacheKey}:{id}",
            fetchFromService: async ct => await _usersService.GetAsync(id, ct),
            cancellationToken
        );
    }

    private async Task<List<User>> GetUsersFromCache(CancellationToken cancellationToken)
    {
        return await GetOrSetCacheAsync(
            UsersCacheKey,
            fetchFromService: async ct => await _usersService.GetAsync(ct),
            cancellationToken
        ) ?? [];
    }

    /// <summary>
    ///     Creates a new user
    /// </summary>
    /// <param name="userCreateRequest">The user to be created</param>
    /// <param name="cancellationToken">The .NET cancellation token</param>
    /// <response code="201">Returns the created user</response>
    /// <response code="400">The user to create is not in a valid state and cannot be created</response>
    /// <response code="401">Authentication has failed</response>
    /// <response code="403">You do not have permission to use this endpoint</response>
    /// <response code="404">The team specified for the user could not be found</response>
    [Authorize(Roles = SubmissionRoles.Judge)]
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ValidationProblemDetails))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(ApiErrorResponseModel))]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Post(UserCreateRequest userCreateRequest, CancellationToken cancellationToken = default)
    {
        var existingUser = await _usersService.GetByUserNameAsync(userCreateRequest.UserName, cancellationToken);

        if (existingUser != null) return Conflict(ApiErrorResponseModel.EntityAlreadyExists(nameof(User), userCreateRequest.UserName));

        var user = ToEntity(userCreateRequest);

        await _usersService.CreateAsync(user, cancellationToken);

        await SetUserCache(user, cancellationToken);

        return CreatedAtAction(nameof(Get), new { id = user.Id }, user.ToResponse());
    }

    /// <summary>
    ///     Creates multiple users in a batch operation
    /// </summary>
    /// <param name="userCreateRequests">An array of users to be created</param>
    /// <param name="cancellationToken">The .NET cancellation token</param>
    /// <response code="201">Returns the created users</response>
    /// <response code="400">The user to create is not in a valid state and cannot be created</response>
    /// <response code="401">Authentication has failed</response>
    /// <response code="403">You do not have permission to use this endpoint</response>
    [Authorize(Roles = SubmissionRoles.Judge)]
    [HttpPost("batch")]
    [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(IList<UserResponse>))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ValidationProblemDetails))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(ApiErrorResponseModel))]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> Post(UserCreateRequest[] userCreateRequests, CancellationToken cancellationToken = default)
    {
        if (userCreateRequests.Length == 0)
        {
            return BadRequest(new ValidationProblemDetails
            {
                Title = "No users to create",
                Detail = "At least one user must be specified to use the batch endpoint."
            });
        }

        var batchOperationModel = new BatchOperationModel<UserCreateRequest, UserResponse>();

        foreach (var userCreateRequest in userCreateRequests)
        {
            var actionResult = await Post(userCreateRequest, cancellationToken);

            if (actionResult is not CreatedAtActionResult createdAtActionResult)
            {
                batchOperationModel.FailedItems.Add(new ItemFailureModel<UserCreateRequest>
                {
                    ErrorMessage = ExtractErrorMessageFromActionResult(actionResult),
                    Item = userCreateRequest
                });
            }
            else
            {
                batchOperationModel.CreatedItems.Add((UserResponse)createdAtActionResult.Value!);
            }
        }

        if (batchOperationModel.FailedItems.Count > 0)
        {
            batchOperationModel.Result = batchOperationModel.CreatedItems.Count > 0
                ? BatchOperationResult.PartialSuccess.ToString()
                : BatchOperationResult.Failed.ToString();
        }
        else
        {
            batchOperationModel.Result = BatchOperationResult.Success.ToString();
        }

        return batchOperationModel.Result != BatchOperationResult.Failed.ToString()
            ? Created("batch", batchOperationModel)
            : BadRequest(batchOperationModel);
    }

    /// <summary>
    ///     Updates the specified user
    /// </summary>
    /// <param name="id">The ID of the user to update</param>
    /// <param name="updatedUserModel">The user that should replace the one in the database</param>
    /// <param name="cancellationToken">The .NET cancellation token</param>
    /// <response code="204">Acknowledgement that the user was updated</response>
    /// <response code="400">The user to replace in the database with is not in a valid state and cannot be replaced</response>
    /// <response code="401">Authentication has failed</response>
    /// <response code="403">You do not have permission to use this endpoint</response>
    [Authorize(Roles = SubmissionRoles.Judge)]
    [HttpPut("{id:length(24)}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ValidationProblemDetails))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(ApiErrorResponseModel))]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> Put(string id, UserModifyRequest updatedUserModel, CancellationToken cancellationToken = default)
    {
        // Team is required and is enforced in the model validation
        var user = await _usersService.GetAsync(id, cancellationToken);

        if (user == null) return CreateUserNotFoundError(id);

        var updatedUser = ToEntity(updatedUserModel);
        updatedUser.Id = user.Id;

        if (string.IsNullOrWhiteSpace(updatedUser.PasswordHash))
        {
            updatedUser.PasswordHash = user.PasswordHash;
        }

        await _usersService.UpdateAsync(updatedUser, cancellationToken);

        await SetUserCache(updatedUser, cancellationToken);

        return NoContent();
    }

    private async Task SetUserCache(User user, CancellationToken cancellationToken)
    {
        if (user.Id == null) throw new InvalidOperationException("The user's ID cannot be null when adding it to the cache.");

        await _cacheService.SetAsync($"{UserIdCacheKey}:{user.Id}", user, _cacheExpiration, cancellationToken);
        await _cacheService.RemoveAsync(UsersCacheKey, cancellationToken);
    }

    private static Team ToEntity(TeamRequest teamRequest)
    {
        return new Team(
            Enum.Parse<CompetitionLevel>(teamRequest.CompetitionLevel),
            teamRequest.SchoolNumber,
            teamRequest.TeamNumber);
    }

    private static User ToEntity(IUserRequest userRequest)
    {
        var user = new User
        {
            Role = userRequest.Role,
            Team = userRequest.Team == null ? null : ToEntity(userRequest.Team),
            UserName = userRequest.UserName
        };

        if (userRequest.Password != null)
        {
            user.PasswordHash = BC.HashPassword(userRequest.Password);
        }
        else if (userRequest is UserCreateRequest)
        {
            throw new InvalidOperationException("Password is required for user creation.");
        }

        return user;
    }
}
