namespace Tsa.Submissions.Coding.Contracts.Constants;

/// <summary>
///     Constants for supported programming languages
/// </summary>
public static class LanguageConstants
{
    /// <summary>
    ///     C language identifier
    /// </summary>
    public const string C = "C";

    /// <summary>
    ///     C++ language identifier
    /// </summary>
    public const string Cpp = "C++";

    /// <summary>
    ///     C# language identifier
    /// </summary>
    public const string CSharp = "C#";

    /// <summary>
    ///     F# language identifier
    /// </summary>
    public const string FSharp = "F#";

    /// <summary>
    ///     Go language identifier
    /// </summary>
    public const string Go = "Go";

    /// <summary>
    ///     Java language identifier
    /// </summary>
    public const string Java = "Java";

    /// <summary>
    ///     Node.js language identifier
    /// </summary>
    public const string NodeJs = "Node.js";

    /// <summary>
    ///     Python language identifier
    /// </summary>
    public const string Python = "Python";

    /// <summary>
    ///     Ruby language identifier
    /// </summary>
    public const string Ruby = "Ruby";

    /// <summary>
    ///     Visual Basic language identifier
    /// </summary>
    public const string VisualBasic = "VisualBasic";

    /// <summary>
    ///     All supported languages
    /// </summary>
    public static readonly string[] SupportedLanguages =
    [
        Python,
        Java,
        CSharp,
        FSharp,
        VisualBasic,
        Cpp,
        C,
        Go,
        NodeJs,
        Ruby
    ];
}
