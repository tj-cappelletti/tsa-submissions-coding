using System.Collections.Generic;

namespace Tsa.Submissions.Coding.WebApi.Models;

public class TestSetModel
{
    public string? Id { get; set; }

    public IList<TestSetValueModel>? Inputs { get; set; }

    public bool IsPublic { get; set; }

    public string? Name { get; set; }

    public string? ProblemId { get; set; }
}
