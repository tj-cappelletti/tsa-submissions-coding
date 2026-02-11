using System.Diagnostics;
using System.Text;
using System.Text.RegularExpressions;
using Tsa.Submissions.Coding.Contracts.TestCases;

namespace Tsa.Submissions.Coding.CodeExecutor.Runner.Executors;

public class DotNetExecutor : ILanguageExecutor
{
    private readonly string _languageExtension;

    public DotNetExecutor(string language)
    {
        _languageExtension = language switch
        {
            "csharp" => "cs",
            "fsharp" => "fs",
            "vb" => "vb",
            _ => throw new ArgumentException($"Unsupported .NET language: {language}", nameof(language))
        };
    }

    private static void CreateProjectFile(CodeExecutionContext context)
    {
        var projectFileContents = $"""
                                   <Project Sdk="Microsoft.NET.Sdk">
                                     <PropertyGroup>
                                       <OutputType>Exe</OutputType>
                                       <TargetFramework>net{context.LanguageVersion}</TargetFramework>
                                       <Nullable>enable</Nullable>
                                     </PropertyGroup>
                                     <ItemGroup>
                                       <PackageReference Include="coverlet.collector" Version="6.0.4" />
                                       <PackageReference Include="Microsoft.NET.Test.Sdk" Version="17.14.1" />
                                       <PackageReference Include="xunit" Version="2.9.3" />
                                       <PackageReference Include="xunit.runner.visualstudio" Version="3.1.4" />
                                     </ItemGroup>
                                     <ItemGroup>
                                       <Using Include="Xunit" />
                                     </ItemGroup>
                                   </Project>
                                   """;

        var projectFilePath = Path.Combine(context.WorkingDirectory, "Solution.csproj");

        File.WriteAllTextAsync(projectFilePath, projectFileContents);
    }

