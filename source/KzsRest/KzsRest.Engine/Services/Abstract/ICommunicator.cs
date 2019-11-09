using System.Threading;
using System.Threading.Tasks;

namespace KzsRest.Engine.Services.Abstract
{
    public interface ICommunicator
    {
        Task<string> GetResponseAsync(string uri, CancellationToken ct);
    }
}