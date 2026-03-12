using System;
using System.Diagnostics.CodeAnalysis;
using Tsa.Submissions.Coding.Contracts.Submissions;
using Tsa.Submissions.Coding.UnitTests.Helpers.Users;

namespace Tsa.Submissions.Coding.UnitTests.Helpers.Submissions;

[ExcludeFromCodeCoverage]
internal class SubmissionListResponseEqualityComparer : EqualityComparerBase<SubmissionListResponse>
{
    protected override bool EqualsCore(SubmissionListResponse x, SubmissionListResponse y)
    {
        var idsMatch = x.Id == y.Id;
        var problemIdsMatch = x.ProblemId == y.ProblemId;
        var programmingLanguageIdsMatch = x.ProgrammingLanguageId == y.ProgrammingLanguageId;
        var programmingLanguageVersionTagsMatch = x.ProgrammingLanguageVersionTag == y.ProgrammingLanguageVersionTag;
        var submittedOnsMatch = x.SubmittedOn == y.SubmittedOn;
        var usersMatch = new UserResponseEqualityComparer().Equals(x.User, y.User);

        return idsMatch
               && problemIdsMatch
               && programmingLanguageIdsMatch
               && programmingLanguageVersionTagsMatch
               && submittedOnsMatch
               && usersMatch;
    }

    public override int GetHashCode(SubmissionListResponse? obj)
    {
        return obj == null
            ? 0
            : HashCode.Combine(
                obj.Id,
                obj.ProblemId,
                obj.ProgrammingLanguageId,
                obj.ProgrammingLanguageVersionTag,
                obj.SubmittedOn,
                new UserResponseEqualityComparer().GetHashCode(obj.User));
    }

    protected override Func<SubmissionListResponse, bool> GetItemPredicate(SubmissionListResponse item)
    {
        return submissionListResponse => submissionListResponse.Id == item.Id;
    }

    protected override object GetOrderByKey(SubmissionListResponse item)
    {
        return item.Id;
    }
}
