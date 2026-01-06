using System;
using System.Text.RegularExpressions;

namespace Tsa.Submissions.Coding.WebApi.ExtensionMethods;

public static class StringExtensions
{
    /// <summary>
    ///     Sanitizes a string for safe logging by removing sensitive information.
    /// </summary>
    /// <param name="input">The input string to sanitize.</param>
    /// <returns>A sanitized version of the string.</returns>
    public static string SanitizeForLogging(this string? input)
    {
        if (string.IsNullOrWhiteSpace(input))
        {
            return string.Empty;
        }

        // Example: Remove sensitive patterns like passwords or tokens
        var sanitized = Regex.Replace(input, @"(?i)(password|token)\s*=\s*[^&\s]+", "REDACTED");

        // Trim leading and trailing whitespace
        sanitized = sanitized.Trim();

        // Optionally, truncate long strings to avoid excessive log size
        const int maxLength = 1000;
        if (sanitized.Length > maxLength)
        {
            sanitized = string.Concat(sanitized.AsSpan(0, maxLength), "... [TRUNCATED]");
        }

        return sanitized;
    }
}
