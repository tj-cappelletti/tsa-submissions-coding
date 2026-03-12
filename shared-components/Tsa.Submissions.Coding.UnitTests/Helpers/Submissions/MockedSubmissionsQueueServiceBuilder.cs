using k8s.Models;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tsa.Submissions.Coding.Contracts.Messages;
using Tsa.Submissions.Coding.UnitTests.Helpers.Mocks;
using Tsa.Submissions.Coding.WebApi.Services;

namespace Tsa.Submissions.Coding.UnitTests.Helpers.Submissions;
internal class MockedSubmissionsQueueServiceBuilder: IMockBuilder<ISubmissionsQueueService>
{
    private readonly Mock<ISubmissionsQueueService> _mock = new();

    /// <summary>
    ///     Builds and returns the configured mock instance.
    /// </summary>
    public Mock<ISubmissionsQueueService> Build()
    {
        return _mock;
    }

    /// <summary>
    ///     Builds and returns the configured mock's object.
    /// </summary>
    public ISubmissionsQueueService BuildObject()
    {
        return _mock.Object;
    }

    public MockedSubmissionsQueueServiceBuilder WithEnqueueSubmissionAsync(SubmissionMessage expectedMessage, Times? times = null)
    {
        _mock
            .Setup(service => service.EnqueueSubmissionAsync(It.Is(expectedMessage, new SubmissionMessageEqualityComparer()), default))
            .Returns(Task.CompletedTask)
            .Verifiable(times ?? Times.Once());

        return this;
    }
}
