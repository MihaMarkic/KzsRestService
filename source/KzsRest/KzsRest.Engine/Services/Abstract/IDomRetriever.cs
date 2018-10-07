using System.Threading;
using System.Threading.Tasks;

namespace KzsRest.Engine.Services.Abstract
{
    public interface IDomRetriever
    {
        Task<string> GetDomForAsync(string relativeAddress, CancellationToken ct);
    }
}
