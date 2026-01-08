using System.Threading;
using System.Threading.Tasks;

namespace Tsa.Submissions.Coding.WebApi.Services;

public interface IPingableService
{
    string ServiceName { get; }

    Task<bool> PingAsync(CancellationToken cancellationToken = default);
}
