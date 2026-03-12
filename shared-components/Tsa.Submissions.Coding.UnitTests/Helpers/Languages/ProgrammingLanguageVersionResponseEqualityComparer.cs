using System;
using System.Diagnostics.CodeAnalysis;
using Tsa.Submissions.Coding.Contracts.Languages;

namespace Tsa.Submissions.Coding.UnitTests.Helpers.Languages;

[ExcludeFromCodeCoverage]
internal class ProgrammingLanguageVersionResponseEqualityComparer : EqualityComparerBase<ProgrammingLanguageVersionResponse>
{
    protected override bool EqualsCore(ProgrammingLanguageVersionResponse x, ProgrammingLanguageVersionResponse y)
    {
        var displayNamesMatch = x.DisplayName == y.DisplayName;
        var isDefaultMatches = x.IsDefault == y.IsDefault;
        var versionTagsMatch = x.VersionTag == y.VersionTag;

        return displayNamesMatch && isDefaultMatches && versionTagsMatch;
    }

    public override int GetHashCode(ProgrammingLanguageVersionResponse? obj)
    {
        return obj == null ? 0 : HashCode.Combine(obj.DisplayName, obj.IsDefault, obj.VersionTag);
    }

    protected override Func<ProgrammingLanguageVersionResponse, bool> GetItemPredicate(ProgrammingLanguageVersionResponse item)
    {
        return programmingLanguageVersionResponse => programmingLanguageVersionResponse.VersionTag == item.VersionTag;
    }

    protected override object GetOrderByKey(ProgrammingLanguageVersionResponse item)
    {
        return item.VersionTag;
    }
}