    public ExecutorResult ExecuteTests(CodeExecutionContext context, TimeSpan timeout)
    {
        try
        {
            // Run dotnet test
            var processStartInfo = new ProcessStartInfo
            {
                FileName = "dotnet",
                Arguments = "test -c Release --logger trx --no-build",
                WorkingDirectory = context.WorkingDirectory,
                RedirectStandardInput = true,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            using var process = Process.Start(processStartInfo);
            if (process == null)
            {
                throw new InvalidOperationException("Failed to start dotnet build process");
            }

            process.WaitForExit();

            string? errorMessage = null;

            if (!TestResultsFileExists(context.WorkingDirectory))
            {
                errorMessage = "Test execution failed";
            }

            // Zero exit code indicates success; One exit code indicates test failures, but execution was successful
            return ExecutorResult.FromProcess(process, errorMessage, [1]);
        }
        catch (Exception exception)
        {
            return ExecutorResult.FromException(exception);
        }
    }

    private static string FormatDataType(string dataType, string rawValue)
    {
        return dataType.ToLower() switch
        {
            "string" => $"\"{rawValue.Replace("\"", "\\\"")}\"",
            "char" => $"'{rawValue}'",
            "bool" => rawValue.ToLower(),
            _ => rawValue
        };
    }

    /// <summary>
    ///     Converts an input string into a valid C# identifier by replacing or removing invalid characters.
    /// </summary>
    /// <param name="input">The input string to sanitize.</param>
    /// <returns>A string that is safe to use as a C# identifier (e.g., method name).</returns>
    /// <remarks>
    ///     This method ensures the result:
    ///     <list type="bullet">
    ///         <item>Starts with a letter or underscore</item>
    ///         <item>Contains only letters, digits, and underscores</item>
    ///         <item>Does not conflict with C# keywords (prefixed with @ if necessary)</item>
    ///     </list>
    /// </remarks>
    private static string GetSourceCodeSafeValue(string input)
    {
        if (string.IsNullOrWhiteSpace(input))
        {
            return "_";
        }

        var sb = new StringBuilder();

        // Process first character - must be letter or underscore
        var firstChar = input[0];
        if (char.IsLetter(firstChar) || firstChar == '_')
        {
            sb.Append(firstChar);
        }
        else if (char.IsDigit(firstChar))
        {
            sb.Append('_').Append(firstChar); // Prefix with underscore
        }
        else
        {
            sb.Append('_'); // Replace invalid character with underscore
        }

        // Process remaining characters
        for (var i = 1; i < input.Length; i++)
        {
            var c = input[i];
            if (char.IsLetterOrDigit(c) || c == '_')
            {
                sb.Append(c);
            }
            else if (char.IsWhiteSpace(c))
            {
                sb.Append('_'); // Replace whitespace with underscore
            }
            else
            {
                // Replace special characters with underscore
                sb.Append('_');
            }
        }

        var result = sb.ToString();

        // Avoid C# keywords by prefixing with @
        if (IsCSharpKeyword(result))
        {
            return $"@{result}";
        }

        return result;
    }

    public List<TestCaseResult> GetTestCaseResults(CodeExecutionContext context)
    {
        var testCaseResults = new List<TestCaseResult>();

        if (!TestResultsFileExists(context.WorkingDirectory))
        {
            throw new InvalidOperationException("Attempting to read test results when none or multiple exist.");
        }

        var testResultsFile = Directory
            .GetFiles(context.WorkingDirectory, "*.trx", SearchOption.AllDirectories)
            .First();

        var stream = File.OpenRead(testResultsFile);

        var unitTestResults = TrxUnitTestResultReader.GetUnitTestResults(stream);

        if (unitTestResults == null)
        {
            // TODO: Figure out a better exception and error handling
            throw new Exception("Unable to parse the TRX file.");
        }

        foreach (var testCase in context.TestCases)
        {
            var testMethodIdentifier = GetSourceCodeSafeValue(testCase.Name);

            var unitTestResult = unitTestResults.Single(unitTestResult =>
                unitTestResult.TestName != null && unitTestResult.TestName.Contains(testMethodIdentifier));

            var passed = unitTestResult.Outcome?.ToLower() == "passed";

            //TODO: Capture actual results
            var actualOutput = unitTestResult.ErrorMessage == null
                ? testCase.ExpectedOutput
                : Regex.Match(unitTestResult.ErrorMessage, "Actual:[\\s]+(.+)$").Groups[1].Value;

            testCaseResults.Add(new TestCaseResult(testCase.Signature, actualOutput, unitTestResult.ErrorMessage, passed, false,
                unitTestResult.DurationValue));
        }

        return testCaseResults;
    }

    /// <summary>
    ///     Checks if a string is a C# keyword.
    /// </summary>
    private static bool IsCSharpKeyword(string identifier)
    {
        return identifier switch
        {
            "abstract" or "as" or "base" or "bool" or "break" or "byte" or "case" or
                "catch" or "char" or "checked" or "class" or "const" or "continue" or
                "decimal" or "default" or "delegate" or "do" or "double" or "else" or
                "enum" or "event" or "explicit" or "extern" or "false" or "finally" or
                "fixed" or "float" or "for" or "foreach" or "goto" or "if" or "implicit" or
                "in" or "int" or "interface" or "internal" or "is" or "lock" or "long" or
                "namespace" or "new" or "null" or "object" or "operator" or "out" or
                "override" or "params" or "private" or "protected" or "public" or "readonly" or
                "ref" or "return" or "sbyte" or "sealed" or "short" or "sizeof" or "stackalloc" or
                "static" or "string" or "struct" or "switch" or "this" or "throw" or "true" or
                "try" or "typeof" or "uint" or "ulong" or "unchecked" or "unsafe" or "ushort" or
                "using" or "virtual" or "void" or "volatile" or "while" => true,
            _ => false
        };
    }

    public ExecutorResult Prepare(CodeExecutionContext context)
    {
        try
        {
            CreateProjectFile(context);
            WriteSolutionToDisk(context, _languageExtension);
            WriteTestFixtureToDisk(context, _languageExtension);

            // Build the project
            var processStartInfo = new ProcessStartInfo
            {
                FileName = "dotnet",
                Arguments = "build -c Release --nologo",
                WorkingDirectory = context.WorkingDirectory,
                RedirectStandardInput = true,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            using var process = Process.Start(processStartInfo);
            if (process == null)
            {
                throw new InvalidOperationException("Failed to start dotnet build process");
            }

            process.WaitForExit();

            var errorMessage = process.ExitCode == 0 ? null : "Build failed";

            return ExecutorResult.FromProcess(process, errorMessage);
        }
        catch (Exception exception)
        {
            return ExecutorResult.FromException(exception);
        }
    }

    private static bool TestResultsFileExists(string workingDirectory)
    {
        return Directory.GetFiles(workingDirectory, "*.trx", SearchOption.AllDirectories).Length == 1;
    }

    private static void WriteSolutionToDisk(CodeExecutionContext context, string languageExtension)
    {
        var sourceFilePath = Path.Combine(context.WorkingDirectory, $"Solution.{languageExtension}");

        File.WriteAllText(sourceFilePath, context.SourceCode);
    }

    private static void WriteTestFixtureToDisk(CodeExecutionContext context, string languageExtension)
    {
        var testCaseMethods = new StringBuilder();

        foreach (var testCase in context.TestCases)
        {
            var inputs = new List<string>();

            foreach (var testCaseInput in testCase.Inputs)
            {
                if (testCaseInput.IsArray)
                {
                    throw new NotImplementedException("Array inputs are not supported yet.");
                }

                inputs.Add(FormatDataType(testCaseInput.DataType, testCaseInput.Value));
            }

            var output = FormatDataType(testCase.OutputDataType, testCase.ExpectedOutput);

            testCaseMethods.AppendLine("    [Theory]");
            testCaseMethods.AppendLine($"    [InlineData({string.Join(", ", inputs)}, {output})]");
            testCaseMethods.AppendLine(context.LanguageFixture.Replace("###TestCaseName###", GetSourceCodeSafeValue(testCase.Name)));
        }

        var testFixtureFilePath = Path.Combine(context.WorkingDirectory, $"TestFixture.{languageExtension}");

        var testFixtureSourceFileContents = $$"""
                                              using Xunit;

                                              namespace TsaCoding;

                                              public class TestFixture
                                              {
                                                  {{testCaseMethods}}
                                              }

                                              """;

        File.WriteAllText(testFixtureFilePath, testFixtureSourceFileContents);
    }
}
