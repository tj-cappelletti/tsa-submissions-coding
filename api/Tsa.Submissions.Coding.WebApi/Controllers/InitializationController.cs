using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Tsa.Submissions.Coding.Contracts.Users;
using Tsa.Submissions.Coding.WebApi.Models;
using Tsa.Submissions.Coding.WebApi.Services;

namespace Tsa.Submissions.Coding.WebApi.Controllers;

[Route("api/[controller]")]
[ApiController]
[Produces("application/json")]
public class InitializationController : ControllerBase
{
    private readonly ILogger<InitializationController> _logger;
    private readonly IUsersService _usersService;

    public InitializationController(ILogger<InitializationController> logger, IUsersService usersService)
    {
        _logger = logger;
        _usersService = usersService;
    }

    [HttpGet("status")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(InitializationStatusModel))]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetInitializationStatus(CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Checking initialization status");
        var isInitialized = await IsInitialized(cancellationToken);

        _logger.LogInformation("Is Initialized: {isInitialized}", isInitialized);

        var statusModel = new InitializationStatusModel
        {
            IsInitialized = isInitialized
        };

        return Ok(statusModel);
    }

    [HttpPost("initialize")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(UserCreateRequest))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public Task<IActionResult> Initialize([FromBody]UserCreateRequest userCreateRequest, CancellationToken cancellationToken = default)
    {
        //var isInitialized = await IsInitialized(cancellationToken);

        //if (isInitialized)
        //{
        //    _logger.LogWarning("Initialization failed: The application is already initialized");

        //    return StatusCode(StatusCodes.Status400BadRequest, "The application is already initialized");
        //}

        //var passwordHash = BC.HashPassword(userModel.Password);

        //var user = new User
        //{
        //    UserName = userModel.UserName,
        //    PasswordHash = passwordHash,
        //    Role = SubmissionRoles.Judge.ToString()
        //};

        //_logger.LogInformation("Initializing the application with user {UserName}", user.UserName.SanitizeForLogging());

        //await _usersService.CreateAsync(user, cancellationToken);

        //return Ok(user.ToModel());

        // TODO: Need to implement initialization logic
        throw new NotImplementedException("Initialization endpoint is not implemented.");
    }

    private async Task<bool> IsInitialized(CancellationToken cancellationToken)
    {
        var users = await _usersService.GetAsync(cancellationToken);
        return users.Count > 0;
    }
}
