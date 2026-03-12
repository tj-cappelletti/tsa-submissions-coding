using Moq;

namespace Tsa.Submissions.Coding.UnitTests.Helpers.Mocks;

internal interface IMockBuilder<T> where T : class
{
    Mock<T> Build();

    T BuildObject();
}
