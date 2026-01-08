using System.Diagnostics;

namespace Tsa.Submissions.Coding.CodeExecutor.Runner.Executors;

/// <summary>
/// Executor for C code
/// </summary>
public class CExecutor : ILanguageExecutor
{
    /// <inheritdoc/>
    public async Task PrepareAsync(ExecutionContext context, CancellationToken cancellationToken = default)
    {
        var sourceFilePath = Path.Combine(context.WorkingDirectory, "solution.c");
        var executablePath = Path.Combine(context.WorkingDirectory, "solution");
        
        File.WriteAllText(sourceFilePath, context.SourceCode);

        // Compile C code
        var psi = new ProcessStartInfo
        {
            FileName = "gcc",
            Arguments = $"-std=c11 -o {executablePath} {sourceFilePath}",
            WorkingDirectory = context.WorkingDirectory,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };

        using var process = Process.Start(psi);
        if (process == null)
            throw new InvalidOperationException("Failed to start gcc process");

        await process.WaitForExitAsync(cancellationToken);
        
        if (process.ExitCode != 0)
        {
            var error = await process.StandardError.ReadToEndAsync(cancellationToken);
            throw new InvalidOperationException($"Compilation failed: {error}");
        }

        context.ExecutablePath = executablePath;
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
            FileName = context.ExecutablePath,
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
