using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Tsa.Submissions.Coding.WebApi.Entities;

namespace Tsa.Submissions.Coding.WebApi.Services;

public interface ITestSetsService : IMongoEntityService<TestSet>, IPingableService
{
    Task<List<TestSet>> GetAsync(Problem problem, CancellationToken cancellationToken = default);
}
