using System.Diagnostics;

namespace Tsa.Submissions.Coding.CodeExecutor.Runner.Executors;

/// <summary>
/// Executor for Python code
/// </summary>
public class PythonExecutor : ILanguageExecutor
{
    /// <inheritdoc/>
    public Task PrepareAsync(ExecutionContext context, CancellationToken cancellationToken = default)
    {
        // Python is interpreted, just save the source code to a file
        var scriptPath = Path.Combine(context.WorkingDirectory, "solution.py");
        File.WriteAllText(scriptPath, context.SourceCode);
        context.ExecutablePath = scriptPath;
        return Task.CompletedTask;
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
            FileName = "python3",
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

        // Write input to stdin
        if (!string.IsNullOrEmpty(input))
        {
            await process.StandardInput.WriteAsync(input);
        }
        process.StandardInput.Close();

        // Wait for exit with timeout
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

/// <summary>
/// Extension methods for Process class
/// </summary>
internal static class ProcessExtensions
{
    /// <summary>
    /// Waits for the process to exit with a timeout
    /// </summary>
    public static async Task<bool> WaitForExitAsync(this Process process, TimeSpan timeout, CancellationToken cancellationToken = default)
    {
        using var cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        cts.CancelAfter(timeout);

        try
        {
            await process.WaitForExitAsync(cts.Token);
            return true;
        }
        catch (OperationCanceledException)
        {
            return false;
        }
    }
}
