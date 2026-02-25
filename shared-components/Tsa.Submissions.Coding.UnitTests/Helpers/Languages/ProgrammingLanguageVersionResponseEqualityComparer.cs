using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
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

    protected override bool EqualsCore(IList<ProgrammingLanguageVersionResponse> x, IList<ProgrammingLanguageVersionResponse> y)
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

    public override int GetHashCode(ProgrammingLanguageVersionResponse? obj)
    {
        return obj == null ? 0 : HashCode.Combine(obj.DisplayName, obj.IsDefault, obj.VersionTag);
    }

    /// <summary>
    ///     Provides the ordering key for list hash code computation.
    ///     Orders by VersionTag to ensure consistent hash codes.
    /// </summary>
    protected override object GetOrderByKey(ProgrammingLanguageVersionResponse item)
    {
        return item.VersionTag;
    }
}
