using System;
using System.Diagnostics.CodeAnalysis;
using Tsa.Submissions.Coding.Contracts.Languages;

namespace Tsa.Submissions.Coding.UnitTests.Helpers.Languages;

[ExcludeFromCodeCoverage]
internal class ProgrammingLanguageRequestEqualityComparer : EqualityComparerBase<ProgrammingLanguageRequest>
{
    protected override bool EqualsCore(ProgrammingLanguageRequest x, ProgrammingLanguageRequest y)
    {
        var fileExtensionsMatch = x.FileExtension == y.FileExtension;
        var identifiersMatch = x.Identifier == y.Identifier;
        var isEnabledMatch = x.IsEnabled == y.IsEnabled;
        var namesMatch = x.Name == y.Name;

        return fileExtensionsMatch &&
               identifiersMatch &&
               isEnabledMatch &&
               namesMatch;
    }

    public override int GetHashCode(ProgrammingLanguageRequest? obj)
    {
        if (obj == null) return 0;

        var hashCode = new HashCode();
        hashCode.Add(obj.FileExtension);
        hashCode.Add(obj.Identifier);
        hashCode.Add(obj.IsEnabled);
        hashCode.Add(obj.Name);

        return hashCode.ToHashCode();
    }

    protected override Func<ProgrammingLanguageRequest, bool> GetItemPredicate(ProgrammingLanguageRequest item)
    {
        return programmingLanguageRequest => programmingLanguageRequest.Identifier == item.Identifier;
    }

    protected override object GetOrderByKey(ProgrammingLanguageRequest item)
    {
        return item.Identifier;
    }
}
