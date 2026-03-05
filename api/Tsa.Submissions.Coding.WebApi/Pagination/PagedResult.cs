using System.Collections.Generic;

namespace Tsa.Submissions.Coding.WebApi.Pagination;

public class PagedResult<T>
{
    public bool HasNextPage { get; set; }

    public IList<T> Items { get; set; } = new List<T>();

    public string? NextCursor { get; set; }

    public int PageSize { get; set; }
}
