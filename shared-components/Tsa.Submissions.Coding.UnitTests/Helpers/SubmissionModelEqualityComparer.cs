using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Tsa.Submissions.Coding.Contracts.Submissions;

namespace Tsa.Submissions.Coding.UnitTests.Helpers;

[ExcludeFromCodeCoverage]
internal class SubmissionModelEqualityComparer : IEqualityComparer<SubmissionResponse?>, IEqualityComparer<IList<SubmissionResponse>?>
{
    private readonly TestSetResultModelEqualityComparer _testSetResultModelEqualityComparer = new();

    public bool Equals(SubmissionResponse? x, SubmissionResponse? y)
    {
        if (ReferenceEquals(x, y)) return true;
        if (x is null) return false;
        if (y is null) return false;

        var idsMatch = x.Id == y.Id;
        var languagesMatch = x.Language == y.Language;
        var problemsMatch = x.ProblemId == y.ProblemId;
        var solutionsMatch = x.Solution == y.Solution;
        var submittedOnsMatch = x.SubmittedOn == y.SubmittedOn;
        var testSetResultsMatch = _testSetResultModelEqualityComparer.Equals(x.TestSetResults, y.TestSetResults);
        var usersMatch = x.UserId == y.UserId;

        return idsMatch &&
               languagesMatch &&
               problemsMatch &&
               solutionsMatch &&
               submittedOnsMatch &&
               testSetResultsMatch &&
               usersMatch;
    }

    public bool Equals(IList<SubmissionResponse>? x, IList<SubmissionResponse>? y)
    {
        if (ReferenceEquals(x, y)) return true;
        if (x is null) return false;
        if (y is null) return false;
        if (x.Count != y.Count) return false;

        foreach (var leftSubmissionModel in x)
        {
            var rightSubmissionModel = y.SingleOrDefault(submissionModel => submissionModel.Id == leftSubmissionModel.Id);

            if (!Equals(leftSubmissionModel, rightSubmissionModel)) return false;
        }

        return true;
    }

    public int GetHashCode(SubmissionResponse? obj)
    {
        return obj == null ? 0 : obj.GetHashCode();
    }

    public int GetHashCode(IList<SubmissionResponse>? obj)
    {
        return obj == null ? 0 : obj.GetHashCode();
    }
}
