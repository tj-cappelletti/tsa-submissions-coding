using System.Diagnostics.CodeAnalysis;
using Moq;
using Tsa.Submissions.Coding.UnitTests.Helpers.Mocks;
using Tsa.Submissions.Coding.UnitTests.Helpers.Pagination;
using Tsa.Submissions.Coding.WebApi.Entities;
using Tsa.Submissions.Coding.WebApi.Pagination;
using Tsa.Submissions.Coding.WebApi.Services;

namespace Tsa.Submissions.Coding.UnitTests.Helpers.Submissions;

[ExcludeFromCodeCoverage]
internal class MockedSubmissionsServiceBuilder : MockedServiceBuilder<ISubmissionsService, Submission>
{
    public override MockedServiceBuilder<ISubmissionsService, Submission> WithCreateAsync(Submission expectedEntity, string newId, Times? times = null)
    {
        WithCreateAsync(expectedEntity, newId, new SubmissionEqualityComparer(), times);

        return this;
    }

    public MockedServiceBuilder<ISubmissionsService, Submission> WithGetPagedByIdCursorAsync(
        CursorPagination expectedCursorPagination,
        PagedResult<Submission> returnValue,
        Times? times = null)
    {
        Mock.Setup(service => service.GetPagedByIdCursorAsync(It.Is(expectedCursorPagination, new CursorPaginationEqualityComparer()), default))
            .ReturnsAsync(returnValue)
            .Verifiable(times ?? Times.Once());

        return this;
    }

    public override MockedServiceBuilder<ISubmissionsService, Submission> WithRemoveAsync(Submission expectedEntity, Times? times = null)
    {
        WithRemoveAsync(expectedEntity, new SubmissionEqualityComparer(), times);

        return this;
    }

    public override MockedServiceBuilder<ISubmissionsService, Submission> WithUpdateAsync(Submission expectedEntity, Times? times = null)
    {
        WithUpdateAsync(expectedEntity, new SubmissionEqualityComparer(), times);

        return this;
    }
}
