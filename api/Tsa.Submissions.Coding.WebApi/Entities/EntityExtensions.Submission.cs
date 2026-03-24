using System;
using System.Collections.Generic;
using System.Linq;
using Tsa.Submissions.Coding.Contracts.Pagination;
using Tsa.Submissions.Coding.Contracts.Submissions;
using Tsa.Submissions.Coding.WebApi.Pagination;

namespace Tsa.Submissions.Coding.WebApi.Entities;

public static partial class EntityExtensions
{
    public static SubmissionListResponse ToListResponse(this Submission submission, Problem problem, ProgrammingLanguage programmingLanguage, User user)
    {
        if (string.IsNullOrWhiteSpace(submission.Id))
        {
            throw new ArgumentException("Submission ID is required.", nameof(submission));
        }

        if (submission.ProblemId != problem.Id)
        {
            throw new ArgumentException($"Submission Problem ID '{submission.ProblemId}' does not match Problem ID '{problem.Id}'.", nameof(submission));
        }

        if (submission.ProgrammingLanguageId != programmingLanguage.Id)
        {
            throw new ArgumentException(
                $"Submission Programming Language ID '{submission.ProgrammingLanguageId}' does not match Programming Language ID '{programmingLanguage.Id}'.",
                nameof(submission));
        }

        // It is safe to assume that the programming language has at least one version
        // The UI should enforce that a programming language version is selected when submitting a solution
        // The checks here are just a safeguard to ensure data integrity in case of any issues on the client side or if the API is called directly without using the UI
        if (programmingLanguage.Versions.All(version => version.VersionTag != submission.ProgrammingLanguageVersionTag))
        {
            throw new ArgumentException(
                $"Submission Programming Language Version Tag '{submission.ProgrammingLanguageVersionTag}' does not match any Version Tag(s) for Programming Language ID '{programmingLanguage.Id}'.",
                nameof(submission));
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
            problem.ToListResponse(),
            programmingLanguage.ToResponse(),
            submission.ProgrammingLanguageVersionTag!,
            submission.SubmittedOn.Value,
            submission.EvaluatedOn,
            user.ToResponse());
    }

    public static PaginatedResponse<SubmissionListResponse> ToPaginatedResponse(
        this PagedResult<Submission> submissions,
        IEnumerable<Problem> problems,
        IEnumerable<ProgrammingLanguage> programmingLanguages,
        IEnumerable<User> users)
    {
        return new PaginatedResponse<SubmissionListResponse>(
            submissions.Items.ToResponses(problems, programmingLanguages, users).ToList(),
            submissions.PageSize,
            submissions.HasNextPage,
            submissions.NextCursor);
    }

    public static SubmissionResponse ToResponse(this Submission submission, ProgrammingLanguage programmingLanguage, Problem problem, User user)
    {
        if (string.IsNullOrWhiteSpace(submission.Id)) throw new InvalidOperationException("Submission ID is required.");

        if (submission.ProgrammingLanguageId != programmingLanguage.Id ||
            programmingLanguage.Versions.All(version => version.VersionTag != submission.ProgrammingLanguageVersionTag))
        {
            throw new InvalidOperationException(
                "The Programming Language for a Submission is required and must match the supplied programming language.");
        }

        if (submission.ProblemId != problem.Id)
            throw new InvalidOperationException("The Problem for a Submission is required and must match the supplied problem.");

        if (string.IsNullOrWhiteSpace(submission.Solution)) throw new InvalidOperationException("Submission Solution is required.");

        if (submission.SubmittedOn == null) throw new InvalidOperationException("Submission Submitted On is required.");

        if (submission.UserId != user.Id) throw new InvalidOperationException("The User for a Submission is required and must match the supplied user.");

        return new SubmissionResponse(
            submission.Id,
            user.ToResponse(),
            programmingLanguage.ToResponse(),
            // The ProgrammingLanguageVersionTag is required and has already been validated to match a version in the provided ProgrammingLanguage, so it is safe to use the null-forgiving operator here.
            submission.ProgrammingLanguageVersionTag!,
            problem.ToListResponse(),
            submission.Solution,
            submission.SubmittedOn.Value,
            submission.EvaluatedOn,
            submission.TestCaseResults.Select(testCaseResult => testCaseResult.ToResponse()).ToList());
    }

    public static IEnumerable<SubmissionListResponse> ToResponses(
        this IEnumerable<Submission> submissions,
        IEnumerable<Problem> problems,
        IEnumerable<ProgrammingLanguage> programmingLanguages,
        IEnumerable<User> users)
    {
        return submissions.Select(submission =>
        {
            var problem = problems.FirstOrDefault(p => p.Id == submission.ProblemId);

            if (problem == null)
            {
                throw new InvalidOperationException($"Problem with ID '{submission.ProblemId}' not found for submission '{submission.Id}'.");
            }

            var programmingLanguage = programmingLanguages.FirstOrDefault(pl => pl.Id == submission.ProgrammingLanguageId);

            if (programmingLanguage == null)
            {
                throw new InvalidOperationException(
                    $"Programming Language with ID '{submission.ProgrammingLanguageId}' not found for submission '{submission.Id}'.");
            }

            var user = users.FirstOrDefault(u => u.Id == submission.UserId);

            if (user == null)
            {
                throw new InvalidOperationException($"User with ID '{submission.UserId}' not found for submission '{submission.Id}'.");
            }

            return submission.ToListResponse(problem, programmingLanguage, user);
        });
    }

    public static IEnumerable<SubmissionResponse> ToResponses(this IEnumerable<Submission> submissions)
    {
        // This method may not be needed since we'll likely never return the full SubmissionResponse in a list operation
        // This likely needs to be SubmissionListResponse

        // return submissions.Select(submission => submission.ToResponse());
        throw new NotImplementedException();
    }
}
