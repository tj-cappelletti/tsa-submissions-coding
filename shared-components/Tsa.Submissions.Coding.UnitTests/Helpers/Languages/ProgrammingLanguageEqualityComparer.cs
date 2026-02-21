using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tsa.Submissions.Coding.WebApi.Entities;

namespace Tsa.Submissions.Coding.UnitTests.Helpers.Languages;

[ExcludeFromCodeCoverage]
internal class ProgrammingLanguageEqualityComparer : EqualityComparerBase<ProgrammingLanguage>
{
    protected override bool EqualsCore(ProgrammingLanguage x, ProgrammingLanguage y)
    {
        var fileExtensionsMatch = x.FileExtension == y.FileExtension;
        var idsMatch = x.Id == y.Id;
        var identifiersMatch = x.Identifier == y.Identifier;
        var isEnabledMatch = x.IsEnabled == y.IsEnabled;
        var namesMatch = x.Name == y.Name;
        var versionsMatch = new ProgrammingLanguageVersionEqualityComparer().Equals(x.Versions, y.Versions);

        return fileExtensionsMatch &&
               idsMatch &&
               identifiersMatch &&
               isEnabledMatch &&
               namesMatch &&
               versionsMatch;
    }

    protected override bool EqualsCore(IList<ProgrammingLanguage> x, IList<ProgrammingLanguage> y)
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

    public override int GetHashCode(ProgrammingLanguage? obj)
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

    protected override object GetOrderByKey(ProgrammingLanguage item)
    {
        return item.Id!;
    }
}
