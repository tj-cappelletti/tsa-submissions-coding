using System.IdentityModel.Tokens.Jwt;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Moq;
using Tsa.Submissions.Coding.WebApi.Configuration;
using Tsa.Submissions.Coding.WebApi.Controllers;
using Tsa.Submissions.Coding.WebApi.Entities;
using Tsa.Submissions.Coding.WebApi.Models;
using Tsa.Submissions.Coding.WebApi.Services;
using Xunit;

namespace Tsa.Submissions.Coding.UnitTests.WebApi.Controllers;

public class AuthenticationControllerTests
{
    private readonly AuthenticationController _controller;
    private readonly IOptions<JwtSettings> _jwtSettings;
    private readonly Mock<IUsersService> _mockUsersService;

    public AuthenticationControllerTests()
    {
        _mockUsersService = new Mock<IUsersService>();

        Mock<ILogger<AuthenticationController>> mockLogger = new();

        _jwtSettings = Options.Create(new JwtSettings
        {
            Key = "!!!!####____YourSuperSecretKeyHere____####!!!!",
            Issuer = "YourIssuer",
            Audience = "YourAudience",
            ExpirationInHours = 1
        });

        _controller = new AuthenticationController(_jwtSettings, mockLogger.Object, _mockUsersService.Object);
    }

    [Fact]
    [Trait("TestCategory", "UnitTest")]
    public async Task Login_InvalidPassword_ReturnsUnauthorized()
    {
        // Arrange
        var authenticationModel = new AuthenticationModel
        {
            UserName = "validuser",
            Password = "wrongpassword"
        };

        var user = new User
        {
            UserName = "validuser",
            PasswordHash = BC.HashPassword("correctpassword"),
            Role = "User"
        };

        _mockUsersService
            .Setup(service => service.GetByUserNameAsync(authenticationModel.UserName, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        // Act
        var result = await _controller.Login(authenticationModel);

        // Assert
        var unauthorizedResult = Assert.IsType<UnauthorizedObjectResult>(result);
        Assert.Equal(StatusCodes.Status401Unauthorized, unauthorizedResult.StatusCode);
    }

    [Fact]
    [Trait("TestCategory", "UnitTest")]
    public async Task Login_UserNotFound_ReturnsUnauthorized()
    {
        // Arrange
        var authenticationModel = new AuthenticationModel
        {
            UserName = "nonexistentuser",
            Password = "password"
        };

        _mockUsersService
            .Setup(service => service.GetByUserNameAsync(authenticationModel.UserName, It.IsAny<CancellationToken>()))
            .ReturnsAsync((User?)null);

        // Act
        var result = await _controller.Login(authenticationModel);

        // Assert
        var unauthorizedResult = Assert.IsType<UnauthorizedObjectResult>(result);

        Assert.Equal(StatusCodes.Status401Unauthorized, unauthorizedResult.StatusCode);
    }

    [Fact]
    [Trait("TestCategory", "UnitTest")]
    public async Task Login_ValidCredentials_ReturnsToken()
    {
        // Arrange
        var authenticationModel = new AuthenticationModel
        {
            UserName = "validuser",
            Password = "correctpassword"
        };

        var user = new User
        {
            UserName = "validuser",
            PasswordHash = BC.HashPassword("correctpassword"),
            Role = "User"
        };

        _mockUsersService
            .Setup(service => service.GetByUserNameAsync(authenticationModel.UserName, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        // Act
        var result = await _controller.Login(authenticationModel);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);

        Assert.Equal(StatusCodes.Status200OK, okResult.StatusCode);
        Assert.IsType<LoginResponseModel>(okResult.Value);

        var loginResponseModel = (LoginResponseModel?)okResult.Value;

        Assert.NotNull(loginResponseModel);
        Assert.NotNull(loginResponseModel.Token);

        // Validate the token
        var tokenHandler = new JwtSecurityTokenHandler();

        var key = Encoding.UTF8.GetBytes(_jwtSettings.Value.Key!);

        tokenHandler.ValidateToken(loginResponseModel.Token, new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = _jwtSettings.Value.Issuer,
            ValidAudience = _jwtSettings.Value.Audience,
            IssuerSigningKey = new SymmetricSecurityKey(key)
        }, out _);
    }
}
