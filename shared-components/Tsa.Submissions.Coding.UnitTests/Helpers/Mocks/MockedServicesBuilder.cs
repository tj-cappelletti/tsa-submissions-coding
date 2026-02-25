using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using Tsa.Submissions.Coding.WebApi.Entities;
using Tsa.Submissions.Coding.WebApi.Services;

namespace Tsa.Submissions.Coding.UnitTests.Helpers.Mocks;

/// <summary>
///     Fluent builder for creating mocked service instances with pre-configured behaviors.
/// </summary>
[ExcludeFromCodeCoverage]
internal abstract class MockedServiceBuilder<TService, TEntity>
    where TService : class, IMongoEntityService<TEntity>
    where TEntity : IMongoDbEntity, new()
{
    private readonly Mock<TService> _mock = new();

    /// <summary>
    ///     Builds and returns the configured mock instance.
    /// </summary>
    public Mock<TService> Build()
    {
        return _mock;
    }

    /// <summary>
    ///     Builds and returns the configured mock's object.
    /// </summary>
    public TService BuildObject()
    {
        return _mock.Object;
    }

    /// <summary>
    ///     Configures CreateAsync to create an entity with the specified ID.
    /// </summary>
    public abstract MockedServiceBuilder<TService, TEntity> WithCreateAsync(
        TEntity expectedEntity,
        string newId,
        Times? times = null);

    /// <summary>
    ///     Configures CreateAsync to create an entity with the specified ID.
    /// </summary>
    protected MockedServiceBuilder<TService, TEntity> WithCreateAsync(
        TEntity expectedEntity,
        string newId,
        IEqualityComparer<TEntity> equalityComparer,
        Times? times = null)
    {
        _mock
            .Setup(service => service.CreateAsync(It.Is(expectedEntity, equalityComparer), default))
            .Callback<TEntity, CancellationToken>((entity, _) => entity.Id = newId)
            .Returns(Task.CompletedTask)
            .Verifiable(times ?? Times.Once());

        return this;
    }

    /// <summary>
    ///     Configures GetAsync(string id) to return a specific entity.
    /// </summary>
    public MockedServiceBuilder<TService, TEntity> WithGetAsync(
        string id,
        TEntity? returnValue,
        Times? times = null)
    {
        _mock
            .Setup(service => service.GetAsync(It.Is(id, new StringEqualityComparer()), default))
            .ReturnsAsync(returnValue)
            .Verifiable(times ?? Times.Once());

        return this;
    }

    /// <summary>
    ///     Configures GetAsync() to return a list of entities.
    /// </summary>
    public MockedServiceBuilder<TService, TEntity> WithGetAsync(
        List<TEntity> returnValue,
        Times? times = null)
    {
        _mock
            .Setup(service => service.GetAsync(default))
            .ReturnsAsync(returnValue)
            .Verifiable(times ?? Times.Once());

        return this;
    }

    public abstract MockedServiceBuilder<TService, TEntity> WithRemoveAsync(
        TEntity entity,
        Times? times = null);

    /// <summary>
    ///     Configures RemoveAsync for a specific entity.
    /// </summary>
    protected MockedServiceBuilder<TService, TEntity> WithRemoveAsync(
        TEntity entity,
        IEqualityComparer<TEntity> equalityComparer,
        Times? times = null)
    {
        _mock
            .Setup(service => service.RemoveAsync(It.Is(entity, equalityComparer), default))
            .Returns(Task.CompletedTask)
            .Verifiable(times ?? Times.Once());

        return this;
    }

    public abstract MockedServiceBuilder<TService, TEntity> WithUpdateAsync(
        TEntity expectedEntity,
        Times? times = null);

    /// <summary>
    ///     Configures UpdateAsync to update a specific entity.
    /// </summary>
    protected MockedServiceBuilder<TService, TEntity> WithUpdateAsync(
        TEntity expectedEntity,
        IEqualityComparer<TEntity> equalityComparer,
        Times? times = null)
    {
        _mock
            .Setup(service => service.UpdateAsync(It.Is(expectedEntity, equalityComparer), default))
            .Returns(Task.CompletedTask)
            .Verifiable(times ?? Times.Once());

        return this;
    }
}
