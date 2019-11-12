using Flurl;
using HtmlAgilityPack;
using KzsRest.Engine.Models;
using KzsRest.Engine.Services.Abstract;
using System;
using System.Net.Http;
using System.Text.RegularExpressions;
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
        const string LeagueStandingsState = "IDMyMzY1MTMxMWE6MTI6e3M6MTk6ImxlYWd1ZV9saW5rX3Zpc2libGUiO3M6MToiMiI7czoxNzoidGVhbV9saW5rX3Zpc2libGUiO3M6MToiMSI7czoxNzoiZ2FtZV9saW5rX3Zpc2libGUiO3M6MToiMiI7czoxOToicGxheWVyX2xpbmtfdmlzaWJsZSI7czoxOiIxIjtzOjI3OiJmdWxsX3N0YW5kaW5nc19saW5rX3Zpc2libGUiO3M6MToiMiI7czoyMDoic3RhZ2VfbGV2ZWxzX3Zpc2libGUiO3M6MToiMiI7czo5OiJsZWFndWVfaWQiO3M6NToiMTAyOTMiO3M6MTQ6InRlYW1fbGlua190eXBlIjtzOjE6IjMiO3M6MTc6InRlYW1fbGlua19oYW5kbGVyIjtzOjEyOiJuYXZpZ2F0ZVRlYW0iO3M6MjQ6ImZ1bGxfc3RhbmRpbmdzX2xpbmtfdHlwZSI7czoxOiIzIjtzOjI3OiJmdWxsX3N0YW5kaW5nc19saW5rX2hhbmRsZXIiO3M6MTc6Im5hdmlnYXRlU3RhbmRpbmdzIjtzOjg6InRlbXBsYXRlIjtzOjI6InYyIjt9";
        const string LeagueFixturesAndResultsState = "MTQ3MjUxMDYyM2E6MTk6e3M6MTc6InRlYW1fbGlua192aXNpYmxlIjtzOjE6IjEiO3M6MjA6InNob3dfc2Vhc29uX3NlbGVjdG9yIjtzOjE6IjEiO3M6MTc6ImdhbWVfbGlua192aXNpYmxlIjtzOjE6IjEiO3M6MTk6InBsYXllcl9saW5rX3Zpc2libGUiO3M6MToiMSI7czoxNDoidGVhbV9saW5rX3R5cGUiO3M6MToiMyI7czoxNDoiZ2FtZV9saW5rX3R5cGUiO3M6MToiMyI7czoxNjoicGxheWVyX2xpbmtfdHlwZSI7czoxOiIzIjtzOjE3OiJ0ZWFtX2xpbmtfaGFuZGxlciI7czoxMjoibmF2aWdhdGVUZWFtIjtzOjE3OiJnYW1lX2xpbmtfaGFuZGxlciI7czoxMjoibmF2aWdhdGVHYW1lIjtzOjE5OiJwbGF5ZXJfbGlua19oYW5kbGVyIjtzOjE0OiJuYXZpZ2F0ZVBsYXllciI7czoxODoiYXJlbmFfbGlua192aXNpYmxlIjtzOjE6IjEiO3M6MTU6ImFyZW5hX2xpbmtfdHlwZSI7czoxOiIzIjtzOjE4OiJhcmVuYV9saW5rX2hhbmRsZXIiO3M6MTM6Im5hdmlnYXRlQXJlbmEiO3M6OToibGVhZ3VlX2lkIjtzOjU6IjEwMjkzIjtzOjk6ImdhbWVfZGF5cyI7czoxOiIxIjtzOjE3OiJzaG93X3JlZmVyZWVfY2l0eSI7czoxOiIxIjtzOjEzOiJpdGVtc19vbl9wYWdlIjtzOjI6IjMwIjtzOjIwOiJzdGFnZV9sZXZlbHNfdmlzaWJsZSI7czoxOiIyIjtzOjg6InRlbXBsYXRlIjtzOjI6InYyIjt9";
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

        public async Task<HtmlDocument> GetLeagueFixturesAsync(int seasonId, CancellationToken ct)
        {
            string url = Url.Combine(BaseAddress, $"?api={Api}",
                "lang=sl&nnav=1&nav_object=0&hide_full_birth_date=0&flash=0&request[0][container]=33-303-container&request[0][widget]=303",
                $"request[0][part]=schedule_and_results&request[0][state]={LeagueFixturesAndResultsState}&request[0][param][filter][season]={seasonId}",
                "request[0][param][filter][type]=schedule_only&request[0][param][filter][tab]=schedule_and_results");

            var info = await communicator.GetResponseAsync(url, ct);
            return new HtmlDocument().LoadHtmlFrom(ExtractDom(info));
        }
        public async Task<HtmlDocument> GetLeagueResultsAsync(int seasonId, CancellationToken ct)
        {
            string url = Url.Combine(BaseAddress, $"?api={Api}",
                "lang=sl&nnav=1&nav_object=0&hide_full_birth_date=0&flash=0&request[0][container]=33-303-container&request[0][widget]=303",
                $"request[0][part]=schedule_and_results&request[0][state]={LeagueFixturesAndResultsState}&request[0][param][filter][season]={seasonId}",
                "request[0][param][filter][type]=results_only&request[0][param][filter][tab]=schedule_and_results");

            var info = await communicator.GetResponseAsync(url, ct);
            return new HtmlDocument().LoadHtmlFrom(ExtractDom(info));
        }

        public async Task<HtmlDocument> GetLeagueStandingsAsync(int seasonId, CancellationToken ct)
        {
            string url = Url.Combine(BaseAddress, $"?api={Api}", 
                "lang=sl&nnav=1&nav_object=0&hide_full_birth_date=0&flash=0&request[0][container]=33-301-standings-container&request[0][widget]=301",
                $"request[0][part]=table&request[0][state]={LeagueStandingsState}&request[0][param][season_id]={seasonId}&request[0][param][showTeamLogo]=",
                "request[0][param][teamLogoSize]=20x20");

            var info = await communicator.GetResponseAsync(url, ct);
            return new HtmlDocument().LoadHtmlFrom(ExtractDom(info));
        }

        public async Task<string> GetSeasonHtmlAsync(int leagueId, CancellationToken ct)
        {
            string url = Url.Combine(BaseAddress, $"?api={Api}",
                "nnav=1&nav_object=0&hide_full_birth_date=0&flash=0&request[0][container]=stats-team&request[0][widget]=601&request[0][param][league_link_visible]=1",
                "request[0][param][team_link_visible]=1&request[0][param][game_link_visible]=1&request[0][param][player_link_visible]=1",
                $"request[0][param][league_id]={leagueId}&request[0][param][game_link_type]=3&request[0][param][game_link_handler]=navigateGame",
                "request[0][param][team_link_type]=3&request[0][param][team_link_handler]=navigateTeam&request[0][param][player_link_type]=3",
                "request[0][param][player_link_handler]=navigatePlayer&request[0][param][template]=v2"
                );

            var html = await communicator.GetResponseAsync(url, ct);
            return html;
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
