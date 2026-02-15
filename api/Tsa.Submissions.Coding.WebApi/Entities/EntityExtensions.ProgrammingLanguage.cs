using System;
using System.Collections.Generic;
using System.Linq;
using Tsa.Submissions.Coding.Contracts.Languages;

namespace Tsa.Submissions.Coding.WebApi.Entities;

public static partial class EntityExtensions
{
    /// <summary>
    ///     Converts a <see cref="ProgrammingLanguage" /> to a <see cref="ProgrammingLanguageResponse" />.
    /// </summary>
    /// <param name="programmingLanguage">The programming language to convert.</param>
    /// <returns>A <see cref="ProgrammingLanguageResponse" /> representing the programming language.</returns>
    /// <exception cref="InvalidOperationException">Thrown if any required properties of the programming language are missing.</exception>
    public static ProgrammingLanguageResponse ToResponse(this ProgrammingLanguage programmingLanguage)
    {
        if (string.IsNullOrWhiteSpace(programmingLanguage.Id))
        {
            throw new InvalidOperationException("Programming Language ID is required.");
        }

        if (string.IsNullOrWhiteSpace(programmingLanguage.Identifier))
        {
            throw new InvalidOperationException("Programming Language Identifier is required.");
        }

        if (string.IsNullOrWhiteSpace(programmingLanguage.Name))
        {
            throw new InvalidOperationException("Programming Language Name is required.");
        }

        if (string.IsNullOrWhiteSpace(programmingLanguage.FileExtension))
        {
            throw new InvalidOperationException("Programming Language File Extension is required.");
        }

        return new ProgrammingLanguageResponse(
            programmingLanguage.Id,
            programmingLanguage.Identifier,
            programmingLanguage.Name,
            programmingLanguage.FileExtension,
            programmingLanguage.IsEnabled,
            programmingLanguage.Versions.Select(v => v.ToResponse()));
    }

    /// <summary>
    ///     Converts a <see cref="ProgrammingLanguageVersion" /> to a <see cref="ProgrammingLanguageVersionResponse" />.
    /// </summary>
    /// <param name="programmingLanguageVersion">The programming language version to convert.</param>
    /// <returns>A <see cref="ProgrammingLanguageVersionResponse" /> representing the programming language version.</returns>
    /// <exception cref="InvalidOperationException">
    ///     Thrown if any required properties of the programming language version are
    ///     missing.
    /// </exception>
    public static ProgrammingLanguageVersionResponse ToResponse(this ProgrammingLanguageVersion programmingLanguageVersion)
    {
        if (string.IsNullOrWhiteSpace(programmingLanguageVersion.DisplayName))
        {
            throw new InvalidOperationException("Programming Language Version Display Name is required.");
        }

        if (string.IsNullOrWhiteSpace(programmingLanguageVersion.VersionTag))
        {
            throw new InvalidOperationException("Programming Language Version Tag is required.");
        }

        return new ProgrammingLanguageVersionResponse(
            programmingLanguageVersion.DisplayName,
            programmingLanguageVersion.IsDefault,
            programmingLanguageVersion.VersionTag);
    }

    /// <summary>
    ///     Converts a collection of <see cref="ProgrammingLanguage" /> to a collection of
    ///     <see cref="ProgrammingLanguageResponse" />.
    /// </summary>
    /// <param name="programmingLanguages">The collection of programming languages to convert.</param>
    /// <returns>A collection of <see cref="ProgrammingLanguageResponse" /> representing the programming languages.</returns>
    /// <exception cref="InvalidOperationException">
    ///     Thrown if any required properties of the programming language version are
    ///     missing.
    /// </exception>
    public static IEnumerable<ProgrammingLanguageResponse> ToResponses(this IEnumerable<ProgrammingLanguage> programmingLanguages)
    {
        return programmingLanguages.Select(programmingLanguage => programmingLanguage.ToResponse());
    }
}
