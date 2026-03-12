using System;
using System.Diagnostics.CodeAnalysis;
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

    public override int GetHashCode(ProgrammingLanguageVersionRequest? obj)
    {
        return obj == null ? 0 : HashCode.Combine(obj.DisplayName, obj.IsDefault, obj.VersionTag);
    }

    protected override Func<ProgrammingLanguageVersionRequest, bool> GetItemPredicate(ProgrammingLanguageVersionRequest item)
    {
        return programmingLanguageVersionRequest => programmingLanguageVersionRequest.VersionTag == item.VersionTag;
    }

    protected override object GetOrderByKey(ProgrammingLanguageVersionRequest item)
    {
        return item.VersionTag;
    }
}
