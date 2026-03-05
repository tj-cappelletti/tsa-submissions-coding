using System.Threading;
using System.Threading.Tasks;
using Tsa.Submissions.Coding.WebApi.Entities;
using Tsa.Submissions.Coding.WebApi.Pagination;

namespace Tsa.Submissions.Coding.WebApi.Services;

public interface ISubmissionsService : IMongoEntityService<Submission>, IPingableService
{
    Task<PagedResult<Submission>> GetPagedByIdCursorAsync(
        CursorPagination pagination,
        CancellationToken cancellationToken = default);

    Task<PagedResult<Submission>> GetPagedByProblemIdCursorAsync(
        string problemId,
        CursorPagination pagination,
        CancellationToken cancellationToken = default);

    Task<PagedResult<Submission>> GetPagedByUserIdCursorAsync(
        string userId,
        CursorPagination pagination,
        CancellationToken cancellationToken = default);
}
