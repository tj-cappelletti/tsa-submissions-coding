using Tsa.Submissions.Coding.Contracts.TestCases;

namespace Tsa.Submissions.Coding.CodeExecutor.Runner.Executors;

/// <summary>
///     Interface for language-specific code execution
/// </summary>
public interface ILanguageExecutor
{
    /// <summary>
    ///     Executes the code with the provided test cases
    /// </summary>
    /// <param name="context">The execution context containing source code, test cases, and working directory</param>
    /// <param name="timeout">Maximum time allowed for test execution before timing out</param>
    /// <returns>An <see cref="ExecutorResult" /> indicating success or failure of the test execution process</returns>
    ExecutorResult ExecuteTests(CodeExecutionContext context, TimeSpan timeout);

    /// <summary>
    ///     Retrieves the results of executed test cases in a standardized format
    /// </summary>
    /// <param name="context">The execution context containing source code, test cases, and working directory</param>
    /// <remarks>
    ///     Since different languages may have different ways of reporting test results,
    ///     this method abstracts the retrieval process and returns results in a standardized format.
    ///     This method should be called after <see cref="ExecuteTests" /> has completed successfully.
    /// </remarks>
    /// <returns>A list of <see cref="TestCaseResult" /> containing the outcome of each test case</returns>
    List<TestCaseResult> GetTestCaseResults(CodeExecutionContext context);

    /// <summary>
    ///     Prepares the execution environment and source code for testing
    /// </summary>
    /// <remarks>
    ///     The source code and test fixtures are written to the working directory specified in the context.
    ///     Language-specific preparation steps are performed, which may include:
    ///     <list type="bullet">
    ///         <item>Creating project/configuration files</item>
    ///         <item>Compiling source code</item>
    ///         <item>Installing dependencies</item>
    ///         <item>Setting up the runtime environment</item>
    ///     </list>
    /// </remarks>
    /// <param name="context">The execution context containing source code, test fixtures, and working directory</param>
    /// <returns>An <see cref="ExecutorResult" /> indicating success or failure of the preparation process</returns>
    ExecutorResult Prepare(CodeExecutionContext context);
}
