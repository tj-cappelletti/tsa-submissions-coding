using System.Diagnostics;

namespace Tsa.Submissions.Coding.CodeExecutor.Runner.Executors;

/// <summary>
/// Executor for .NET languages (C#, F#, VB)
/// </summary>
public class DotNetExecutor : ILanguageExecutor
{
    private readonly string _languageExtension;

    /// <summary>
    /// Initializes a new instance of the <see cref="DotNetExecutor"/> class
    /// </summary>
    /// <param name="language">Language identifier: csharp, fsharp, or vb</param>
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

    /// <inheritdoc/>
    public async Task PrepareAsync(ExecutionContext context, CancellationToken cancellationToken = default)
    {
        var sourceFilePath = Path.Combine(context.WorkingDirectory, $"Solution.{_languageExtension}");
        File.WriteAllText(sourceFilePath, context.SourceCode);

        // Create a simple project file
        var projectContent = $@"<Project Sdk=""Microsoft.NET.Sdk"">
  <PropertyGroup>
    <OutputType>Exe</OutputType>
    <TargetFramework>net9.0</TargetFramework>
    <Nullable>enable</Nullable>
  </PropertyGroup>
</Project>";
        
        var projectPath = Path.Combine(context.WorkingDirectory, "Solution.csproj");
        File.WriteAllText(projectPath, projectContent);

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
            throw new InvalidOperationException("Failed to start dotnet build process");

        await process.WaitForExitAsync(cancellationToken);
        
        if (process.ExitCode != 0)
        {
            var error = await process.StandardError.ReadToEndAsync(cancellationToken);
            throw new InvalidOperationException($"Compilation failed: {error}");
        }

        context.ExecutablePath = Path.Combine(context.WorkingDirectory, "bin", "Release", "net9.0", "Solution.dll");
    }

    /// <inheritdoc/>
    public async Task<(string stdout, string stderr, int exitCode)> ExecuteAsync(
        ExecutionContext context,
        string input,
        int timeoutMs,
        CancellationToken cancellationToken = default)
    {
        var psi = new ProcessStartInfo
        {
            FileName = "dotnet",
            Arguments = context.ExecutablePath,
            WorkingDirectory = context.WorkingDirectory,
            RedirectStandardInput = true,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };

        using var process = new Process { StartInfo = psi };
        var stdout = new System.Text.StringBuilder();
        var stderr = new System.Text.StringBuilder();

        process.OutputDataReceived += (sender, e) =>
        {
            if (e.Data != null) stdout.AppendLine(e.Data);
        };

        process.ErrorDataReceived += (sender, e) =>
        {
            if (e.Data != null) stderr.AppendLine(e.Data);
        };

        process.Start();
        process.BeginOutputReadLine();
        process.BeginErrorReadLine();

        if (!string.IsNullOrEmpty(input))
        {
            await process.StandardInput.WriteAsync(input);
        }
        process.StandardInput.Close();

        var completed = await process.WaitForExitAsync(TimeSpan.FromMilliseconds(timeoutMs), cancellationToken);

        if (!completed)
        {
            try
            {
                process.Kill(true);
            }
            catch
            {
                // Process may have already exited
            }
            return (string.Empty, "Execution timed out", -1);
        }

        return (stdout.ToString(), stderr.ToString(), process.ExitCode);
    }
}
