namespace Tsa.Submissions.Coding.CodeExecutor.UnitTests.Executors;

using Runner.Executors;

/// <summary>
/// Unit tests for PythonExecutor
/// </summary>
public class PythonExecutorTests
{
    [Fact]
    [Trait("TestCategory", "UnitTest")]
    public async Task PrepareAsync_WithValidCode_ShouldCreateScriptFile()
    {
        // Arrange
        var executor = new PythonExecutor();
        var workingDir = Path.Combine(Path.GetTempPath(), $"test_{Guid.NewGuid():N}");
        Directory.CreateDirectory(workingDir);

        try
        {
            var context = new ExecutionContext
            {
                SourceCode = "print('Hello, World!')",
                WorkingDirectory = workingDir,
                Language = "Python"
            };

            // Act
            await executor.PrepareAsync(context);

            // Assert
            Assert.NotNull(context.ExecutablePath);
            Assert.True(File.Exists(context.ExecutablePath));
            var content = File.ReadAllText(context.ExecutablePath);
            Assert.Equal("print('Hello, World!')", content);
        }
        finally
        {
            Directory.Delete(workingDir, true);
        }
    }

    [Fact]
    [Trait("TestCategory", "UnitTest")]
    public async Task ExecuteAsync_WithValidCode_ShouldReturnCorrectOutput()
    {
        // Arrange
        var executor = new PythonExecutor();
        var workingDir = Path.Combine(Path.GetTempPath(), $"test_{Guid.NewGuid():N}");
        Directory.CreateDirectory(workingDir);

        try
        {
            var context = new ExecutionContext
            {
                SourceCode = "print('Hello, World!')",
                WorkingDirectory = workingDir,
                Language = "Python"
            };

            await executor.PrepareAsync(context);

            // Act
            (string stdout, string stderr, int exitCode) = await executor.ExecuteAsync(context, "", 5000);

            // Assert
            Assert.Equal(0, exitCode);
            Assert.Contains("Hello, World!", stdout);
            Assert.Empty(stderr.Trim());
        }
        finally
        {
            Directory.Delete(workingDir, true);
        }
    }

    [Fact]
    [Trait("TestCategory", "UnitTest")]
    public async Task ExecuteAsync_WithInput_ShouldProcessInput()
    {
        // Arrange
        var executor = new PythonExecutor();
        var workingDir = Path.Combine(Path.GetTempPath(), $"test_{Guid.NewGuid():N}");
        Directory.CreateDirectory(workingDir);

        try
        {
            var context = new ExecutionContext
            {
                SourceCode = "name = input()\nprint(f'Hello, {name}!')",
                WorkingDirectory = workingDir,
                Language = "Python"
            };

            await executor.PrepareAsync(context);

            // Act
            (string stdout, string stderr, int exitCode) = await executor.ExecuteAsync(context, "Alice\n", 5000);

            // Assert
            Assert.Equal(0, exitCode);
            Assert.Contains("Hello, Alice!", stdout);
        }
        finally
        {
            Directory.Delete(workingDir, true);
        }
    }

    [Fact]
    [Trait("TestCategory", "UnitTest")]
    public async Task ExecuteAsync_WithRuntimeError_ShouldReturnNonZeroExitCode()
    {
        // Arrange
        var executor = new PythonExecutor();
        var workingDir = Path.Combine(Path.GetTempPath(), $"test_{Guid.NewGuid():N}");
        Directory.CreateDirectory(workingDir);

        try
        {
            var context = new ExecutionContext
            {
                SourceCode = "raise Exception('Test error')",
                WorkingDirectory = workingDir,
                Language = "Python"
            };

            await executor.PrepareAsync(context);

            // Act
            (string stdout, string stderr, int exitCode) = await executor.ExecuteAsync(context, "", 5000);

            // Assert
            Assert.NotEqual(0, exitCode);
            Assert.Contains("Exception", stderr);
        }
        finally
        {
            Directory.Delete(workingDir, true);
        }
    }
}
