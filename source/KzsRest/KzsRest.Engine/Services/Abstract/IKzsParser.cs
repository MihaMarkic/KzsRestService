using KzsRest.Engine.Keys;
using KzsRest.Models;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace KzsRest.Engine.Services.Abstract
{
    public interface IKzsParser
    {
        Task<LeagueOverview> GetLeagueOverviewAsync(int leagueId, CancellationToken ct);
        ValueTask<KzsLeagues> GetTopLevelAsync(CancellationToken ct);
        Task<Team> GetTeamAsync(TeamKey key, CancellationToken ct);
    }
}
