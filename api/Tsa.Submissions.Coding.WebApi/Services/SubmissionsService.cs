using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using Tsa.Submissions.Coding.WebApi.Configuration;
using Tsa.Submissions.Coding.WebApi.Entities;
using Tsa.Submissions.Coding.WebApi.Pagination;

namespace Tsa.Submissions.Coding.WebApi.Services;

public class SubmissionsService : MongoDbService<Submission>, ISubmissionsService
{
    public const string MongoDbCollectionName = "submissions";

    private readonly Func<PaginationSortOrder, SortDefinition<Submission>> _sortById = order =>
        order == PaginationSortOrder.Descending
            ? Builders<Submission>.Sort.Descending(s => s.Id)
            : Builders<Submission>.Sort.Ascending(s => s.Id);

    private readonly Func<PaginationSortOrder, SortDefinition<Submission>> _sortBySubmittedOn = order =>
        order == PaginationSortOrder.Descending
            ? Builders<Submission>.Sort.Descending(s => s.SubmittedOn)
            : Builders<Submission>.Sort.Ascending(s => s.SubmittedOn);

    public string CollectionName => MongoDbCollectionName;

    public string ServiceName => "Submissions";

    public SubmissionsService(
        ICacheService cacheService,
        IMongoClient mongoClient,
        IOptions<SubmissionsDatabase> options,
        ILogger<SubmissionsService> logger)
        : base(
            cacheService,
            mongoClient,
            options.Value.Name!,
            MongoDbCollectionName,
            logger) { }

    public async Task<PagedResult<Submission>> GetPagedByIdCursorAsync(
        CursorPagination pagination,
        CancellationToken cancellationToken = default)
    {
        var filterBuilder = Builders<Submission>.Filter;
        var filter = filterBuilder.Empty;

        // If cursor provided, only get submissions older than the cursor (pagination)
        if (!string.IsNullOrEmpty(pagination.Cursor))
        {
            filter &= PaginationFilterById(pagination.Cursor, pagination.SortOrder);
        }

        return await GetPagedResult(
            filter,
            pagination,
            _sortById,
            cancellationToken);
    }

    public async Task<PagedResult<Submission>> GetPagedByProblemIdCursorAsync(
        string problemId,
        CursorPagination pagination,
        CancellationToken cancellationToken = default)
    {
        var filterBuilder = Builders<Submission>.Filter;
        var filter = filterBuilder.Eq(s => s.ProblemId, problemId);

        // If cursor provided, only get submissions older than the cursor (pagination)
        if (!string.IsNullOrEmpty(pagination.Cursor))
        {
            filter &= PaginationFilterById(pagination.Cursor, pagination.SortOrder);
        }

        return await GetPagedResult(
            filter,
            pagination,
            _sortById,
            cancellationToken);
    }

    public async Task<PagedResult<Submission>> GetPagedByUserIdCursorAsync(
        string userId,
        CursorPagination pagination,
        CancellationToken cancellationToken = default)
    {
        var filterBuilder = Builders<Submission>.Filter;
        var filter = filterBuilder.Eq(s => s.UserId, userId);

        // If cursor provided, only get submissions older than the cursor (pagination)
        if (!string.IsNullOrEmpty(pagination.Cursor))
        {
            filter &= PaginationFilterById(pagination.Cursor, pagination.SortOrder);
        }

        // NOTE: Sorting by SubmittedOn with Id-based cursor has a theoretical edge case:
        // If two submissions have identical SubmittedOn timestamps AND the second is inserted
        // before the first, pagination could be inconsistent. However, this is an acceptable
        // risk because:
        // 1. Single-computer constraint enforced by controller (2-3 second minimum between submissions)
        // 2. Would require sub-millisecond timing + out-of-order database writes
        // 3. Impact is minor (temporary pagination inconsistency, not data loss)
        // 4. Controller-level rate limiting is the primary defense against this scenario
        return await GetPagedResult(
            filter,
            pagination,
            _sortBySubmittedOn,
            cancellationToken);
    }

    private async Task<PagedResult<Submission>> GetPagedResult(
        FilterDefinition<Submission> filter,
        CursorPagination pagination,
        Func<PaginationSortOrder, SortDefinition<Submission>> sort,
        CancellationToken cancellationToken = default)
    {
        var items = await EntityCollection
            .Find(filter)
            .Sort(sort(pagination.SortOrder))
            .Limit(pagination.PageSize + 1)
            .ToListAsync(cancellationToken);

        var hasNextPage = items.Count > pagination.PageSize;

        string? nextCursor = null;

        if (hasNextPage)
        {
            nextCursor = items[^1].Id;

            items.RemoveAt(items.Count - 1);
        }

        return new PagedResult<Submission>
        {
            Items = items,
            PageSize = pagination.PageSize,
            NextCursor = nextCursor,
            HasNextPage = hasNextPage
        };
    }

    private static FilterDefinition<Submission> PaginationFilterById(string id, PaginationSortOrder sortOrder)
    {
        return sortOrder == PaginationSortOrder.Descending
            ? Builders<Submission>.Filter.Lte(s => s.Id, id)
            : Builders<Submission>.Filter.Gte(s => s.Id, id);
    }
}
