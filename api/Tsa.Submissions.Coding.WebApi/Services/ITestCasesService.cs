using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Tsa.Submissions.Coding.WebApi.Entities;

namespace Tsa.Submissions.Coding.WebApi.Services;

public interface ITestCasesService : IMongoEntityService<TestCase>, IPingableService
{
    Task<List<TestCase>> GetByProblemAsync(Problem problem, CancellationToken cancellationToken = default);
    
    Task<TestCase?> GetBySignatureAsync(Problem problem, string signature, CancellationToken cancellationToken = default);

    Task<bool> SignatureExistsAsync(Problem problem, string signature, CancellationToken cancellationToken = default);
}
