using System.Threading;
using System.Threading.Tasks;
using KzsRest.Engine.Models;

namespace KzsRest.Engine.Services.Abstract
{
    public interface IHttpCompositeDomRetriever
    {
        Task<TeamHtmlData> GetTeamDataAsync(int teamId, int seasonId, CancellationToken ct);
    }
}