using System.Collections.Generic;
using System.Linq;
using Tsa.Submissions.Coding.WebApi.Models;

namespace Tsa.Submissions.Coding.WebApi.Entities;

public static partial class EntityExtensions
{
    private static List<SubmissionModel> SubmissionsToSubmissionModels(IEnumerable<Submission> submissions)
    {
        return submissions.Select(submission => submission.ToModel()).ToList();
    }

    public static SubmissionModel ToModel(this Submission submission)
    {
        return new SubmissionModel
        {
            Id = submission.Id,
            IsFinalSubmission = submission.IsFinalSubmission,
            Language = submission.Language,
            // Problem is required, if null, we are in a bad state
            ProblemId = submission.Problem!.Id.AsString,
            Solution = submission.Solution,
            SubmittedOn = submission.SubmittedOn,
            TestSetResults = submission.TestSetResults?.ToModels(),
            // User is required, if null, we are in a bad state
            User = new UserModel { Id = submission.User!.Id.AsString }
        };
    }

    public static List<SubmissionModel> ToModels(this IList<Submission> submissions)
    {
        return SubmissionsToSubmissionModels(submissions);
    }

    public static List<SubmissionModel> ToModels(this IEnumerable<Submission> submissions)
    {
        return SubmissionsToSubmissionModels(submissions);
    }
}
