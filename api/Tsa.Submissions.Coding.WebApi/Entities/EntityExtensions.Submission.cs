using System;
using System.Collections.Generic;
using System.Linq;
using Tsa.Submissions.Coding.Contracts.Languages;
using Tsa.Submissions.Coding.Contracts.Submissions;

namespace Tsa.Submissions.Coding.WebApi.Entities;

public static partial class EntityExtensions
{
    public static ProgrammingLanguageResponse ToResponse(this ProgrammingLanguage programmingLanguage)
    {
        var versions = new List<ProgrammingLanguageVersionResponse>();

        foreach (var programmingLanguageVersion in programmingLanguage.Versions)
        {
            if (string.IsNullOrWhiteSpace(programmingLanguageVersion.DisplayName))
            {
                throw new InvalidOperationException("Programming Language Version Display Name is required.");
            }

            if (string.IsNullOrWhiteSpace(programmingLanguageVersion.VersionTag))
            {
                throw new InvalidOperationException("Programming Language Version Tag is required.");
            }

            versions.Add(new ProgrammingLanguageVersionResponse(
                programmingLanguageVersion.DisplayName,
                programmingLanguageVersion.IsDefault,
                programmingLanguageVersion.VersionTag
            ));
        }

        if (string.IsNullOrWhiteSpace(programmingLanguage.Id)) throw new InvalidOperationException("Programming Language ID is required.");

        if (string.IsNullOrWhiteSpace(programmingLanguage.Identifier))
            throw new InvalidOperationException("Programming Language Identifier is required.");

        if (string.IsNullOrWhiteSpace(programmingLanguage.Name)) throw new InvalidOperationException("Programming Language Name is required.");

        if (string.IsNullOrWhiteSpace(programmingLanguage.FileExtension))
            throw new InvalidOperationException("Programming Language File Extension is required.");

        return new ProgrammingLanguageResponse(
            programmingLanguage.Id,
            programmingLanguage.Identifier,
            programmingLanguage.Name,
            programmingLanguage.FileExtension,
            programmingLanguage.IsEnabled,
            versions);
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

    public static IEnumerable<SubmissionResponse> ToResponses(this IEnumerable<Submission> submissions)
    {
        return submissions.Select(submission => submission.ToResponse());
    }
}
