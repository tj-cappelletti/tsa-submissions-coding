using System;
using System.Diagnostics.CodeAnalysis;
using Tsa.Submissions.Coding.WebApi.Entities;

namespace Tsa.Submissions.Coding.UnitTests.Helpers.Languages;

[ExcludeFromCodeCoverage]
internal class ProgrammingLanguageVersionEqualityComparer : EqualityComparerBase<ProgrammingLanguageVersion>
{
    protected override bool EqualsCore(ProgrammingLanguageVersion x, ProgrammingLanguageVersion y)
    {
        var displayNamesMatch = x.DisplayName == y.DisplayName;
        var isDefaultMatches = x.IsDefault == y.IsDefault;
        var versionTagsMatch = x.VersionTag == y.VersionTag;

        return displayNamesMatch && isDefaultMatches && versionTagsMatch;
    }

    public override int GetHashCode(ProgrammingLanguageVersion? obj)
    {
        return obj == null ? 0 : HashCode.Combine(obj.DisplayName, obj.IsDefault, obj.VersionTag);
    }

    protected override Func<ProgrammingLanguageVersion, bool> GetItemPredicate(ProgrammingLanguageVersion item)
    {
        return programmingLanguageVersionResponse => programmingLanguageVersionResponse.VersionTag == item.VersionTag;
    }

    protected override object GetOrderByKey(ProgrammingLanguageVersion item)
    {
        return item.VersionTag!;
    }
}
