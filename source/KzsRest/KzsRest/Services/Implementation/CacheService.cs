using KzsRest.Engine.MetricsExtensions;
using KzsRest.Services.Abstract;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace KzsRest.Services.Implementation
{
    public class CacheService : ICacheService
    {
        readonly IMemoryCache cache;
        readonly IHttpContextAccessor httpContextAccessor;
        public CacheService(IMemoryCache cache, IHttpContextAccessor httpContextAccessor)
        {
            this.cache = cache;
            this.httpContextAccessor = httpContextAccessor;
        }
        public async ValueTask<TResult> GetDataAsync<TKey, TResult>(TKey key, TimeSpan span, Func<TKey, CancellationToken, Task<TResult>> creator, CancellationToken ct)
        {
            if (!cache.TryGetValue<TResult>(key, out var result))
            {
                result = await creator(key, ct);
                cache.Set(key, result, span);
            }
            else
            {
                AppMetrics.CacheHits.Labels(httpContextAccessor.HttpContext.Request.Path).Inc();
            }
            return result;
        }
    }
}
