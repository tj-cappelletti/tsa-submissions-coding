using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Tsa.Submissions.Coding.Contracts.Languages;

namespace Tsa.Submissions.Coding.UnitTests.Helpers.Languages;

[ExcludeFromCodeCoverage]
internal class ProgrammingLanguageResponseEqualityComparer : EqualityComparerBase<ProgrammingLanguageResponse>
{
    protected override bool EqualsCore(ProgrammingLanguageResponse x, ProgrammingLanguageResponse y)
    {
        var fileExtensionsMatch = x.FileExtension == y.FileExtension;
        var idsMatch = x.Id == y.Id;
        var identifiersMatch = x.Identifier == y.Identifier;
        var isEnabledMatch = x.IsEnabled == y.IsEnabled;
        var namesMatch = x.Name == y.Name;
        var versionsMatch = new ProgrammingLanguageVersionResponseEqualityComparer().Equals(x.Versions, y.Versions);

        return fileExtensionsMatch &&
               idsMatch &&
               identifiersMatch &&
               isEnabledMatch &&
               namesMatch &&
               versionsMatch;
    }

    protected override bool EqualsCore(IList<ProgrammingLanguageResponse> x, IList<ProgrammingLanguageResponse> y)
    {
        foreach (var leftProgrammingLanguageResponse in x)
        {
            var matchingProgrammingLanguageResponse = y
                .SingleOrDefault(programmingLanguageResponse =>
                    programmingLanguageResponse.Id == leftProgrammingLanguageResponse.Id);

            if (!Equals(leftProgrammingLanguageResponse, matchingProgrammingLanguageResponse))
            {
                return false;
            }
        }

        return true;
    }

    public override int GetHashCode(ProgrammingLanguageResponse? obj)
    {
        if (obj == null) return 0;

        var hashCode = new HashCode();
        hashCode.Add(obj.FileExtension);
        hashCode.Add(obj.Id);
        hashCode.Add(obj.Identifier);
        hashCode.Add(obj.IsEnabled);
        hashCode.Add(obj.Name);

        foreach (var version in obj.Versions)
        {
            hashCode.Add(version);
        }

        return hashCode.ToHashCode();
    }

    protected override object GetOrderByKey(ProgrammingLanguageResponse item)
    {
        return item.Id;
    }
}
