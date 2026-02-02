using System.Globalization;
using System.Xml.Linq;

namespace Tsa.Submissions.Coding.CodeExecutor.Runner.Executors;

public static class TrxUnitTestResultReader
{
    private static readonly XNamespace TrxNamespace = "http://microsoft.com/schemas/VisualStudio/TeamTest/2010";

    public static IReadOnlyList<UnitTestResult> GetUnitTestResults(Stream trxStream)
    {
        if (trxStream is null) throw new ArgumentNullException(nameof(trxStream));

        var doc = XDocument.Load(trxStream);

        var ns = doc.Root?.Name.Namespace
                 ?? TrxNamespace;

        var resultsElement = doc.Root?.Element(ns + "Results");
        if (resultsElement is null)
        {
            return Array.Empty<UnitTestResult>();
        }

        var unitTestResultElements = resultsElement.Descendants(ns + "UnitTestResult");

        return unitTestResultElements.Select(MapUnitTestResult).ToList();
    }

    private static UnitTestResult MapUnitTestResult(XElement unitTestResultElement)
    {
        var errorMessage = unitTestResultElement
            .Element(TrxNamespace + "Output")?
            .Element(TrxNamespace + "ErrorInfo")?
            .Element(TrxNamespace + "Message")?
            .Value;

        var durationRaw = (string?)unitTestResultElement.Attribute("duration");

        return new UnitTestResult
        {
            ExecutionId = (string?)unitTestResultElement.Attribute("executionId"),
            TestId = (string?)unitTestResultElement.Attribute("testId"),
            TestName = (string?)unitTestResultElement.Attribute("testName"),
            ComputerName = (string?)unitTestResultElement.Attribute("computerName"),

            Duration = durationRaw,
            DurationValue = ParseTrxDuration(durationRaw) ?? TimeSpan.Zero,

            StartTime = ParseTrxDateTimeOffset((string?)unitTestResultElement.Attribute("startTime")),
            EndTime = ParseTrxDateTimeOffset((string?)unitTestResultElement.Attribute("endTime")),

            TestType = (string?)unitTestResultElement.Attribute("testType"),
            Outcome = (string?)unitTestResultElement.Attribute("outcome"),
            TestListId = (string?)unitTestResultElement.Attribute("testListId"),
            RelativeResultsDirectory = (string?)unitTestResultElement.Attribute("relativeResultsDirectory"),
            ErrorMessage = errorMessage
        };
    }

    private static DateTimeOffset? ParseTrxDateTimeOffset(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return null;
        }

        if (DateTimeOffset.TryParse(value, CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind, out var dto))
        {
            return dto;
        }

        return null;
    }

    private static TimeSpan? ParseTrxDuration(string? duration)
    {
        // TRX duration example: "00:00:00.0044593" (hh:mm:ss.fffffff)
        if (string.IsNullOrWhiteSpace(duration))
        {
            return null;
        }

        if (TimeSpan.TryParseExact(duration, "c", CultureInfo.InvariantCulture, out var ts))
        {
            return ts;
        }

        if (TimeSpan.TryParse(duration, CultureInfo.InvariantCulture, out ts))
        {
            return ts;
        }

        return null;
    }

    public sealed record UnitTestResult
    {
        public string? ComputerName { get; init; }

        /// <summary>
        ///     Raw duration string from TRX (e.g., "00:00:00.0044593")
        /// </summary>
        public string? Duration { get; init; }

        /// <summary>Parsed duration (nullable if parsing fails)</summary>
        public TimeSpan DurationValue { get; init; } = TimeSpan.Zero;

        public DateTimeOffset? EndTime { get; init; }

        public string? ErrorMessage { get; init; }

        public string? ExecutionId { get; init; }

        public string? Outcome { get; init; }

        public string? RelativeResultsDirectory { get; init; }

        public DateTimeOffset? StartTime { get; init; }

        public string? TestId { get; init; }

        public string? TestListId { get; init; }

        public string? TestName { get; init; }

        public string? TestType { get; init; }
    }
}
