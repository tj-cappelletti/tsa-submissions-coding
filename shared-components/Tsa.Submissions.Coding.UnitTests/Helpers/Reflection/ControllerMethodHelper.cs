using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using Microsoft.AspNetCore.Mvc;
using Tsa.Submissions.Coding.WebApi.Authorization;

namespace Tsa.Submissions.Coding.UnitTests.Helpers.Reflection;

/// <summary>
///     Provides helper methods for validating controller method configurations.
/// </summary>
[ExcludeFromCodeCoverage]
public static class ControllerMethodHelper
{
    /// <summary>
    ///     Determines the expected authorization roles for a controller method.
    /// </summary>
    /// <param name="methodInfo">The method to inspect</param>
    /// <param name="allRolesMethods">Array of method names that allow all authenticated roles</param>
    /// <param name="judgeOrParticipantMethods">Array of method names that require Judge or Participant role</param>
    /// <param name="judgeOrSystemMethods">Array of method names that require Judge or System role</param>
    /// <param name="judgeOnlyMethods">Array of method names that require Judge role</param>
    /// <param name="participantOnlyMethods">Array of method names that require Participant role</param>
    /// <param name="systemOnlyMethods">Array of method names that require System role</param>
    /// <returns>The expected role(s) as defined in <see cref="SubmissionRoles" /></returns>
    /// <exception cref="InvalidOperationException">
    ///     Thrown when the method is not in either the judgeOnlyMethods or allRolesMethods arrays
    /// </exception>
    /// <remarks>
    ///     This method ensures that every controller method has explicit authorization requirements defined.
    ///     If a method is not found in either array, the test will fail, forcing developers to
    ///     consciously decide the authorization level for new methods.
    /// </remarks>
    public static string GetExpectedAuthorizationRoles(
        MethodInfo methodInfo,
        string[] allRolesMethods,
        string[] judgeOnlyMethods,
        string[] judgeOrParticipantMethods,
        string[] judgeOrSystemMethods,
        string[] participantOnlyMethods,
        string[] systemOnlyMethods)
    {
        var methodName = methodInfo.Name;

        if (allRolesMethods.Contains(methodName))
        {
            return SubmissionRoles.All;
        }

        if (judgeOnlyMethods.Contains(methodName))
        {
            return SubmissionRoles.Judge;
        }

        if (judgeOrParticipantMethods.Contains(methodName))
        {
            return SubmissionRoles.JudgeOrParticipant;
        }

        if (judgeOrSystemMethods.Contains(methodName))
        {
            return SubmissionRoles.JudgeOrSystem;
        }

        if (participantOnlyMethods.Contains(methodName))
        {
            return SubmissionRoles.Participant;
        }

        if (systemOnlyMethods.Contains(methodName))
        {
            return SubmissionRoles.System;
        }

        throw new InvalidOperationException(
            $"Method '{methodInfo.DeclaringType?.Name}.{methodName}' is not in the expected authorization list. " +
            "Add it to one of the MethodsFor* arrays in the test class.");
    }

    /// <summary>
    ///     Determines the expected HTTP method attribute type for a controller method based on its name.
    /// </summary>
    /// <param name="methodInfo">The method to inspect</param>
    /// <returns>The expected HTTP method attribute type</returns>
    /// <exception cref="InvalidOperationException">
    ///     Thrown when the method name doesn't match a known HTTP operation pattern
    /// </exception>
    /// <remarks>
    ///     This method uses naming conventions to determine the expected HTTP verb:
    ///     - Methods starting with "Delete" -> HttpDeleteAttribute
    ///     - Methods starting with "Get" -> HttpGetAttribute
    ///     - Methods starting with "Post" -> HttpPostAttribute
    ///     - Methods starting with "Put" -> HttpPutAttribute
    ///     - And so on for other HTTP verbs
    /// </remarks>
    public static Type GetExpectedHttpMethodAttributeType(MethodInfo methodInfo)
    {
        return methodInfo.Name switch
        {
            var name when name.StartsWith("Delete", StringComparison.Ordinal) => typeof(HttpDeleteAttribute),
            var name when name.StartsWith("Get", StringComparison.Ordinal) => typeof(HttpGetAttribute),
            var name when name.StartsWith("Head", StringComparison.Ordinal) => typeof(HttpHeadAttribute),
            var name when name.StartsWith("Options", StringComparison.Ordinal) => typeof(HttpOptionsAttribute),
            var name when name.StartsWith("Patch", StringComparison.Ordinal) => typeof(HttpPatchAttribute),
            var name when name.StartsWith("Post", StringComparison.Ordinal) => typeof(HttpPostAttribute),
            var name when name.StartsWith("Put", StringComparison.Ordinal) => typeof(HttpPutAttribute),
            _ => throw new InvalidOperationException(
                $"Method '{methodInfo.DeclaringType?.Name}.{methodInfo.Name}' does not follow a recognized HTTP operation naming pattern. " +
                "Method names must start with an HTTP verb (Delete, Get, Post, Put, etc.).")
        };
    }
}
