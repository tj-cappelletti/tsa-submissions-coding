using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;

namespace Tsa.Submissions.Coding.UnitTests.Helpers.Reflection;

/// <summary>
///     Provides reflection helper methods for testing class and method attributes.
/// </summary>
[ExcludeFromCodeCoverage]
internal static class TypeHelpers
{
    /// <summary>
    ///     Determines whether a class has exactly one attribute of the specified type.
    /// </summary>
    /// <typeparam name="T">The attribute type to check for</typeparam>
    /// <param name="classType">The class type to inspect</param>
    /// <returns>True if exactly one attribute of the specified type is found; otherwise, false</returns>
    /// <remarks>
    ///     This method is useful for verifying that controller classes have required attributes
    ///     such as [ApiController], [Route], or [Produces].
    /// </remarks>
    public static bool ClassHasSingleAttribute<T>(Type classType) where T : Attribute
    {
        var attributes = classType.GetCustomAttributes(typeof(T), false);

        return attributes.Length == 1;
    }

    /// <summary>
    ///     Gets a single attribute of type <typeparamref name="T" /> from a class.
    /// </summary>
    /// <typeparam name="T">The attribute type to retrieve</typeparam>
    /// <param name="classType">The class type to inspect</param>
    /// <returns>The attribute if found; otherwise, null</returns>
    /// <exception cref="InvalidOperationException">
    ///     Thrown when multiple attributes of the specified type are found
    /// </exception>
    /// <remarks>
    ///     This method is useful for retrieving and validating controller-level attributes
    ///     such as [Route] or [Produces] and inspecting their properties.
    /// </remarks>
    public static T? GetClassAttribute<T>(Type classType) where T : Attribute
    {
        var attributes = classType.GetCustomAttributes(typeof(T), false);
        return attributes.Length switch
        {
            0 => null,
            > 1 => throw new InvalidOperationException(
                $"The class '{classType.Name}' has multiple attributes of type '{typeof(T).Name}'. Expected exactly one."),
            _ => (T)attributes[0]
        };
    }

    /// <summary>
    ///     Gets a single attribute of type <typeparamref name="T" /> from a method.
    /// </summary>
    /// <typeparam name="T">The attribute type to retrieve</typeparam>
    /// <param name="methodInfo">The method to inspect</param>
    /// <returns>The attribute if found; otherwise, null</returns>
    /// <exception cref="InvalidOperationException">
    ///     Thrown when multiple attributes of the specified type are found
    /// </exception>
    /// <remarks>
    ///     This method is useful for retrieving and validating method-level attributes
    ///     such as [Authorize], [HttpGet], [HttpPost], etc., and inspecting their properties.
    /// </remarks>
    public static T? GetMethodAttribute<T>(MethodInfo methodInfo) where T : Attribute
    {
        var attributes = methodInfo.GetCustomAttributes(typeof(T), false);

        return attributes.Length switch
        {
            0 => null,
            > 1 => throw new InvalidOperationException(
                $"The method '{methodInfo.DeclaringType?.Name}.{methodInfo.Name}' has multiple attributes of type '{typeof(T).Name}'. Expected exactly one."),
            _ => (T)attributes[0]
        };
    }

    /// <summary>
    ///     Gets all public instance methods declared directly on the specified type.
    /// </summary>
    /// <param name="type">The type to inspect</param>
    /// <returns>An array of public instance methods</returns>
    /// <remarks>
    ///     This method excludes:
    ///     - Inherited methods (only returns methods declared on the type itself)
    ///     - Property getters/setters (IsSpecialName)
    ///     - Event add/remove methods (IsSpecialName)
    ///     - Operator overloads (IsSpecialName)
    ///     This is useful for testing controller action methods without including infrastructure methods.
    /// </remarks>
    public static MethodInfo[] GetPublicMethods(Type type)
    {
        return type
            .GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly)
            .Where(methodInfo => !methodInfo.IsSpecialName)
            .ToArray();
    }

    /// <summary>
    ///     Determines whether a method has exactly one attribute of the specified type.
    /// </summary>
    /// <typeparam name="T">The attribute type to check for</typeparam>
    /// <param name="methodInfo">The method to inspect</param>
    /// <returns>True if exactly one attribute of the specified type is found; otherwise, false</returns>
    /// <remarks>
    ///     This method is useful for verifying that action methods have required attributes
    ///     such as [Authorize] or HTTP method attributes ([HttpGet], [HttpPost], etc.).
    /// </remarks>
    public static bool MethodHasSingleAttribute<T>(MethodInfo methodInfo) where T : Attribute
    {
        return MethodHasSingleAttribute(methodInfo, typeof(T));
    }

    /// <summary>
    ///     Determines whether a method has exactly one attribute of the specified type.
    /// </summary>
    /// <param name="methodInfo">The method to inspect</param>
    /// <param name="attributeType">The attribute type to check for</param>
    /// <returns>True if exactly one attribute of the specified type is found; otherwise, false</returns>
    /// <remarks>
    ///     This overload accepts a Type parameter, allowing for dynamic attribute type checking.
    ///     Useful when the attribute type is determined at runtime or when checking for base attribute types.
    /// </remarks>
    public static bool MethodHasSingleAttribute(MethodInfo methodInfo, Type attributeType)
    {
        var attributes = methodInfo.GetCustomAttributes(attributeType, false);

        return attributes.Length == 1;
    }
}
