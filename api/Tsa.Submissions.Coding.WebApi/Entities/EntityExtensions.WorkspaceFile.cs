using System;
using System.Collections.Generic;
using System.Linq;
using Tsa.Submissions.Coding.Contracts.ProblemLanguageVariants;

namespace Tsa.Submissions.Coding.WebApi.Entities;

public static partial class EntityExtensions
{
    /// <summary>
    ///     Converts a <see cref="WorkspaceFile" /> entity to a <see cref="WorkspaceFileResponse" />.
    /// </summary>
    /// <param name="workspaceFile">The workspace file entity to convert</param>
    /// <returns>A <see cref="WorkspaceFileResponse" /> representing the workspace file</returns>
    /// <exception cref="InvalidOperationException">
    ///     Thrown when required fields are missing
    /// </exception>
    public static WorkspaceFileResponse ToResponse(this WorkspaceFile workspaceFile)
    {
        if (string.IsNullOrWhiteSpace(workspaceFile.Type))
        {
            throw new InvalidOperationException("Workspace File Type is required.");
        }

        if (string.IsNullOrWhiteSpace(workspaceFile.Path))
        {
            throw new InvalidOperationException("Workspace File Path is required.");
        }

        return new WorkspaceFileResponse(
            workspaceFile.Type,
            workspaceFile.Path,
            workspaceFile.Contents,
            workspaceFile.Source,
            workspaceFile.IsTemplate);
    }

    /// <summary>
    ///     Converts a collection of <see cref="WorkspaceFile" /> entities to
    ///     <see cref="WorkspaceFileResponse" /> objects.
    /// </summary>
    /// <param name="workspaceFiles">The collection of workspace file entities to convert</param>
    /// <returns>A collection of <see cref="WorkspaceFileResponse" /> objects</returns>
    /// <exception cref="InvalidOperationException">
    ///     Thrown when required fields are missing from any workspace file
    /// </exception>
    public static IEnumerable<WorkspaceFileResponse> ToResponses(this IEnumerable<WorkspaceFile> workspaceFiles)
    {
        return workspaceFiles.Select(workspaceFile => workspaceFile.ToResponse());
    }
}
