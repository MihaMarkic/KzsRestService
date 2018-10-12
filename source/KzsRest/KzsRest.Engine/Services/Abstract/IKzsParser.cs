using KzsRest.Engine.Models;
using System.Threading;
using System.Threading.Tasks;

namespace KzsRest.Engine.Services.Abstract
{
    public interface IKzsParser
    {
        ValueTask<Standings[]> GetStandingsAsync(string address, CancellationToken ct);
        ValueTask<KzsLeagues> GetTopLevelAsync(CancellationToken ct);
    }
}
