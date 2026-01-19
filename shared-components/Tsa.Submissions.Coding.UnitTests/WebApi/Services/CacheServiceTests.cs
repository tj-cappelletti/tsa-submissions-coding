using System;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Distributed;
using Moq;
using Newtonsoft.Json;
using Tsa.Submissions.Coding.WebApi.Services;
using Xunit;

namespace Tsa.Submissions.Coding.UnitTests.WebApi.Services;

[ExcludeFromCodeCoverage]
public class CacheServiceTests
{
    [Fact]
    [Trait("TestCategory", "UnitTest")]
    public async Task GetAsync_Should_Return_Null_When_Key_Does_Not_Exist()
    {
        // Arrange
        var cache = new Mock<IDistributedCache>();

        var cacheService = new CacheService(cache.Object);

        const string key = "nonexistent_key";

        cache.Setup(distributedCache => distributedCache.GetAsync(key, It.IsAny<CancellationToken>()))
            .ReturnsAsync((byte[]?)null);

        // Act
        var result = await cacheService.GetAsync<string>(key);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    [Trait("TestCategory", "UnitTest")]
    public async Task GetAsync_Should_Return_Value_When_Key_Exists()
    {
        // Arrange
        var cache = new Mock<IDistributedCache>();

        var cacheService = new CacheService(cache.Object);

        const string key = "existing_key";
        const string expectedValue = "cached_value";
        var expectedValueAsBytes = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(expectedValue));

        cache.Setup(distributedCache => distributedCache.GetAsync(key, It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedValueAsBytes);

        // Act
        var result = await cacheService.GetAsync<string>(key);

        // Assert
        Assert.Equal(expectedValue, result);
    }

    [Fact]
    [Trait("TestCategory", "UnitTest")]
    public async Task RemoveAsync_Should_Remove_Value_From_Cache()
    {
        // Arrange
        var cache = new Mock<IDistributedCache>();

        var cacheService = new CacheService(cache.Object);

        const string key = "remove_key";
        const string value = "cached_value";
        var expectedValueAsBytes = Encoding.UTF8.GetBytes(value);

        cache.Setup(distributedCache => distributedCache.GetAsync(key, It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedValueAsBytes);

        // Act
        await cacheService.RemoveAsync(key);

        // Assert
        cache.Verify(distributedCache => distributedCache.RemoveAsync(key, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    [Trait("TestCategory", "UnitTest")]
    public async Task RemoveAsync_Should_Not_Throw_When_Key_Does_Not_Exist()
    {
        // Arrange
        var cache = new Mock<IDistributedCache>();

        var cacheService = new CacheService(cache.Object);

        const string key = "nonexistent_key";

        cache.Setup(distributedCache => distributedCache.GetAsync(key, It.IsAny<CancellationToken>()))
            .ReturnsAsync((byte[]?)null);

        // Act
        await cacheService.RemoveAsync(key);

        // Assert
        cache.Verify(distributedCache => distributedCache.RemoveAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    [Trait("TestCategory", "UnitTest")]
    public async Task SetAsync_Should_Set_Value_In_Cache()
    {
        // Arrange
        var cache = new Mock<IDistributedCache>();

        var cacheService = new CacheService(cache.Object);

        const string key = "set_key";
        const string value = "set_value";
        var valueAsBytes = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(value));

        // Act
        await cacheService.SetAsync(key, value);

        // Assert
        cache.Verify(
            distributedCache => distributedCache.SetAsync(key, valueAsBytes, It.IsAny<DistributedCacheEntryOptions>(), It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    [Trait("TestCategory", "UnitTest")]
    public async Task SetAsync_Should_Set_Value_In_Cache_With_Expiration()
    {
        // Arrange
        var cache = new Mock<IDistributedCache>();

        var cacheService = new CacheService(cache.Object);

        const string key = "set_key_with_expiration";
        const string value = "set_value_with_expiration";
        var valueAsBytes = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(value));

        var expiration = TimeSpan.FromMinutes(5);

        Func<DistributedCacheEntryOptions, bool> validateDistributedCacheEntryOptions =
            distributedCacheEntryOptions => distributedCacheEntryOptions.AbsoluteExpirationRelativeToNow == expiration;

        // Act
        await cacheService.SetAsync(key, value, expiration);

        // Assert
        cache.Verify(
            distributedCache => distributedCache.SetAsync(
                key,
                valueAsBytes,
                It.Is<DistributedCacheEntryOptions>(distributedCacheEntryOptions => validateDistributedCacheEntryOptions(distributedCacheEntryOptions)),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }
}
