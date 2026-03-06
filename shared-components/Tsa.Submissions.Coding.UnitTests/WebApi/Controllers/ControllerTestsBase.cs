using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using Tsa.Submissions.Coding.UnitTests.Helpers.Reflection;
using Tsa.Submissions.Coding.WebApi.Authorization;
using Xunit;

namespace Tsa.Submissions.Coding.UnitTests.WebApi.Controllers;

/// <summary>
///     Provides a base class for controller unit tests with reusable test methods
///     for validating common controller attributes and configurations.
/// </summary>
/// <remarks>
///     This base class implements the Template Method pattern, allowing derived classes
///     to specify controller-specific configuration (like authorization requirements)
///     while inheriting common test logic. Each test method should be called from
///     a [Fact] method in the derived class to maintain individual test visibility
///     in Visual Studio Test Explorer.
/// </remarks>
[ExcludeFromCodeCoverage]
public abstract class ControllerTestsBase<TController> where TController : class
{
    // TODO: Account for method overloads in these arrays
    // At present, Get is the only with overloads and both overloads require the same authorization, so it doesn't cause test failures.
    // If future methods have overloads with different authorization requirements, the tests will need to be updated to account for that.

    /// <summary>
    ///     Gets the array of method names that allow all authenticated roles.
    /// </summary>
    /// <remarks>
    ///     Override this property in derived classes to specify which controller methods
    ///     should have [Authorize] with <see cref="SubmissionRoles.All" />.
    /// </remarks>
    protected abstract string[] MethodsForAllRoles { get; }

    /// <summary>
    ///     Gets the array of method names that require the Judge role.
    /// </summary>
    /// <remarks>
    ///     Override this property in derived classes to specify which controller methods
    ///     should have [Authorize] with <see cref="SubmissionRoles.Judge" />.
    /// </remarks>
    protected abstract string[] MethodsForJudgesOnly { get; }

    protected abstract string[] MethodsForJudgesOrParticipants { get; }

    protected abstract string[] MethodsForJudgesOrSystem { get; }

    protected abstract string[] MethodsForParticipantsOnly { get; }

    protected abstract string[] MethodsForSystemOnly { get; }

    /// <summary>
    ///     Verifies that the controller has the expected number of public methods.
    /// </summary>
    /// <remarks>
    ///     This test helps catch when methods are added or removed from the controller
    ///     without updating the test configuration arrays. Method overloads are counted
    ///     as a single method name using Distinct.
    /// </remarks>
    protected void ClassHasExpectedNumberOfPublicMethods()
    {
        var methodInfos = TypeHelpers.GetPublicMethods(typeof(TController));

        // Use Distinct to ensure we are counting unique method names, which accounts for method overloads
        var uniqueMethodNames = methodInfos.Select(methodInfo => methodInfo.Name).Distinct().ToArray();

        var expectedMethodCount =
            MethodsForJudgesOnly.Length +
            MethodsForAllRoles.Length +
            MethodsForJudgesOrParticipants.Length +
            MethodsForJudgesOrSystem.Length +
            MethodsForParticipantsOnly.Length +
            MethodsForSystemOnly.Length;

        Assert.Equal(expectedMethodCount, uniqueMethodNames.Length);
    }

    /// <summary>
    ///     An abstract test method that verifies all public controller methods have the [Authorize] attribute with the proper
    ///     roles assigned.
    /// </summary>
    /// <remarks>
    ///     This method needs to be implemented as an abstract method so that it can be decorated with the [Fact] attribute in
    ///     the derived test class, ensuring that it is executed as a test and appears in the test explorer. The actual
    ///     implementation of this method will call the protected helper method
    ///     PublicMethodsHaveAuthorizeAttributeWithProperRoles, passing in the controller type to be tested.
    /// </remarks>
    public abstract void Controller_Public_Methods_Should_Have_Authorize_Attribute_With_Proper_Roles();

    /// <summary>
    ///     An abstract test method that verifies all public controller methods have the appropriate HTTP method attributes
    ///     (e.g., [HttpGet], [HttpPost], etc.).
    /// </summary>
    /// <remarks>
    ///     This method needs to be implemented as an abstract method so that it can be decorated with the [Fact] attribute in
    ///     the derived test class, ensuring that it is executed as a test and appears in the test explorer. The actual
    ///     implementation of this method will call the protected helper method
    ///     PublicMethodsHaveHttpMethodAttribute, passing in the controller type to be tested.
    /// </remarks>
    public abstract void Controller_Public_Methods_Should_Have_HttpMethod_Attributes();

    public abstract void Controller_Should_Have_ApiController_Attribute();

    public abstract void Controller_Should_Have_Expected_Number_Of_Public_Methods();

    public abstract void Controller_Should_Have_Produces_Attribute();

    public abstract void Controller_Should_Have_Route_Attribute();

    public abstract void Controller_Should_Not_Have_Public_Methods_Without_Http_Attributes();

