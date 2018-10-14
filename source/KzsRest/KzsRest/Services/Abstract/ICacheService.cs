using System;
using System.Threading;
using System.Threading.Tasks;

namespace KzsRest.Services.Abstract
{
    public interface ICacheService
    {
        ValueTask<TResult> GetDataAsync<TKey, TResult>(TKey key, TimeSpan span, Func<TKey, CancellationToken, Task<TResult>> creator, CancellationToken ct);
    }
}
