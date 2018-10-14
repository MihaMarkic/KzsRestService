using KzsRest.Engine.Keys;
using KzsRest.Models;
using System.Threading;
using System.Threading.Tasks;

namespace KzsRest.Engine.Services.Abstract
{
    public interface IKzsParser
    {
        Task<Standings[]> GetStandingsAsync(string address, CancellationToken ct);
        ValueTask<KzsLeagues> GetTopLevelAsync(CancellationToken ct);
        Task<Team> GetTeamAsync(TeamKey key, CancellationToken ct);
    }
}
