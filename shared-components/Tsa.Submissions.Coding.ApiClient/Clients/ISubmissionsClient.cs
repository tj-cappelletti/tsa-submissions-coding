using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tsa.Submissions.Coding.Contracts.Submissions;

namespace Tsa.Submissions.Coding.ApiClient.Clients;
public interface ISubmissionsClient
{
    Task<SubmissionResponse> GetAsync(string id, CancellationToken cancellationToken);
}
