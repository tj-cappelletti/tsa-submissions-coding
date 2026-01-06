using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Tsa.Submissions.Coding.WebApi.Controllers;
using Tsa.Submissions.Coding.WebApi.Models;
using Tsa.Submissions.Coding.WebApi.Services;
using Xunit;

namespace Tsa.Submissions.Coding.UnitTests.WebApi.Controllers;

[ExcludeFromCodeCoverage]
public class StatusControllerTests
{
    private static void AssertServiceStatus(ServicesStatusModel servicesStatusModel, PingableServiceFailures pingableServiceFailures)
    {
        Assert.Equal(servicesStatusModel.ProblemsServiceIsAlive, !pingableServiceFailures.HasFlag(PingableServiceFailures.Problems));
        Assert.Equal(servicesStatusModel.SubmissionsServiceIsAlive, !pingableServiceFailures.HasFlag(PingableServiceFailures.Submissions));
        Assert.Equal(servicesStatusModel.TestSetsServiceIsAlive, !pingableServiceFailures.HasFlag(PingableServiceFailures.TestSets));
    }

    private static List<IPingableService> BuildHealthyPingableServices()
    {
        var pingableServices = new List<IPingableService>();

        foreach (var serviceType in GetServiceTypes())
        {
            var mockedPingableService = new Mock<IPingableService>();

            mockedPingableService
                .Setup(pingableService => pingableService.PingAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            mockedPingableService
                .Setup(pingableService => pingableService.ServiceName)
                .Returns(serviceType.Name.Replace("Service", string.Empty));

            pingableServices.Add(mockedPingableService.Object);
        }

        return pingableServices;
    }

    private static IEnumerable<IPingableService> BuildUnhealthyPingableServices(
        PingableServiceFailures pingableServiceFailures,
        ServiceFailureType serviceFailureType)
    {
        foreach (var serviceType in GetServiceTypes())
        {
            var serviceName = serviceType.Name.Replace("Service", string.Empty);

            var pingableServiceFailureFlag = Enum.Parse<PingableServiceFailures>(serviceName);

            var mockedPingableService = new Mock<IPingableService>();

            mockedPingableService
                .Setup(pingableService => pingableService.ServiceName)
                .Returns(serviceName);

            if (pingableServiceFailures.HasFlag(pingableServiceFailureFlag))
            {
                switch (serviceFailureType)
                {
                    case ServiceFailureType.ExceptionThrown:
                        mockedPingableService
                            .Setup(pingableService => pingableService.PingAsync(It.IsAny<CancellationToken>()))
                            .Throws(new Exception("Test handling exceptions"));
                        break;

                    case ServiceFailureType.PingFailed:
                        mockedPingableService.Setup(pingableService => pingableService.PingAsync(It.IsAny<CancellationToken>()))
                            .ReturnsAsync(false);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(serviceFailureType), serviceFailureType, null);
                }
            }
            else
            {
                mockedPingableService
                    .Setup(pingableService => pingableService.PingAsync(It.IsAny<CancellationToken>()))
                    .ReturnsAsync(true);
            }

            yield return mockedPingableService.Object;
        }
    }

    [Theory]

    #region Inline Data

    // Generate each set here: https://planetcalc.com/3757/
    [InlineData(PingableServiceFailures.Problems | PingableServiceFailures.Submissions | PingableServiceFailures.TestSets)]
    [InlineData(PingableServiceFailures.Problems | PingableServiceFailures.Submissions)]
    [InlineData(PingableServiceFailures.Problems | PingableServiceFailures.TestSets)]
    [InlineData(PingableServiceFailures.Submissions | PingableServiceFailures.TestSets)]
    [InlineData(PingableServiceFailures.Problems)]
    [InlineData(PingableServiceFailures.Submissions)]
    [InlineData(PingableServiceFailures.TestSets)]

    #endregion

