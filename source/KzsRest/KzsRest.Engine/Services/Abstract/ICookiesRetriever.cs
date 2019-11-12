using System;
using System.Threading;
using System.Threading.Tasks;

namespace KzsRest.Engine.Services.Abstract
{
    public interface ICookiesRetriever
    {
        Task<int?> GetSeasonId(Uri uri, CancellationToken ct = default);
    }
}
