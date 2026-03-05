using System;
using System.Collections.Generic;
using System.Linq;
using Tsa.Submissions.Coding.Contracts.Pagination;
using Tsa.Submissions.Coding.Contracts.Submissions;
using Tsa.Submissions.Coding.WebApi.Pagination;

namespace Tsa.Submissions.Coding.WebApi.Entities;

public static partial class EntityExtensions
{
    public static SubmissionListResponse ToListResponse(this Submission submission, User user)
    {
        if (string.IsNullOrWhiteSpace(submission.Id))
        {
            throw new ArgumentException("Submission ID is required.", nameof(submission));
        }

        if (string.IsNullOrWhiteSpace(submission.ProblemId))
        {
            throw new ArgumentException("Submission Problem ID is required.", nameof(submission));
        }

        if (string.IsNullOrWhiteSpace(submission.ProgrammingLanguageId))
        {
            throw new ArgumentException("Submission Programming Language ID is required.", nameof(submission));
        }

        if (string.IsNullOrWhiteSpace(submission.ProgrammingLanguageVersionTag))
        {
            throw new ArgumentException("Submission Programming Language Version Tag is required.", nameof(submission));
        }

        if (submission.SubmittedOn == null)
        {
            throw new ArgumentException("Submission Submitted On is required.", nameof(submission));
        }

        if (submission.UserId != user.Id)
        {
            throw new ArgumentException($"Submission User ID '{submission.UserId}' does not match User ID '{user.Id}'.", nameof(submission));
        }

        return new SubmissionListResponse(
            submission.Id,
            submission.ProblemId,
            submission.ProgrammingLanguageId,
            submission.ProgrammingLanguageVersionTag,
            submission.SubmittedOn.Value,
            user.ToResponse());
    }

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

    public static IEnumerable<SubmissionListResponse> ToResponses(
        this IEnumerable<Submission> submissions,
        IEnumerable<User> users)
    {
        return submissions.Select(submission =>
        {
            var user = users.FirstOrDefault(u => u.Id == submission.UserId);

            if (user == null)
            {
                throw new InvalidOperationException($"User with ID '{submission.UserId}' not found for submission '{submission.Id}'.");
            }

            return submission.ToListResponse(user);
        });
    }

    public static IEnumerable<SubmissionResponse> ToResponses(this IEnumerable<Submission> submissions)
    {
        return submissions.Select(submission => submission.ToResponse());
    }

    public static PaginatedResponse<SubmissionListResponse> ToPaginatedResponse(
        this PagedResult<Submission> submissions,
        IList<User> users)
    {
        return new PaginatedResponse<SubmissionListResponse>(
            submissions.Items.ToResponses(users).ToList(),
            submissions.PageSize,
            submissions.HasNextPage,
            submissions.NextCursor);
    }
}