    [Trait("TestCategory", "UnitTest")]
    public async Task Get_Should_Return_InternalServerError(PingableServiceFailures pingableServiceFailures)
    {
        // Arrange
        var pingableServices = BuildUnhealthyPingableServices(pingableServiceFailures, ServiceFailureType.PingFailed);

        var statusController = new StatusController(pingableServices);

        // Act
        var actionResult = await statusController.Get();

        // Assert
        Assert.NotNull(actionResult);
        Assert.IsType<ActionResult<ServicesStatusModel>>(actionResult);

        var objectResult = actionResult.Result as ObjectResult;
        Assert.NotNull(objectResult);
        Assert.NotNull(objectResult.StatusCode);
        Assert.Equal(500, objectResult.StatusCode!.Value);
        Assert.NotNull(objectResult.Value);
        Assert.IsType<ServicesStatusModel>(objectResult.Value);

        var servicesStatus = objectResult.Value as ServicesStatusModel;
        Assert.False(servicesStatus!.IsHealthy);

        AssertServiceStatus(servicesStatus, pingableServiceFailures);
    }

    [Theory]

    #region Inline Data

    // Generate each set here: https://planetcalc.com/3757/
    [InlineData(PingableServiceFailures.Problems | PingableServiceFailures.Submissions | PingableServiceFailures.TestSets)]
    [InlineData(PingableServiceFailures.Problems | PingableServiceFailures.Submissions)]
    [InlineData(PingableServiceFailures.Problems | PingableServiceFailures.TestSets)]
    [InlineData(PingableServiceFailures.Submissions | PingableServiceFailures.TestSets)]
    [InlineData(PingableServiceFailures.Problems)]
    [InlineData(PingableServiceFailures.Submissions)]
    [InlineData(PingableServiceFailures.TestSets)]

    #endregion

    [Trait("TestCategory", "UnitTest")]
    public async Task Get_Should_Return_InternalServerError_When_Exception_Is_Thrown(PingableServiceFailures pingableServiceFailures)
    {
        // Arrange
        var pingableServices = BuildUnhealthyPingableServices(pingableServiceFailures, ServiceFailureType.ExceptionThrown);

        var statusController = new StatusController(pingableServices);

        // Act
        var actionResult = await statusController.Get();

        // Assert
        Assert.NotNull(actionResult);
        Assert.IsType<ActionResult<ServicesStatusModel>>(actionResult);

        var objectResult = actionResult.Result as ObjectResult;
        Assert.NotNull(objectResult);
        Assert.NotNull(objectResult.StatusCode);
        Assert.Equal(500, objectResult.StatusCode!.Value);
        Assert.NotNull(objectResult.Value);
        Assert.IsType<ServicesStatusModel>(objectResult.Value);

        var servicesStatus = objectResult.Value as ServicesStatusModel;

        Assert.False(servicesStatus!.IsHealthy);

        AssertServiceStatus(servicesStatus, pingableServiceFailures);
    }

    private static IEnumerable<Type> GetServiceTypes()
    {
        yield return typeof(ProblemsService);
        yield return typeof(SubmissionsService);
        yield return typeof(TestSetsService);
    }

    [Fact]
    [Trait("TestCategory", "UnitTest")]
    public void Controller_Public_Methods_Should_Have_Http_Method_Attribute()
    {
        var statusControllerType = typeof(StatusController);

        var methodInfos = statusControllerType.GetMethods(BindingFlags.DeclaredOnly);

        foreach (var methodInfo in methodInfos)
        {
            // Needs to be nullable so the compiler sees it's initialized
            // The Assert.Fail doesn't tell it that the line it's being used
            // will only ever be hit if it's initialized
            Type? attributeType = null;

            switch (methodInfo.Name.ToLower())
            {
                case "delete":
                    attributeType = typeof(HttpDeleteAttribute);
                    break;
                case "get":
                    attributeType = typeof(HttpGetAttribute);
                    break;
                case "head":
                    attributeType = typeof(HttpHeadAttribute);
                    break;
                case "options":
                    attributeType = typeof(HttpOptionsAttribute);
                    break;
                case "patch":
                    attributeType = typeof(HttpPatchAttribute);
                    break;
                case "post":
                    attributeType = typeof(HttpPostAttribute);
                    break;
                case "put":
                    attributeType = typeof(HttpPutAttribute);
                    break;
                default:
                    Assert.Fail("Unsupported public HTTP operation");
                    break;
            }

            var attributes = methodInfo.GetCustomAttributes(attributeType, false);

            Assert.NotNull(attributes);
            Assert.NotEmpty(attributes);
            Assert.Single(attributes);
        }
    }

