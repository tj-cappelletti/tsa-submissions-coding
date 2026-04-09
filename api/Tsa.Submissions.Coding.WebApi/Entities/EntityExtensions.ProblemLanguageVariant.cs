using System;
using System.Collections.Generic;
using System.Linq;
using Tsa.Submissions.Coding.Contracts.ProblemLanguageVariants;

namespace Tsa.Submissions.Coding.WebApi.Entities;

public static partial class EntityExtensions
{
    /// <summary>
    ///     Converts a <see cref="ProblemLanguageVariant" /> entity to a lightweight
    ///     <see cref="ProblemLanguageVariantListResponse" />.
    /// </summary>
    /// <param name="variant">The problem language variant entity to convert</param>
    /// <returns>
    ///     A <see cref="ProblemLanguageVariantListResponse" /> containing basic variant information
    /// </returns>
    /// <exception cref="InvalidOperationException">
    ///     Thrown when required fields are missing
    /// </exception>
    /// <remarks>
    ///     This method produces a lightweight response intended for list operations.
    ///     It excludes the reference solution, starter code, test harness code, and workspace files
    ///     to minimize payload size.
    /// </remarks>
    private static ProblemLanguageVariantListResponse ToListResponse(this ProblemLanguageVariant variant)
    {
        if (string.IsNullOrWhiteSpace(variant.Id))
        {
            throw new InvalidOperationException("Problem Language Variant ID is required.");
        }

        if (string.IsNullOrWhiteSpace(variant.ProblemId))
        {
            throw new InvalidOperationException("Problem ID is required.");
        }

        if (string.IsNullOrWhiteSpace(variant.ProgrammingLanguageId))
        {
            throw new InvalidOperationException("Programming Language ID is required.");
        }

        if (string.IsNullOrWhiteSpace(variant.ProgrammingLanguageVersionTag))
        {
            throw new InvalidOperationException("Programming Language Version Tag is required.");
        }

        return new ProblemLanguageVariantListResponse(
            variant.Id,
            variant.ProblemId,
            variant.ProgrammingLanguageId,
            variant.ProgrammingLanguageVersionTag,
            variant.IsActive);
    }

    /// <summary>
    ///     Converts a collection of <see cref="ProblemLanguageVariant" /> entities to lightweight
    ///     <see cref="ProblemLanguageVariantListResponse" /> objects.
    /// </summary>
    /// <param name="variants">The collection of problem language variant entities to convert</param>
    /// <returns>A collection of <see cref="ProblemLanguageVariantListResponse" /> objects</returns>
    /// <exception cref="InvalidOperationException">
    ///     Thrown when required fields are missing from any variant
    /// </exception>
    /// <remarks>
    ///     This method produces lightweight responses optimized for list operations.
    ///     Each response excludes the reference solution, starter code, test harness code,
    ///     and workspace files to minimize payload size.
    ///     Use <see cref="ToResponse(ProblemLanguageVariant)" /> for individual variant details.
    /// </remarks>
    public static IEnumerable<ProblemLanguageVariantListResponse> ToListResponses(this IEnumerable<ProblemLanguageVariant> variants)
    {
        return variants.Select(variant => variant.ToListResponse());
    }

    /// <summary>
    ///     Converts a <see cref="ProblemLanguageVariant" /> entity to a complete
    ///     <see cref="ProblemLanguageVariantResponse" />.
    /// </summary>
    /// <param name="variant">The problem language variant entity to convert</param>
    /// <returns>
    ///     A <see cref="ProblemLanguageVariantResponse" /> containing complete variant data
    ///     including workspace files, test harness code, and reference solution
    /// </returns>
    /// <exception cref="InvalidOperationException">
    ///     Thrown when required fields are missing
    /// </exception>
    /// <remarks>
    ///     This method produces a complete response intended for single variant retrieval.
    ///     Use this for detailed views where complete information is needed.
    /// </remarks>
    public static ProblemLanguageVariantResponse ToResponse(this ProblemLanguageVariant variant)
    {
        if (string.IsNullOrWhiteSpace(variant.Id))
        {
            throw new InvalidOperationException("Problem Language Variant ID is required.");
        }

        if (string.IsNullOrWhiteSpace(variant.ProblemId))
        {
            throw new InvalidOperationException("Problem ID is required.");
        }

        if (string.IsNullOrWhiteSpace(variant.ProgrammingLanguageId))
        {
            throw new InvalidOperationException("Programming Language ID is required.");
        }

        if (string.IsNullOrWhiteSpace(variant.ProgrammingLanguageVersionTag))
        {
            throw new InvalidOperationException("Programming Language Version Tag is required.");
        }

        if (string.IsNullOrWhiteSpace(variant.ReferenceSolution))
        {
            throw new InvalidOperationException("Reference Solution is required.");
        }

        if (string.IsNullOrWhiteSpace(variant.StarterCode))
        {
            throw new InvalidOperationException("Starter Code is required.");
        }

        if (string.IsNullOrWhiteSpace(variant.TestHarnessCode))
        {
            throw new InvalidOperationException("Test Harness Code is required.");
        }

        return new ProblemLanguageVariantResponse(
            variant.Id,
            variant.ProblemId,
            variant.ProgrammingLanguageId,
            variant.ProgrammingLanguageVersionTag,
            variant.ReferenceSolution,
            variant.StarterCode,
            variant.TestHarnessCode,
            variant.WorkspaceFiles.Select(wf => wf.ToResponse()).ToList(),
            variant.IsActive,
            variant.BaselineMetrics?.ToResponse());
    }
}
