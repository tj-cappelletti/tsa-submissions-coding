using Tsa.Submissions.Coding.WebApi.Entities;

namespace Tsa.Submissions.Coding.WebApi.Services;

public interface IProblemsService : IMongoEntityService<Problem>, IPingableService { }
