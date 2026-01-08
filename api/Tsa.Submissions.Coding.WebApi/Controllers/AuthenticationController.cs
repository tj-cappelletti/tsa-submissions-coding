using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Tsa.Submissions.Coding.WebApi.Configuration;
using Tsa.Submissions.Coding.WebApi.ExtensionMethods;
using Tsa.Submissions.Coding.WebApi.Models;
using Tsa.Submissions.Coding.WebApi.Services;

namespace Tsa.Submissions.Coding.WebApi.Controllers
{
    [Route("api/auth")]
    [ApiController]
    [Produces("application/json")]
    public class AuthenticationController : ControllerBase
    {
        private readonly JwtSettings _jwtSettings;
        private readonly ILogger<AuthenticationController> _logger;
        private readonly IUsersService _usersService;

        public AuthenticationController(IOptions<JwtSettings> jwtSettings, ILogger<AuthenticationController> logger, IUsersService usersService)
        {
            _jwtSettings = jwtSettings.Value;
            _logger = logger;
            _usersService = usersService;
        }

        [AllowAnonymous]
        [HttpPost("login")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(LoginResponseModel))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> Login([FromBody] AuthenticationModel authenticationModel, CancellationToken cancellationToken = default)
        {
            var user = await _usersService.GetByUserNameAsync(authenticationModel.UserName, cancellationToken);

            if (user == null)
            {
                _logger.LogWarning("Login failed for user {UserName}: User not found", authenticationModel.UserName.SanitizeForLogging());
                return Unauthorized(ApiErrorResponseModel.Unauthorized);
            }

            var passwordVerified = BC.Verify(authenticationModel.Password, user.PasswordHash);

            if (!passwordVerified)
            {
                _logger.LogWarning("Login failed for user {UserName}: Invalid password", authenticationModel.UserName.SanitizeForLogging());
                return Unauthorized(ApiErrorResponseModel.Unauthorized);
            }

            _logger.LogInformation("User {UserName} logged in successfully", user.UserName);

            var tokenHandler = new JwtSecurityTokenHandler();

            _logger.LogInformation("Creating JWT token for user {UserName}", user.UserName.SanitizeForLogging());
            _logger.LogInformation("Token expiration time: {TokenExpiration}", _jwtSettings.ExpirationInHours);
            var tokenExpiration = DateTime.UtcNow.AddHours(_jwtSettings.ExpirationInHours);

            _logger.LogInformation("Fetching JWT key");
            // JWT Settings are validated in Startup.cs
            var key = Encoding.UTF8.GetBytes(_jwtSettings.Key!);

            _logger.LogInformation("Creating token descriptor");
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Audience = _jwtSettings.Audience,
                Issuer = _jwtSettings.Issuer,
                Expires = tokenExpiration,
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature),
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.Name, user.UserName!),
                    new Claim(ClaimTypes.Role, user.Role!)
                }),
            };

            _logger.LogInformation("Creating token");
            var token = tokenHandler.CreateToken(tokenDescriptor);

            _logger.LogInformation("Serializing token");
            var tokenString = tokenHandler.WriteToken(token);

            _logger.LogInformation("Token created successfully");
            return Ok(new LoginResponseModel(tokenString, tokenExpiration));
        }
    }
}
