namespace Tsa.Submissions.Coding.WebApi.Models;

public class TestSetValueModel
{
    public string? DataType { get; set; }

    public int? Index { get; set; }

    public bool IsArray { get; set; }

    public string? ValueAsJson { get; set; }
}
