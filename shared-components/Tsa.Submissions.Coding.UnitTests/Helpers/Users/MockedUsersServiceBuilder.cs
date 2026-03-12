using System.Diagnostics.CodeAnalysis;
using Moq;
using Tsa.Submissions.Coding.UnitTests.Helpers.Mocks;
using Tsa.Submissions.Coding.WebApi.Entities;
using Tsa.Submissions.Coding.WebApi.Services;

namespace Tsa.Submissions.Coding.UnitTests.Helpers.Users;

[ExcludeFromCodeCoverage]
internal class MockedUsersServiceBuilder : MockedServiceBuilder<IUsersService, User>
{
    public override MockedServiceBuilder<IUsersService, User> WithCreateAsync(User expectedEntity, string newId, Times? times = null)
    {
        WithCreateAsync(expectedEntity, newId, new UserEqualityComparer(), times);

        return this;
    }

    public override MockedServiceBuilder<IUsersService, User> WithRemoveAsync(User expectedEntity, Times? times = null)
    {
        WithRemoveAsync(expectedEntity, new UserEqualityComparer(), times);

        return this;
    }

    public override MockedServiceBuilder<IUsersService, User> WithUpdateAsync(User expectedEntity, Times? times = null)
    {
        WithUpdateAsync(expectedEntity, new UserEqualityComparer(), times);

        return this;
    }
}
