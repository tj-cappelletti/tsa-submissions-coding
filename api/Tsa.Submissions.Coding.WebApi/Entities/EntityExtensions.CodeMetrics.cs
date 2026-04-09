using Tsa.Submissions.Coding.Contracts.ProblemLanguageVariants;

namespace Tsa.Submissions.Coding.WebApi.Entities;

public static partial class EntityExtensions
{
    /// <summary>
    ///     Converts a <see cref="CodeMetrics" /> entity to a <see cref="CodeMetricsResponse" />.
    /// </summary>
    /// <param name="codeMetrics">The code metrics entity to convert</param>
    /// <returns>A <see cref="CodeMetricsResponse" /> representing the code metrics</returns>
    public static CodeMetricsResponse ToResponse(this CodeMetrics codeMetrics)
    {
        return new CodeMetricsResponse(
            codeMetrics.CyclomaticComplexity,
            codeMetrics.ExecutionTimeInMs,
            codeMetrics.LinesOfCode);
    }
}
