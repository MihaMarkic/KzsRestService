using KzsRest.Engine.Services.Abstract;
using Microsoft.Extensions.ObjectPool;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace KzsRest.Engine.Services.Implementation
{
    public class CookiesRetriever : DisposableObject, ICookiesRetriever
    {
        readonly DefaultObjectPool<SessionRetrieverHttpClient> pool =
            new DefaultObjectPool<SessionRetrieverHttpClient>(new DefaultPooledObjectPolicy<SessionRetrieverHttpClient>());
        public async Task<int?> GetSeasonId(Uri uri, CancellationToken ct = default)
        {
            var client = pool.Get();
            try
            {
                int? sessionId = await client.GetSessionIdAsync(uri, ct);
                return sessionId;
            }
            finally
            {
                pool.Return(client);
            }
        }
    }
}
