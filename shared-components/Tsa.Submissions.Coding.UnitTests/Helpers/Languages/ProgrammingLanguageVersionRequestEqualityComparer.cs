using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Tsa.Submissions.Coding.Contracts.Languages;

namespace Tsa.Submissions.Coding.UnitTests.Helpers.Languages;

[ExcludeFromCodeCoverage]
internal class ProgrammingLanguageVersionRequestEqualityComparer : EqualityComparerBase<ProgrammingLanguageVersionRequest>
{
    protected override bool EqualsCore(ProgrammingLanguageVersionRequest x, ProgrammingLanguageVersionRequest y)
    {
        var displayNamesMatch = x.DisplayName == y.DisplayName;
        var isDefaultMatch = x.IsDefault == y.IsDefault;
        var versionTagsMatch = x.VersionTag == y.VersionTag;

        return displayNamesMatch && isDefaultMatch && versionTagsMatch;
    }

    protected override bool EqualsCore(IList<ProgrammingLanguageVersionRequest> x, IList<ProgrammingLanguageVersionRequest> y)
    {
        foreach (var left in x)
        {
            var right = y.SingleOrDefault(problemResponse => problemResponse.VersionTag == left.VersionTag);

            if (!Equals(left, right)) return false;
        }

        return true;
    }

    public override int GetHashCode(ProgrammingLanguageVersionRequest? obj)
    {
        return obj == null ? 0 : HashCode.Combine(obj.DisplayName, obj.IsDefault, obj.VersionTag);
    }

    protected override object GetOrderByKey(ProgrammingLanguageVersionRequest item)
    {
        return item.VersionTag;
    }
}
