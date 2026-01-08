using System.Threading;
using System.Threading.Tasks;
using Tsa.Submissions.Coding.WebApi.Entities;

namespace Tsa.Submissions.Coding.WebApi.Services;

public interface IUsersService : IMongoEntityService<User>, IPingableService
{
    Task<User?> GetByUserNameAsync(string? userName, CancellationToken cancellationToken = default);
}
