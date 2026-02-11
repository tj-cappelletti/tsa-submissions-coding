using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Tsa.Submissions.Coding.WebApi.Entities;

namespace Tsa.Submissions.Coding.WebApi.Services;

public interface ITestCasesService : IMongoEntityService<TestCase>, IPingableService
{
    string ComputeSignature(TestCase testCase);

    Task<List<TestCase>> GetByProblemAsync(Problem problem, CancellationToken cancellationToken = default);
}
