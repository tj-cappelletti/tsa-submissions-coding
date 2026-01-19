using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Tsa.Submissions.Coding.WebApi.Entities;

namespace Tsa.Submissions.Coding.UnitTests.Helpers;

//TODO: Turn into code generator
[ExcludeFromCodeCoverage]
internal class SubmissionEqualityComparer : IEqualityComparer<Submission?>, IEqualityComparer<IList<Submission>?>
{
    private readonly bool _ignoreDateTimes;

    private readonly TestSetResultEqualityComparer _testSetResultEqualityComparer = new();

    public SubmissionEqualityComparer(bool ignoreDateTimes)
    {
        _ignoreDateTimes = ignoreDateTimes;
    }

    public SubmissionEqualityComparer() : this(false) { }

    public bool Equals(Submission? x, Submission? y)
    {
        if (ReferenceEquals(x, y)) return true;
        if (x is null) return false;
        if (y is null) return false;

        var idsMatch = x.Id == y.Id;
        var isFinalSubmissionsMatch = x.IsFinalSubmission == y.IsFinalSubmission;
        var languagesMatch = x.Language == y.Language;
        var problemsMatch = x.Problem?.Id.AsString == y.Problem?.Id.AsString;
        var solutionsMatch = x.Solution == y.Solution;
        var submittedOnsMatch = x.SubmittedOn == y.SubmittedOn;
        var testSetResultsMatch = _testSetResultEqualityComparer.Equals(x.TestSetResults, y.TestSetResults);
        var usersMatch = x.User?.Id.AsString == y.User?.Id.AsString;


        if (_ignoreDateTimes)
        {
            return idsMatch &&
                   isFinalSubmissionsMatch &&
                   languagesMatch &&
                   problemsMatch &&
                   solutionsMatch &&
                   testSetResultsMatch &&
                   usersMatch;
        }

        return idsMatch &&
               isFinalSubmissionsMatch &&
               languagesMatch &&
               problemsMatch &&
               solutionsMatch &&
               submittedOnsMatch &&
               testSetResultsMatch &&
               usersMatch;
    }

    public bool Equals(IList<Submission>? x, IList<Submission>? y)
    {
        if (ReferenceEquals(x, y)) return true;
        if (x is null) return false;
        if (y is null) return false;
        if (x.Count != y.Count) return false;

        foreach (var leftSubmission in x)
        {
            var rightSubmission = y.SingleOrDefault(submission => submission.Id == leftSubmission.Id);

            if (!Equals(leftSubmission, rightSubmission)) return false;
        }

        return true;
    }

    public int GetHashCode(Submission? obj)
    {
        return obj == null ? 0 : obj.GetHashCode();
    }

    public int GetHashCode(IList<Submission>? obj)
    {
        return obj == null ? 0 : obj.GetHashCode();
    }
}
