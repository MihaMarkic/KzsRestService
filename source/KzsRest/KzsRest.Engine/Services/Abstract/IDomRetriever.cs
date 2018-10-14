using System.Threading;
using System.Threading.Tasks;

namespace KzsRest.Engine.Services.Abstract
{
    public interface IDomRetriever
    {
        Task<DomResultItem[]> GetDomForAsync(string relativeAddress, CancellationToken ct, params string[] args);
    }
}
