using Tsa.Submissions.Coding.CodeExecutor.Shared.Constants;

namespace Tsa.Submissions.Coding.CodeExecutor.Runner.Executors;

/// <summary>
/// Factory for creating language-specific executors
/// </summary>
public static class LanguageExecutorFactory
{
    /// <summary>
    /// Creates an executor for the specified language
    /// </summary>
    /// <param name="language">The programming language</param>
    /// <returns>The language executor</returns>
    /// <exception cref="NotSupportedException">Thrown when language is not supported</exception>
    public static ILanguageExecutor CreateExecutor(string language)
    {
        return language switch
        {
            LanguageConstants.Python => new PythonExecutor(),
            LanguageConstants.Java => new JavaExecutor(),
            LanguageConstants.CSharp => new DotNetExecutor("csharp"),
            LanguageConstants.FSharp => new DotNetExecutor("fsharp"),
            LanguageConstants.VisualBasic => new DotNetExecutor("vb"),
            LanguageConstants.Cpp => new CppExecutor(),
            LanguageConstants.C => new CExecutor(),
            LanguageConstants.Go => new GoExecutor(),
            LanguageConstants.NodeJs => new NodeJsExecutor(),
            LanguageConstants.Ruby => new RubyExecutor(),
            _ => throw new NotSupportedException($"Language '{language}' is not supported")
        };
    }
}
