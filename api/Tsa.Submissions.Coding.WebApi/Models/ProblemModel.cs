using System.Collections.Generic;

namespace Tsa.Submissions.Coding.WebApi.Models;

public class ProblemModel
{
    public string? Description { get; set; }

    public string? Id { get; set; }

    public bool IsActive { get; set; }

    public IList<TestSetModel>? TestSets { get; set; }

    public string? Title { get; set; }
}
