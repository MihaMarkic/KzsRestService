using KzsRest.Services.Abstract;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace KzsRest.Services.Implementation
{
    public class CacheService : ICacheService
    {
        readonly IMemoryCache cache;
        public CacheService(IMemoryCache cache)
        {
            this.cache = cache;
        }
        public async ValueTask<TResult> GetDataAsync<TKey, TResult>(TKey key, TimeSpan span, Func<TKey, CancellationToken, Task<TResult>> creator, CancellationToken ct)
        {
            if (!cache.TryGetValue<TResult>(key, out var result))
            {
                result = await creator(key, ct);
                cache.Set(key, result, span);
            }
            return result;
        }
    }
}