    protected void HasProducesAttribute(params string[] contentTypes)
    {
        Assert.True(TypeHelpers.ClassHasSingleAttribute<ProducesAttribute>(typeof(TController)));

        var producesAttribute = TypeHelpers.GetClassAttribute<ProducesAttribute>(typeof(TController));

        Assert.NotNull(producesAttribute);
        Assert.Equal(contentTypes.Length, producesAttribute.ContentTypes.Count);

        foreach (var contentType in contentTypes)
        {
            Assert.Contains(contentType, producesAttribute.ContentTypes);
        }
    }

    /// <summary>
    ///     Verifies that the controller has a [Route] attribute with the expected template.
    /// </summary>
    /// <param name="expectedRoute">The expected route template (e.g., "api/problems")</param>
    /// <remarks>
    ///     This test ensures that the controller's base route is correctly configured
    ///     and matches the expected API endpoint structure.
    /// </remarks>
    protected void HasRouteAttribute(string expectedRoute)
    {
        Assert.True(TypeHelpers.ClassHasSingleAttribute<RouteAttribute>(typeof(TController)));

        var routeAttribute = TypeHelpers.GetClassAttribute<RouteAttribute>(typeof(TController));
        // Null forgiveness is used here because the previous assertion guarantees the attribute exists
        Assert.Equal(expectedRoute, routeAttribute!.Template);
    }

    /// <summary>
    ///     Verifies that all public controller methods have the [Authorize] attribute
    ///     with the appropriate roles assigned.
    /// </summary>
    /// <remarks>
    ///     This test ensures that no endpoints are accidentally exposed without proper authorization.
    ///     The expected roles for each method are determined by checking the
    ///     <see cref="MethodsForAllRoles" /> and <see cref="MethodsForJudgesOnly" /> arrays.
    ///     Any method not found in these arrays will cause the test to fail, forcing
    ///     developers to explicitly specify authorization requirements for new methods.
    /// </remarks>
    protected void PublicMethodsHaveAuthorizeAttributeWithProperRoles()
    {
        // Arrange
        var methodInfos = TypeHelpers.GetPublicMethods(typeof(TController));

        // Assert
        Assert.NotEmpty(methodInfos);

        foreach (var methodInfo in methodInfos)
        {
            // Get the [Authorize] attribute
            var authorizeAttribute = TypeHelpers.GetMethodAttribute<AuthorizeAttribute>(methodInfo);
            Assert.NotNull(authorizeAttribute);

            // Determine expected roles
            var expectedRoles = ControllerMethodHelper.GetExpectedAuthorizationRoles(
                methodInfo,
                MethodsForAllRoles,
                MethodsForJudgesOnly,
                MethodsForJudgesOrParticipants,
                MethodsForJudgesOrSystem,
                MethodsForParticipantsOnly,
                MethodsForSystemOnly);

            // Verify roles match
            Assert.Equal(expectedRoles, authorizeAttribute.Roles);
        }
    }

    /// <summary>
    ///     Verifies that there are no public methods without HTTP method attributes.
    /// </summary>
    /// <remarks>
    ///     This test provides an additional safety check to catch any public methods
    ///     that are missing HTTP method attributes entirely. This helps prevent
    ///     accidentally exposing methods that weren't intended to be API endpoints.
    /// </remarks>
    protected void PublicMethodsHaveHttpAttributes()
    {
        var methodInfos = TypeHelpers.GetPublicMethods(typeof(TController));

        var methodsWithoutHttpAttributes = methodInfos
            .Where(methodInfo => !TypeHelpers.MethodHasSingleAttribute<HttpMethodAttribute>(methodInfo))
            .Select(methodInfo => methodInfo.Name)
            .ToList();

        Assert.Empty(methodsWithoutHttpAttributes);
    }

    /// <summary>
    ///     Verifies that all public controller methods have the appropriate HTTP method attribute.
    /// </summary>
    /// <remarks>
    ///     This test verifies that:
    ///     1. Every public method has at least one HTTP method attribute (HttpGet, HttpPost, etc.)
    ///     2. The specific HTTP method attribute matches the method's naming convention
    ///     (e.g., methods starting with "Get" should have [HttpGet])
    /// </remarks>
    protected void PublicMethodsHaveHttpMethodAttribute()
    {
        // Arrange
        var methodInfos = TypeHelpers.GetPublicMethods(typeof(TController));

        // Assert
        Assert.NotEmpty(methodInfos);

        foreach (var methodInfo in methodInfos)
        {
            // Verify it has some HTTP method attribute
            Assert.True(
                TypeHelpers.MethodHasSingleAttribute<HttpMethodAttribute>(methodInfo),
                $"Method '{methodInfo.Name}' is missing an HTTP method attribute");

            // Determine expected HTTP method attribute type
            var expectedAttributeType = ControllerMethodHelper.GetExpectedHttpMethodAttributeType(methodInfo);

            // Verify it has the correct specific HTTP method attribute
            Assert.True(
                TypeHelpers.MethodHasSingleAttribute(methodInfo, expectedAttributeType),
                $"Method '{methodInfo.Name}' should have [{expectedAttributeType.Name}] but doesn't");
        }
    }
}
