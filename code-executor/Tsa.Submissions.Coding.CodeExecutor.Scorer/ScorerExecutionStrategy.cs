using System.Diagnostics;
using Tsa.Submissions.Coding.CodeExecutor.Core.Strategies;
using Tsa.Submissions.Coding.Contracts.CodeExecutor;
using Tsa.Submissions.Coding.Contracts.Constants;

namespace Tsa.Submissions.Coding.CodeExecutor.Scorer;

public class ScorerExecutionStrategy : IExecutionStrategy<ScorerResult>
{
    private const string MissingSubmissionFileErrorMessage = "No submission source file found in workspace files.";
    public string StrategyName => nameof(ScorerExecutionStrategy);

    public ScorerResult Execute(RunnerJobPayload payload)
    {
        try
        {
            var workingDir = Path.Combine(Path.GetTempPath(), $"exec_{Guid.NewGuid():N}");
            Console.WriteLine($"Creating working directory - {workingDir}");
            Directory.CreateDirectory(workingDir);

            var solutionWorkspaceFile = payload.WorkspaceFiles.SingleOrDefault(file => file.Source == WorkspaceFileSources.Submission);

            if (solutionWorkspaceFile == null)
            {
                Console.WriteLine("ERROR: {0}", MissingSubmissionFileErrorMessage);
                return new ScorerResult(MissingSubmissionFileErrorMessage, string.Empty, string.Empty);
            }

            var solutionFilePath = Path.Combine(workingDir, solutionWorkspaceFile.Path);

            Console.WriteLine($"Writing submission source code to file - {solutionFilePath}");
            File.WriteAllText(solutionFilePath, payload.Solution);

            Console.WriteLine("Calculating code metrics");

            var processStartInfo = new ProcessStartInfo
            {
                FileName = "lizard",
                Arguments = $"-X {solutionFilePath}",
                WorkingDirectory = workingDir,
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

            if (process.ExitCode == 0)
            {
                var output = process.StandardOutput.ReadToEnd();
                Console.WriteLine("Code metrics calculation successful. Output:");
                Console.WriteLine(output);

                var lizardFileMeasures = LizardFileMeasureParser.ParseFromString(output);

                if (lizardFileMeasures.Count != 1)
                {
                    return new ScorerResult("Unexpected number of lizard file measures", string.Empty, output);
                }

                var lizardFileMeasure = lizardFileMeasures[0];

                if(lizardFileMeasure.MetricsByLabel == null)
                {
                    return new ScorerResult("Lizard file measure metrics by label is null", string.Empty, output);
                }

                var cyclomaticComplexity = Convert.ToInt32(lizardFileMeasure.MetricsByLabel["CCN"]);
                var linesOfCode = Convert.ToInt32(lizardFileMeasure.MetricsByLabel["NCSS"]);

                return new ScorerResult(output, cyclomaticComplexity, linesOfCode);
            }

            var errorMessage = "Failed to calculate code metrics";

            var errorOutput = process.StandardError.ReadToEnd();

            Console.WriteLine(errorMessage);
            Console.WriteLine(errorOutput);

            return new ScorerResult(errorMessage, errorOutput, string.Empty);
        }
        catch (Exception exception)
        {
            Console.WriteLine($"Exception occurred: {exception.Message}");
            return ScorerResult.FromException(exception);
        }
    }
}
