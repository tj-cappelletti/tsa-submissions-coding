namespace Tsa.Submissions.Coding.Contracts.Constants;

/// <summary>
///     Constants for workspace file types used in problem language variants.
/// </summary>
public static class WorkspaceFileTypes
{
    /// <summary>
    ///     A file with inline content that is written directly to disk.
    /// </summary>
    public const string File = "File";

    /// <summary>
    ///     A file whose content is resolved from a named source on the execution context.
    /// </summary>
    public const string Link = "Link";

    /// <summary>
    ///     All supported workspace file types.
    /// </summary>
    public static readonly string[] SupportedTypes =
    [
        File,
        Link
    ];
}
