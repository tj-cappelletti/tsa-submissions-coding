using System;
using System.Diagnostics.CodeAnalysis;
using Tsa.Submissions.Coding.Contracts.Submissions;

namespace Tsa.Submissions.Coding.UnitTests.Helpers.Submissions;

[ExcludeFromCodeCoverage]
internal class SubmissionCreateRequestEqualityComparer : EqualityComparerBase<SubmissionCreateRequest>
{
    protected override bool EqualsCore(SubmissionCreateRequest x, SubmissionCreateRequest y)
    {
        var problemIdsMatch = x.ProblemId == y.ProblemId;
        var programmingLanguageIdsMatch = x.ProgrammingLanguageId == y.ProgrammingLanguageId;
        var programmingLanguageVersionTagsMatch = x.ProgrammingLanguageVersionTag == y.ProgrammingLanguageVersionTag;
        var solutionsMatch = x.Solution == y.Solution;

        return problemIdsMatch
               && programmingLanguageIdsMatch
               && programmingLanguageVersionTagsMatch
               && solutionsMatch;
    }

    public override int GetHashCode(SubmissionCreateRequest? obj)
    {
        return obj == null
            ? 0
            : HashCode.Combine(
                obj.ProblemId,
                obj.ProgrammingLanguageId,
                obj.ProgrammingLanguageVersionTag,
                obj.Solution);
    }

    protected override Func<SubmissionCreateRequest, bool> GetItemPredicate(SubmissionCreateRequest item)
    {
        return submissionCreateRequest => submissionCreateRequest.Solution == item.Solution;
    }

    protected override object GetOrderByKey(SubmissionCreateRequest item)
    {
        return item.Solution;
    }
}
