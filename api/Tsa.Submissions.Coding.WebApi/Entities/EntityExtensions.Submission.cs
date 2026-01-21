using System;
using System.Collections.Generic;
using System.Linq;
using Tsa.Submissions.Coding.Contracts.Submissions;

namespace Tsa.Submissions.Coding.WebApi.Entities;

public static partial class EntityExtensions
{
    public static ProgrammingLanguageResponse ToResponse(this ProgrammingLanguage programmingLanguage)
    {
        return new ProgrammingLanguageResponse(
            programmingLanguage.Name,
            programmingLanguage.Version);
    }

    public static SubmissionResponse ToResponse(this Submission submission, IList<TestSetResultResponse>? testSetResultResponses = null)
    {
        if (string.IsNullOrWhiteSpace(submission.Id)) throw new InvalidOperationException("Submission ID is required.");

        if (submission.Language == null) throw new InvalidOperationException("Submission Programming Language is required.");

        if (submission.Problem == null) throw new InvalidOperationException("Submission Problem is required.");

        if (string.IsNullOrWhiteSpace(submission.Solution)) throw new InvalidOperationException("Submission Solution is required.");

        if (submission.SubmittedOn == null) throw new InvalidOperationException("Submission Submitted On is required.");

        if (submission.User == null) throw new InvalidOperationException("Submission User is required.");

        return new SubmissionResponse(
            submission.Id,
            submission.Language.ToResponse(),
            submission.Problem.Id.AsString,
            submission.Solution,
            submission.SubmittedOn.Value,
            testSetResultResponses ?? [],
            submission.User.Id.AsString);
    }

    public static TestSetResultResponse ToResponse(this TestSetResult testSetResult)
    {
        if (testSetResult.TestSet == null) throw new InvalidOperationException("Test Set Result Test Set is required.");

        return new TestSetResultResponse(
            testSetResult.Passed,
            testSetResult.RunDuration,
            testSetResult.TestSet.Id.AsString);
    }

    public static IEnumerable<SubmissionResponse> ToResponses(this IEnumerable<Submission> submissions)
    {
        return submissions.Select(submission => submission.ToResponse()).ToList();
    }
}
