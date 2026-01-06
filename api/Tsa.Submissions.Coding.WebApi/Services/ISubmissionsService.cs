using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Tsa.Submissions.Coding.WebApi.Entities;

namespace Tsa.Submissions.Coding.WebApi.Services;

public interface ISubmissionsService : IMongoEntityService<Submission>, IPingableService
{
    Task<List<Submission>> GetByProblemIdAsync(string problemId, CancellationToken cancellationToken = default);
}
