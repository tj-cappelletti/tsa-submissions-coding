using System;
using System.Collections.Generic;
using System.Linq;
using Tsa.Submissions.Coding.Contracts.Languages;
using Tsa.Submissions.Coding.Contracts.Submissions;

namespace Tsa.Submissions.Coding.WebApi.Entities;

public static partial class EntityExtensions
{
    public static SubmissionResponse ToResponse(this Submission submission)
    {
        throw new NotImplementedException("Submission to SubmissionResponse mapping is not implemented yet.");
        //if (string.IsNullOrWhiteSpace(submission.Id)) throw new InvalidOperationException("Submission ID is required.");

        //if (submission.Language == null) throw new InvalidOperationException("Submission Programming Language is required.");

        //if (submission.Problem == null) throw new InvalidOperationException("Submission Problem is required.");

        //if (string.IsNullOrWhiteSpace(submission.Solution)) throw new InvalidOperationException("Submission Solution is required.");

        //if (submission.SubmittedOn == null) throw new InvalidOperationException("Submission Submitted On is required.");

        //if (string.IsNullOrWhiteSpace(submission.UserId)) throw new InvalidOperationException("Submission User ID is required.");

        //return new SubmissionResponse(
        //    submission.Id,
        //    submission.Language.ToResponse(),
        //    submission.Problem.Id.AsString,
        //    submission.Solution,
        //    submission.SubmittedOn.Value,
        //    submission.TestCaseResults,
        //    submission.UserId);
    }

    public static IEnumerable<SubmissionResponse> ToResponses(this IEnumerable<Submission> submissions)
    {
        return submissions.Select(submission => submission.ToResponse());
    }
}
