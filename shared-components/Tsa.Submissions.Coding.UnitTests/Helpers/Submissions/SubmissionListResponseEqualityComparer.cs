using System;
using System.Diagnostics.CodeAnalysis;
using Tsa.Submissions.Coding.Contracts.Submissions;
using Tsa.Submissions.Coding.UnitTests.Helpers.Languages;
using Tsa.Submissions.Coding.UnitTests.Helpers.Problems;
using Tsa.Submissions.Coding.UnitTests.Helpers.Users;

namespace Tsa.Submissions.Coding.UnitTests.Helpers.Submissions;

[ExcludeFromCodeCoverage]
internal class SubmissionListResponseEqualityComparer : EqualityComparerBase<SubmissionListResponse>
{
    protected override bool EqualsCore(SubmissionListResponse x, SubmissionListResponse y)
    {
        var idsMatch = x.Id == y.Id;
        var problemsMatch = new ProblemListResponseEqualityComparer().Equals(x.Problem, y.Problem);
        var programmingLanguagesMatch = new ProgrammingLanguageResponseEqualityComparer().Equals(x.ProgrammingLanguage, y.ProgrammingLanguage);
        var programmingLanguageVersionTagsMatch = x.ProgrammingLanguageVersionTag == y.ProgrammingLanguageVersionTag;
        var submittedOnsMatch = x.SubmittedOn == y.SubmittedOn;
        var usersMatch = new UserResponseEqualityComparer().Equals(x.User, y.User);

        return idsMatch
               && problemsMatch
               && programmingLanguagesMatch
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
                new ProblemListResponseEqualityComparer().GetHashCode(obj.Problem),
                new ProgrammingLanguageResponseEqualityComparer().GetHashCode(obj.ProgrammingLanguage),
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