    [Fact]
    [Trait("TestCategory", "UnitTest")]
    public void Controller_Should_Have_ApiController_Attribute()
    {
        var statusControllerType = typeof(StatusController);

        var attributes = statusControllerType.GetCustomAttributes(typeof(ApiControllerAttribute), false);

        Assert.NotNull(attributes);
        Assert.NotEmpty(attributes);
        Assert.Single(attributes);
    }

    [Fact]
    [Trait("TestCategory", "UnitTest")]
    public void Controller_Should_Have_Produces_Attribute()
    {
        var statusControllerType = typeof(StatusController);

        var attributes = statusControllerType.GetCustomAttributes(typeof(ProducesAttribute), false);

        Assert.NotNull(attributes);
        Assert.NotEmpty(attributes);
        Assert.Single(attributes);

        var producesAttribute = (ProducesAttribute)attributes[0];

        Assert.Contains("application/json", producesAttribute.ContentTypes);
    }

    [Fact]
    [Trait("TestCategory", "UnitTest")]
    public void Controller_Should_Have_Route_Attribute()
    {
        var statusControllerType = typeof(StatusController);

        var attributes = statusControllerType.GetCustomAttributes(typeof(RouteAttribute), false);

        Assert.NotNull(attributes);
        Assert.NotEmpty(attributes);
        Assert.Single(attributes);

        var routeAttribute = (RouteAttribute)attributes[0];

        Assert.Equal("api/[controller]", routeAttribute.Template);
    }

    [Fact]
    [Trait("TestCategory", "UnitTest")]
    public async Task Get_Should_Return_Ok()
    {
        // Arrange
        var pingableServices = BuildHealthyPingableServices();

        var statusController = new StatusController(pingableServices);

        // Act
        var actionResult = await statusController.Get();

        // Assert
        Assert.NotNull(actionResult);
        Assert.IsType<ActionResult<ServicesStatusModel>>(actionResult);

        var okObjectResult = actionResult.Result as ObjectResult;
        Assert.NotNull(okObjectResult);
        Assert.NotNull(okObjectResult.StatusCode);
        Assert.Equal(200, okObjectResult.StatusCode!.Value);
        Assert.NotNull(okObjectResult.Value);
        Assert.IsType<ServicesStatusModel>(okObjectResult.Value);

        var servicesStatus = okObjectResult.Value as ServicesStatusModel;
        Assert.True(servicesStatus!.IsHealthy);
    }

    [Fact]
    [Trait("TestCategory", "UnitTest")]
    public async Task Get_Should_Return_Ok_When_Unknown_Service_Is_Injected()
    {
        // Arrange
        var pingableServices = BuildHealthyPingableServices();

        var mockedUnknownService = new Mock<IPingableService>();
        mockedUnknownService
            .Setup(pingableService => pingableService.PingAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
        mockedUnknownService
            .Setup(pingableService => pingableService.ServiceName)
            .Returns("Bomb");

        pingableServices.Add(mockedUnknownService.Object);

        var statusController = new StatusController(pingableServices);

        // Act
        var actionResult = await statusController.Get();

        // Assert
        Assert.NotNull(actionResult);
        Assert.IsType<ActionResult<ServicesStatusModel>>(actionResult);

        var okObjectResult = actionResult.Result as ObjectResult;
        Assert.NotNull(okObjectResult);
        Assert.NotNull(okObjectResult.StatusCode);
        Assert.Equal(200, okObjectResult.StatusCode!.Value);
        Assert.NotNull(okObjectResult.Value);
        Assert.IsType<ServicesStatusModel>(okObjectResult.Value);

        var servicesStatus = okObjectResult.Value as ServicesStatusModel;
        Assert.True(servicesStatus!.IsHealthy);
    }
}

[Flags]
public enum PingableServiceFailures
{
    Problems = 1 << 0,
    Submissions = 1 << 1,
    TestSets = 1 << 2
}

public enum ServiceFailureType
{
    ExceptionThrown,
    PingFailed
}
