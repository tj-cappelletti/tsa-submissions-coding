namespace Tsa.Submissions.Coding.Contracts.Pagination;

public record PaginatedResponse<T>
{
    public bool HasNextPage { get; init; }

    public IList<T> Items { get; init; }

    public string? NextCursor { get; init; }

    public int PageSize { get; init; }

    public PaginatedResponse(IList<T> items, int pageSize, bool hasNextPage, string? nextCursor)
    {
        HasNextPage = hasNextPage;
        Items = items;
        NextCursor = nextCursor;
        PageSize = pageSize;
    }
}
