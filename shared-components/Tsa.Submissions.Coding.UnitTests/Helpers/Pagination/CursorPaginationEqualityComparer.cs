using System;
using System.Diagnostics.CodeAnalysis;
using Tsa.Submissions.Coding.WebApi.Pagination;

namespace Tsa.Submissions.Coding.UnitTests.Helpers.Pagination;

[ExcludeFromCodeCoverage]
internal class CursorPaginationEqualityComparer : EqualityComparerBase<CursorPagination>
{
    protected override bool EqualsCore(CursorPagination x, CursorPagination y)
    {
        var cursorsMatch = x.Cursor == y.Cursor;
        var pageSizesMatch = x.PageSize == y.PageSize;
        var sortOrdersMatch = x.SortOrder == y.SortOrder;

        return cursorsMatch && pageSizesMatch && sortOrdersMatch;
    }

    public override int GetHashCode(CursorPagination? obj)
    {
        return obj == null
            ? 0
            : HashCode.Combine(obj.Cursor, obj.PageSize, obj.SortOrder);
    }

    protected override Func<CursorPagination, bool> GetItemPredicate(CursorPagination item)
    {
        throw new NotImplementedException("CursorPagination should not be used in a list.");
    }

    protected override object GetOrderByKey(CursorPagination item)
    {
        return (item.Cursor, item.PageSize, item.SortOrder);
    }
}
