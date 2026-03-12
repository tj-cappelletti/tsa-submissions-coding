using System;
using System.Diagnostics.CodeAnalysis;

namespace Tsa.Submissions.Coding.UnitTests.Helpers;

[ExcludeFromCodeCoverage]
internal class StringEqualityComparer : EqualityComparerBase<string>
{
    protected override bool EqualsCore(string x, string y)
    {
        return x == y;
    }

    public override int GetHashCode(string? obj)
    {
        return obj == null
            ? 0
            : obj.GetHashCode(StringComparison.CurrentCulture);
    }

    protected override Func<string, bool> GetItemPredicate(string item)
    {
        return @string => @string == item;
    }

    protected override object GetOrderByKey(string item)
    {
        return item;
    }
}
