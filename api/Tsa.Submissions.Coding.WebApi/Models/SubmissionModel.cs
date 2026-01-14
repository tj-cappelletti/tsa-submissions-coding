using System;
using System.Collections.Generic;

namespace Tsa.Submissions.Coding.WebApi.Models;

public class SubmissionModel
{
    public string? Id { get; set; }

    public bool IsFinalSubmission { get; set; }

    public string? Language { get; set; }

    public string? ProblemId { get; set; }

    public string? Solution { get; set; }

    public DateTimeOffset? SubmittedOn { get; set; }

    public IList<TestSetResultModel>? TestSetResults { get; set; }

    public UserModel? User { get; set; }
}
