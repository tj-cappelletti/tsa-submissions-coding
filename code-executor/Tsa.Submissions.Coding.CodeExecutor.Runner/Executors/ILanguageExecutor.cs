using Tsa.Submissions.Coding.Contracts.TestCases;

namespace Tsa.Submissions.Coding.CodeExecutor.Runner.Executors;

/// <summary>
///     Interface for language-specific code execution
/// </summary>
public interface ILanguageExecutor
{
    /// <summary>
    ///     Executes the language-specific build process for the provided source code
    /// </summary>
    /// <remarks>
    ///     Not all languages require a build step, but for those that do, this method should handle the compilation or
    ///     preparation of the source code before test execution.
    ///     For languages that do not require a build step, this method can simply return a successful result without
    ///     performing any actions.
    ///     Alternatively, it can be used to perform any necessary setup or validation of the source code before executing
    ///     tests.
    ///     This method should be called before <see cref="ExecuteTests" /> to ensure that the code is ready for execution.
    /// </remarks>
    /// <param name="context">The execution context containing source code, test cases, and working directory</param>
    /// <param name="timeout">Maximum time allowed for test execution before timing out</param>
    /// <returns>An <see cref="ExecutorResult" /> indicating success or failure of the test execution process</returns>
    ExecutorResult ExecuteBuild(CodeExecutionContext context, TimeSpan timeout);

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
    /// <returns>A list of <see cref="TestCaseResultRequest" /> containing the outcome of each test case</returns>
    List<TestCaseResultRequest> GetTestCaseResults(CodeExecutionContext context);

    /// <summary>
    ///     Prepares the execution environment and source code for testing
    /// </summary>
    /// <remarks>
    ///     The source code and test fixtures are written to the working directory specified in the context.
    ///     Language-specific preparation steps are performed, which may include:
    ///     <list type="bullet">
    ///         <item>Creating project/configuration files</item>
    ///         <item>Setting up the runtime environment</item>
    ///     </list>
    ///     Any build steps should be handled in the <see cref="ExecuteBuild" /> method, while this method focuses on preparing
    ///     the code and environment for execution.
    /// </remarks>
    /// <param name="context">The execution context containing source code, test fixtures, and working directory</param>
    /// <returns>An <see cref="ExecutorResult" /> indicating success or failure of the preparation process</returns>
    ExecutorResult Prepare(CodeExecutionContext context);
}
