using System.Diagnostics.CodeAnalysis;
using Moq;
using Tsa.Submissions.Coding.UnitTests.Helpers.Mocks;
using Tsa.Submissions.Coding.WebApi.Entities;
using Tsa.Submissions.Coding.WebApi.Services;

namespace Tsa.Submissions.Coding.UnitTests.Helpers.Problems;

[ExcludeFromCodeCoverage]
internal class MockedProblemsServiceBuilder : MockedServiceBuilder<IProblemsService, Problem>
{
    public override MockedServiceBuilder<IProblemsService, Problem> WithCreateAsync(Problem expectedEntity, string newId, Times? times = null)
    {
        WithCreateAsync(expectedEntity, newId, new ProblemEqualityComparer(), times);

        return this;
    }

    public override MockedServiceBuilder<IProblemsService, Problem> WithRemoveAsync(Problem expectedEntity, Times? times = null)
    {
        WithRemoveAsync(expectedEntity, new ProblemEqualityComparer(), times);

        return this;
    }

    public override MockedServiceBuilder<IProblemsService, Problem> WithUpdateAsync(Problem expectedEntity, Times? times = null)
    {
        WithUpdateAsync(expectedEntity, new ProblemEqualityComparer(), times);

        return this;
    }
}
