namespace Tsa.Submissions.Coding.Contracts.Constants;

/// <summary>
///     Constants for workspace file link sources used in problem language variants.
///     These identify which property on the execution context provides the file content.
/// </summary>
public static class WorkspaceFileSources
{
    /// <summary>
    ///     The reference solution authored by the problem creator.
    /// </summary>
    public const string ReferenceSolution = "referenceSolution";

    /// <summary>
    ///     The starter code provided to participants.
    /// </summary>
    public const string StarterCode = "starterCode";

    /// <summary>
    ///     The participant's submitted solution.
    /// </summary>
    public const string Submission = "submission";

    /// <summary>
    ///     The test harness code authored by the problem creator.
    /// </summary>
    public const string TestHarnessCode = "testHarnessCode";

    /// <summary>
    ///     All supported link sources.
    /// </summary>
    public static readonly string[] SupportedSources =
    [
        ReferenceSolution,
        StarterCode,
        Submission,
        TestHarnessCode
    ];
}
