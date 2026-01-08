using System.Diagnostics;

namespace Tsa.Submissions.Coding.CodeExecutor.Runner.Executors;

/// <summary>
/// Executor for Java code
/// </summary>
public class JavaExecutor : ILanguageExecutor
{
    /// <inheritdoc/>
    public async Task PrepareAsync(ExecutionContext context, CancellationToken cancellationToken = default)
    {
        // Extract class name from source code
        var className = ExtractClassName(context.SourceCode);
        var javaFilePath = Path.Combine(context.WorkingDirectory, $"{className}.java");
        
        File.WriteAllText(javaFilePath, context.SourceCode);

        // Compile Java code
        var psi = new ProcessStartInfo
        {
            FileName = "javac",
            Arguments = javaFilePath,
            WorkingDirectory = context.WorkingDirectory,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };

        using var process = Process.Start(psi);
        if (process == null)
            throw new InvalidOperationException("Failed to start javac process");

        await process.WaitForExitAsync(cancellationToken);
        
        if (process.ExitCode != 0)
        {
            var error = await process.StandardError.ReadToEndAsync(cancellationToken);
            throw new InvalidOperationException($"Compilation failed: {error}");
        }

        context.ExecutablePath = className;
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
            FileName = "java",
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

    private static string ExtractClassName(string sourceCode)
    {
        // Simple regex to extract class name
        var match = System.Text.RegularExpressions.Regex.Match(sourceCode, @"public\s+class\s+(\w+)");
        if (match.Success)
        {
            return match.Groups[1].Value;
        }

        // Fallback to Main if no public class found
        return "Solution";
    }
}
