using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Tsa.Submissions.Coding.WebApi.Models;

namespace Tsa.Submissions.Coding.UnitTests.Helpers;

[ExcludeFromCodeCoverage]
internal class SubmissionModelEqualityComparer : IEqualityComparer<SubmissionModel?>, IEqualityComparer<IList<SubmissionModel>?>
{
    private readonly TestSetResultModelEqualityComparer _testSetResultModelEqualityComparer = new();
    private readonly UserModelEqualityComparer _userModelEqualityComparer = new();

    public bool Equals(SubmissionModel? x, SubmissionModel? y)
    {
        if (ReferenceEquals(x, y)) return true;
        if (x is null) return false;
        if (y is null) return false;

        var idsMatch = x.Id == y.Id;
        var isFinalSubmissionModelsMatch = x.IsFinalSubmission == y.IsFinalSubmission;
        var languagesMatch = x.Language == y.Language;
        var problemsMatch = x.ProblemId == y.ProblemId;
        var solutionsMatch = x.Solution == y.Solution;
        var submittedOnsMatch = x.SubmittedOn == y.SubmittedOn;
        var testSetResultsMatch = _testSetResultModelEqualityComparer.Equals(x.TestSetResults, y.TestSetResults);
        var usersMatch = _userModelEqualityComparer.Equals(x.User, y.User);

        return idsMatch &&
               isFinalSubmissionModelsMatch &&
               languagesMatch &&
               problemsMatch &&
               solutionsMatch &&
               submittedOnsMatch &&
               testSetResultsMatch &&
               usersMatch;
    }

    public bool Equals(IList<SubmissionModel>? x, IList<SubmissionModel>? y)
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

    public int GetHashCode(SubmissionModel? obj)
    {
        return obj == null ? 0 : obj.GetHashCode();
    }

    public int GetHashCode(IList<SubmissionModel>? obj)
    {
        return obj == null ? 0 : obj.GetHashCode();
    }
}
