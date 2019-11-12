using KzsRest.Engine.Keys;
using KzsRest.Engine.Services.Abstract;
using KzsRest.Models;
using Microsoft.Extensions.Logging;
using System;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;

namespace KzsRest.Engine.Services.Implementation
{
    public partial class KzsCommunicatorParser : IKzsParser
    {
        const string Root = "Root";
        readonly IHttpCompositeDomRetriever compositeDomRetriever;
        readonly ILogger<KzsCommunicatorParser> logger;
        readonly ICacheService cacheService;
        public static readonly CultureInfo SloveneCulture = new CultureInfo("sl-SI");
        public KzsCommunicatorParser(IHttpCompositeDomRetriever compositeDomRetriever, ICacheService cacheService, ILogger<KzsCommunicatorParser> logger)
        {
            this.compositeDomRetriever = compositeDomRetriever;
            this.cacheService = cacheService;
            this.logger = logger;
        }
        public async Task<LeagueOverview> GetLeagueOverviewAsync(int leagueId, CancellationToken ct)
        {
            int? seasonId = await GetSeasonIdAsync(leagueId, ct);
            var fixturesTask = compositeDomRetriever.GetLeagueFixturesAsync(seasonId.Value, ct);
            var resultsTask = compositeDomRetriever.GetLeagueResultsAsync(seasonId.Value, ct);
            var standingsHtml = await compositeDomRetriever.GetLeagueStandingsAsync(seasonId.Value, ct);

            var standings = await KzsStructureParser.GetStandingsAsync(standingsHtml.DocumentNode, ct);
            var fixturesHtml = await fixturesTask;
            var fixtures = KzsStructureParser.ExtractFixturesOrResults<GameFixture>(fixturesHtml.DocumentNode, includeResults: false, ct);
            var resultsHtml = await resultsTask;
            var results = KzsStructureParser.ExtractFixturesOrResults<GameResult>(resultsHtml.DocumentNode, includeResults: true, ct);

            return new LeagueOverview(standings, fixtures, results);

            //if (domFixturesAndStandings.Length == 1 && domResults.Length == 1)
            //{
            //    try
            //    {
            //        var leagueOverviewTask = ExtractStandingsAndFixturesAsync(domFixturesAndStandings[0], areStandingRequired, ct);
            //        var resultsTask = ExtractLeagueGameResultsAsync(domResults[0], ct);
            //        var leagueOverview = await leagueOverviewTask;
            //        return leagueOverview.Clone(results: await resultsTask);
            //    }
            //    catch (Exception ex)
            //    {
            //        throw new DomParsingException("Failed extracting results from DOM", ex);
            //    }
            //}
            //else
            //{
            //    throw new Exception("No DOM retrieved");
            //}
        }
        public async Task<int?> GetSeasonIdAsync(int leagueId, CancellationToken ct)
        {
            var key = new SeasonIdKey(leagueId);
            var result = await cacheService.GetDataAsync(
                key,
                TimeSpan.FromDays(1),
                async (k, cti) =>
                {
                    var html = await compositeDomRetriever.GetSeasonHtmlAsync(leagueId, cti);
                    int? seasonId = KzsStructureParser.ExtractSeasonId(html);
                    if (!seasonId.HasValue)
                    {
                        throw new Exception($"Couldn't extract season_id for league_id {leagueId}");
                    }
                    return seasonId.Value;
                }, ct);
            return result;
        }

        public async Task<Team> GetTeamAsync(TeamKey key, CancellationToken ct)
        {
            var htmlData = await compositeDomRetriever.GetTeamDataAsync(key.TeamId, key.SeasonId, ct);
            var teamTask = KzsStructureParser.GetTeamDataAsync(htmlData.Info, ct);
            var lastResultsTask = KzsStructureParser.GetLastTeamResultsAsync(key.TeamId, htmlData.LastResults, ct);
            var fixturesTask = KzsStructureParser.GetTeamFixturesAsync(key.TeamId, htmlData.Fixtures, ct);
            var playersTask = KzsStructureParser.GetPlayersAsync(htmlData.Players, ct);
            var team = await teamTask;
            try
            {
                var lastResults = await lastResultsTask;
                team = team.Clone(lastResults: lastResults);
            }
            catch (Exception ex)
            {
                logger.LogWarning(ex, "Failed retrieving lastResults");
                throw new DomParsingException("Failed retrieving lastResults", ex);
            }
            try
            {
                var fixtures = await fixturesTask;
                team = team.Clone(fixtures: fixtures);
            }
            catch (Exception ex)
            {
                logger.LogWarning(ex, "Failed retrieving fixtures");
                throw new DomParsingException("Failed retrieving fixtures", ex);
            }
            try
            {
                var players = await playersTask;
                team = team.Clone(players: players);
            }
            catch (Exception ex)
            {
                logger.LogWarning(ex, "Failed retrieving players");
                throw new DomParsingException("Failed retrieving players", ex);
            }
            return team;
        }

        //public ImmutableArray<Player> ParsePlayers(HtmlNode node)
        //{
        //    var nodes = node.SelectNodes("tbody/tr");
        //    if (nodes != null)
        //    {
        //        var playerNodes = nodes.Skip(1);
        //        var players = playerNodes.Select(n => ParsePlayer(n)).ToImmutableArray();
        //        return players;
        //    }
        //    return ImmutableArray<Player>.Empty;
        //}

        //public Player ParsePlayer(HtmlNode node)
        //{
        //    var player = new Player(
        //        );
        //    return player;
        //}

        public ValueTask<KzsLeagues> GetTopLevelAsync(CancellationToken ct)
        {
            return new ValueTask<KzsLeagues>(KzsLeagues.Default);
        }
    }
}
