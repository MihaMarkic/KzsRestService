using System;
using System.Threading;
using System.Threading.Tasks;

namespace KzsRest.Engine.Services.Abstract
{
    public interface IDomCache
    {
        ValueTask<string> GetDomForAsync(string relativeAddress, TimeSpan duration, CancellationToken ct);
    }
}
