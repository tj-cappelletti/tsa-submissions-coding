using System.Threading;
using System.Threading.Tasks;
using Tsa.Submissions.Coding.Contracts.Messages;

namespace Tsa.Submissions.Coding.WebApi.Services;

public interface ISubmissionsQueueService
{
    Task EnqueueSubmissionAsync(SubmissionMessage submissionMessage, CancellationToken cancellationToken = default);
}
