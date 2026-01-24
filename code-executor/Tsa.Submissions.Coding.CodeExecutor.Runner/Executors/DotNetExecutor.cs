using System.Diagnostics;

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

    public (string stdout, string stderr, int exitCode) Execute(
        CodeExecutionContext context,
        string input,
        TimeSpan timeout)
    {
        throw new NotImplementedException();
    }

    public void Prepare(CodeExecutionContext context)
    {
        var sourceFilePath = Path.Combine(context.WorkingDirectory, $"Solution.{_languageExtension}");
        File.WriteAllText(sourceFilePath, context.SourceCode);

        var testFixtures = new List<string>(context.TestCases.Count);

        foreach (var testCase in context.TestCases)
        {
            var testFixture = testCase.LanguageFixtures.SingleOrDefault(f => f.Language == context.Language);

            if (testFixture == null) continue;

            testFixtures.Add(testFixture.Fixture);
        }

        // TODO: Handle F# and VB.NET specifics
        var testFixtureSourceFileContents = $@"using Xunit;

namespace TsaCoding;

public class TestFixture
{{
    {string.Join("\n", testFixtures)}
}}
";

        // Create a simple project file
        var projectFileContents = $@"<Project Sdk=""Microsoft.NET.Sdk"">
  <PropertyGroup>
    <OutputType>Exe</OutputType>
    <TargetFramework>net{context.LanguageVersion}</TargetFramework>
    <Nullable>enable</Nullable>
  </PropertyGroup>
  <ItemGroup>
    <PackageReference Include=""coverlet.collector"" Version=""6.0.4"" />
    <PackageReference Include=""Microsoft.NET.Test.Sdk"" Version=""17.14.1"" />
    <PackageReference Include=""xunit"" Version=""2.9.3"" />
    <PackageReference Include=""xunit.runner.visualstudio"" Version=""3.1.4"" />
  </ItemGroup>
  <ItemGroup>
    <Using Include=""Xunit"" />
  </ItemGroup>
</Project>";

        var testFixtureFilePath = Path.Combine(context.WorkingDirectory, $"TestFixture.{_languageExtension}");
        File.WriteAllText(testFixtureFilePath, testFixtureSourceFileContents);

        var projectFilePath = Path.Combine(context.WorkingDirectory, "Solution.csproj");
        File.WriteAllTextAsync(projectFilePath, projectFileContents);

        // Build the project
        var psi = new ProcessStartInfo
        {
            FileName = "dotnet",
            Arguments = "build -c Release --nologo",
            WorkingDirectory = context.WorkingDirectory,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };

        using var process = Process.Start(psi);
        if (process == null)
        {
            throw new InvalidOperationException("Failed to start dotnet build process");
        }

        process.WaitForExit();

        if (process.ExitCode == 0) return;

        var error = process.StandardError.ReadToEnd();
        throw new InvalidOperationException($"Compilation failed: {error}");
    }
}
