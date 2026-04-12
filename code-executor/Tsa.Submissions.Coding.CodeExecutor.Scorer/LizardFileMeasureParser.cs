using System.Globalization;
using System.Xml.Linq;

namespace Tsa.Submissions.Coding.CodeExecutor.Scorer;

public sealed record LizardFileMeasure(
    string FileName,
    IReadOnlyList<double> Values,
    IReadOnlyDictionary<string, double>? MetricsByLabel);

public static class LizardFileMeasureParser
{
    private static List<string> ExtractLabels(XElement measure)
    {
        // Handles:
        // <labels><label>...</label></labels>
        // or <labels>a,b,c</labels>
        var labelsNode = measure.Elements()
            .FirstOrDefault(e => e.Name.LocalName.Equals("labels", StringComparison.OrdinalIgnoreCase));

        if (labelsNode is null)
        {
            return [];
        }

        var labelElements = labelsNode.Elements()
            .Where(e => e.Name.LocalName.Equals("label", StringComparison.OrdinalIgnoreCase))
            .Select(e => e.Value.Trim())
            .Where(s => !string.IsNullOrWhiteSpace(s))
            .ToList();

        if (labelElements.Count > 0)
        {
            return labelElements;
        }

        return labelsNode.Value
            .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .Where(s => !string.IsNullOrWhiteSpace(s))
            .ToList();
    }

    private static List<double> ExtractValues(XElement item)
    {
        // Typical shape: <item ...><value>..</value><value>..</value></item>
        var directValues = item.Elements()
            .Where(e => e.Name.LocalName.Equals("value", StringComparison.OrdinalIgnoreCase))
            .Select(e => ParseDoubleSafe(e.Value))
            .ToList();

        if (directValues.Count > 0)
        {
            return directValues;
        }

        // Fallback: <item value="..."> or comma/space text content
        var attrValue = (string?)item.Attribute("value");
        if (!string.IsNullOrWhiteSpace(attrValue))
        {
            return SplitAndParse(attrValue);
        }

        var text = item.Value.Trim();

        if (!string.IsNullOrWhiteSpace(text))
        {
            return SplitAndParse(text);
        }

        return [];
    }

    private static IReadOnlyList<LizardFileMeasure> Parse(XDocument doc)
    {
        var fileMeasure = doc
            .Descendants()
            .FirstOrDefault(e =>
                e.Name.LocalName.Equals("measure", StringComparison.OrdinalIgnoreCase) &&
                string.Equals((string?)e.Attribute("type"), "File", StringComparison.OrdinalIgnoreCase));

        if (fileMeasure is null)
        {
            return Array.Empty<LizardFileMeasure>();
        }

        var labels = ExtractLabels(fileMeasure);
        var result = new List<LizardFileMeasure>();

        foreach (var item in fileMeasure.Elements().Where(e => e.Name.LocalName.Equals("item", StringComparison.OrdinalIgnoreCase)))
        {
            var fileName = (string?)item.Attribute("name") ?? string.Empty;
            var values = ExtractValues(item);

            IReadOnlyDictionary<string, double>? metrics = null;
            if (labels.Count > 0 && labels.Count == values.Count)
            {
                metrics = labels
                    .Select((label, i) => new { label, value = values[i] })
                    .ToDictionary(x => x.label, x => x.value, StringComparer.OrdinalIgnoreCase);
            }

            result.Add(new LizardFileMeasure(fileName, values, metrics));
        }

        return result;
    }

    private static double ParseDoubleSafe(string raw)
    {
        return double.TryParse(raw, NumberStyles.Float | NumberStyles.AllowThousands, CultureInfo.InvariantCulture, out var d)
            ? d
            : 0d;
    }

    public static IReadOnlyList<LizardFileMeasure> ParseFromFile(string xmlPath)
    {
        var doc = XDocument.Load(xmlPath);
        return Parse(doc);
    }

    public static IReadOnlyList<LizardFileMeasure> ParseFromString(string xml)
    {
        var doc = XDocument.Parse(xml);
        return Parse(doc);
    }

    private static List<double> SplitAndParse(string input)
    {
        return input
            .Split(new[] { ',', ';', ' ', '\t', '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries)
            .Select(ParseDoubleSafe)
            .ToList();
    }
}
