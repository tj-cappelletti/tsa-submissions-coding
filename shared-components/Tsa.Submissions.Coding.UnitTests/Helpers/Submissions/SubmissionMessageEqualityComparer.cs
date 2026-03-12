using System;
using System.Diagnostics.CodeAnalysis;
using Tsa.Submissions.Coding.Contracts.Messages;

namespace Tsa.Submissions.Coding.UnitTests.Helpers.Submissions;

[ExcludeFromCodeCoverage]
internal class SubmissionMessageEqualityComparer : EqualityComparerBase<SubmissionMessage>
{
    protected override bool EqualsCore(SubmissionMessage x, SubmissionMessage y)
    {
        var problemIdsMatch = x.ProblemId == y.ProblemId;
        var submissionIdsMatch = x.SubmissionId == y.SubmissionId;
        var submittedOnMatch = x.SubmittedOn == y.SubmittedOn;
        var userIdsMatch = x.UserId == y.UserId;

        return problemIdsMatch && submissionIdsMatch && submittedOnMatch && userIdsMatch;
    }

    public override int GetHashCode(SubmissionMessage? obj)
    {
        return obj == null
            ? 0
            : HashCode.Combine(obj.ProblemId, obj.SubmissionId, obj.SubmittedOn, obj.UserId);
    }

    protected override Func<SubmissionMessage, bool> GetItemPredicate(SubmissionMessage item)
    {
        return submissionMessage => submissionMessage.SubmissionId == item.SubmissionId;
    }

    protected override object GetOrderByKey(SubmissionMessage item)
    {
        return item.SubmissionId;
    }
}
