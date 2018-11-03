using System;
using System.Threading;
using System.Threading.Tasks;

namespace KzsRest.Engine.Services.Abstract
{
    public interface IDomSource
    {
        Task<string> GetHtmlContentAsync(string relativeAddress, CancellationToken ct, params string[] args);
    }
}
