using Flurl;
using HtmlAgilityPack;
using KzsRest.Engine.Models;
using KzsRest.Engine.Services.Abstract;
using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace KzsRest.Engine.Services.Implementation
{
    /// <summary>
    /// Converts raw HTML data to HTMLDocument
    /// </summary>
    public class HttpCompositeDomRetriever : IHttpCompositeDomRetriever
    {
        const string Api = "39f56437f972dc4ca91d2c997f874dcfc232a688";
        const string PlayersState = "IDgxMzIxMDA5NmE6MjI6e3M6MjA6InNob3dfc2Vhc29uX3NlbGVjdG9yIjtzOjE6IjEiO3M6MTc6InRlYW1fbGlua192aXNpYmxlIjtzOjE6IjEiO3M6MTg6ImFyZW5hX2xpbmtfdmlzaWJsZSI7czoxOiIxIjtzOjE1OiJhcmVuYV9saW5rX3R5cGUiO3M6MToiMyI7czoxODoiYXJlbmFfbGlua19oYW5kbGVyIjtzOjEzOiJuYXZpZ2F0ZUFyZW5hIjtzOjE3OiJnYW1lX2xpbmtfdmlzaWJsZSI7czoxOiIxIjtzOjE5OiJwbGF5ZXJfbGlua192aXNpYmxlIjtzOjE6IjEiO3M6MTQ6InRlYW1fbGlua190eXBlIjtzOjE6IjMiO3M6MTQ6ImdhbWVfbGlua190eXBlIjtzOjE6IjMiO3M6MTY6InBsYXllcl9saW5rX3R5cGUiO3M6MToiMyI7czoxNzoidGVhbV9saW5rX2hhbmRsZXIiO3M6MTI6Im5hdmlnYXRlVGVhbSI7czoxNzoiZ2FtZV9saW5rX2hhbmRsZXIiO3M6MTI6Im5hdmlnYXRlR2FtZSI7czoxOToicGxheWVyX2xpbmtfaGFuZGxlciI7czoxNDoibmF2aWdhdGVQbGF5ZXIiO3M6OToic2Vhc29uX2lkIjtzOjY6IjExMDMxMSI7czo3OiJ0ZWFtX2lkIjtzOjc6IjQ1OTY4MDEiO3M6MTM6InNob3dfZ2FtZV9kYXkiO3M6MToiMSI7czoxODoic2hvd19yb3N0ZXJfd2VpZ2h0IjtzOjE6IjAiO3M6MjE6InNob3dfcm9zdGVyX2JpcnRoZGF0ZSI7czoxOiIwIjtzOjI3OiJzaG93X3Jvc3Rlcl9wbGFjZV9vZl9iaXJkdGgiO3M6MToiMCI7czoyMjoic2hvd19yb3N0ZXJfcGxheXNfZnJvbSI7czoxOiIwIjtzOjIyOiJzaG93X3Jvc3Rlcl9iaXJ0aF95ZWFyIjtzOjE6IjEiO3M6ODoidGVtcGxhdGUiO3M6MjoidjIiO30=";
        const string BaseAddress = "https://widgets.baskethotel.com/widget-service/show";
        readonly ICommunicator communicator;
        public HttpCompositeDomRetriever(ICommunicator communicator)
        {
            this.communicator = communicator;
        }
        public async Task<TeamHtmlData> GetTeamDataAsync(int teamId, int seasonId, CancellationToken ct)
        {
            var playersTask = communicator.GetResponseAsync(Url.Combine(BaseAddress, $"?api={Api}",
                "lang=sl&nnav=1&nav_object=0&flash=0&request[0][container]=33-200-tab-container&request[0][widget]=200",
                $"request[0][part]=roster&request[0][state]={PlayersState}&request[0][param][team_id]={teamId}&request[0][param][season_id]={seasonId}"), ct);
            string resultsAndFixturesUrl = Url.Combine(BaseAddress, $"?api={Api}",
                $"lang=sl&nnav=1&nav_object=0&hide_full_birth_date=0&flash=0&request[0][container]=33-200-tab-container&request[0][widget]=200",
                $"request[0][part]=schedule_and_results&request[0][state]={PlayersState}&request[0][param][team_id]={teamId}",
                $"request[0][param][season_id]={seasonId}&request[0][param][group_id]=0&request[0][param][month]=all");
            var resultsAndFixturesTask = communicator.GetResponseAsync(resultsAndFixturesUrl, ct); ;
            var info = await communicator.GetResponseAsync(Url.Combine(BaseAddress, $"?api={Api}",
                $"lang=sl&nnav=1&nav_object=0&flash=0&request[0][param][season_id]={seasonId}",
                $"request[0][container]=33-200-tab-container&request[0][widget]=200&request[0][part]=home&request[0][state]={PlayersState}",
                $"request[0][param][team_id]={teamId}"), ct);
            var infoHtml = new HtmlDocument().LoadHtmlFrom(ExtractDom(info));
            var players = await playersTask;
            var playersHtml = new HtmlDocument().LoadHtmlFrom(ExtractDom(players));
            var resultsAndFixtures = await resultsAndFixturesTask;
            var resultsAndFixturesHtml = new HtmlDocument().LoadHtmlFrom(ExtractDom(resultsAndFixtures));
            return new TeamHtmlData(info: infoHtml.DocumentNode,
                lastResults: resultsAndFixturesHtml.DocumentNode,
                fixtures: resultsAndFixturesHtml.DocumentNode,
                players: playersHtml.DocumentNode);
        }

        internal static string ExtractDom(string source)
        {
            int startIndex = source.IndexOf('<');
            int lastIndex = source.LastIndexOf('>');
            return ReplaceSpecialCharacters(source.Substring(startIndex, lastIndex - startIndex + 1));
        }

        internal static string ReplaceSpecialCharacters(string source)
        {
            return source.Replace(@"\n", "").Replace(@"\r", "").Replace(@"\\", @"\").Replace(@"\/", "/").Replace(@"\""", @"""");
        }
    }
}
