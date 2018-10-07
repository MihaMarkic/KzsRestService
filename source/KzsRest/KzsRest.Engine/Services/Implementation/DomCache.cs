using KzsRest.Engine.Services.Abstract;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace KzsRest.Engine.Services.Implementation
{
    public class DomCache : IDomCache
    {
        readonly IDomRetriever domRetriever;
        public DomCache(IDomRetriever domRetriever)
        {
            this.domRetriever = domRetriever;
        }

        public async ValueTask<string> GetDomForAsync(string relativeAddress, TimeSpan duration, CancellationToken ct)
        {
            return await domRetriever.GetDomForAsync(relativeAddress, ct);
        }
    }
}
