using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
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

    protected override bool EqualsCore(IList<ProgrammingLanguageVersion> x, IList<ProgrammingLanguageVersion> y)
    {
        foreach (var leftProgrammingLanguageVersionResponse in x)
        {
            var matchingProgrammingLanguageVersionResponse = y
                .SingleOrDefault(programmingLanguageVersionResponse =>
                    programmingLanguageVersionResponse.VersionTag == leftProgrammingLanguageVersionResponse.VersionTag);

            if (matchingProgrammingLanguageVersionResponse == null)
            {
                return false;
            }
        }

        return true;
    }

    public override int GetHashCode(ProgrammingLanguageVersion? obj)
    {
        return obj == null ? 0 : HashCode.Combine(obj.DisplayName, obj.IsDefault, obj.VersionTag);
    }

    protected override object GetOrderByKey(ProgrammingLanguageVersion item)
    {
        return item.VersionTag!;
    }
}
