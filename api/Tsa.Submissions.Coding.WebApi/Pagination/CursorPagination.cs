namespace Tsa.Submissions.Coding.WebApi.Pagination;

public class CursorPagination
{
    public string? Cursor { get; set; }

    public int PageSize { get; set; } = 20;

    public PaginationSortOrder SortOrder { get; set; } = PaginationSortOrder.Descending;
}
