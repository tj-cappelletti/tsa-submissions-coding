using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Tsa.Submissions.Coding.WebApi.Services;

public class CacheService : ICacheService
{
    private readonly IDistributedCache _distributedCache;

    public CacheService(IDistributedCache distributedCache)
    {
        _distributedCache = distributedCache;
    }

    public async Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken = default)
    {
        var value = await _distributedCache.GetAsync(key, cancellationToken);

        if (value is null) return default;

        var valueAsString = Encoding.UTF8.GetString(value, 0, value.Length);

        return JsonConvert.DeserializeObject<T>(valueAsString);
    }

    public async Task RemoveAsync(string key, CancellationToken cancellationToken = default)
    {
        var value = await _distributedCache.GetAsync(key, cancellationToken);

        if (value is null) return;

        await _distributedCache.RemoveAsync(key, cancellationToken);
    }

    public async Task SetAsync<T>(string key, T value, TimeSpan? absoluteExpirationRelativeToNow = null, CancellationToken cancellationToken = default)
    {
        var options = new DistributedCacheEntryOptions();

        if (absoluteExpirationRelativeToNow.HasValue)
        {
            options.AbsoluteExpirationRelativeToNow = absoluteExpirationRelativeToNow.Value;
        }

        var serializedObject = JsonConvert.SerializeObject(value);

        var valueAsBytes = Encoding.UTF8.GetBytes(serializedObject);

        await _distributedCache.SetAsync(key, valueAsBytes, options, cancellationToken);
    }
}
