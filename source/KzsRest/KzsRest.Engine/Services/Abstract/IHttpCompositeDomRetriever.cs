using System.Threading;
using System.Threading.Tasks;
using HtmlAgilityPack;
using KzsRest.Engine.Models;

namespace KzsRest.Engine.Services.Abstract
{
    public interface IHttpCompositeDomRetriever
    {
        Task<TeamHtmlData> GetTeamDataAsync(int teamId, int seasonId, CancellationToken ct);
        Task<HtmlDocument> GetLeagueFixturesAsync(int seasonId, CancellationToken ct);
        Task<HtmlDocument> GetLeagueResultsAsync(int seasonId, CancellationToken ct);
        Task<HtmlDocument> GetLeagueStandingsAsync(int seasonId, CancellationToken ct);
        Task<string> GetSeasonHtmlAsync(int leagueId, CancellationToken ct);
    }
}