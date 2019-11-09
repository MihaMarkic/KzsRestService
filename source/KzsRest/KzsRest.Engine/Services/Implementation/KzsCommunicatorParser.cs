using HtmlAgilityPack;
using KzsRest.Engine.Keys;
using KzsRest.Engine.Services.Abstract;
using KzsRest.Models;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Immutable;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace KzsRest.Engine.Services.Implementation
{
    public class KzsCommunicatorParser : IKzsParser
    {
        const string Root = "Root";
        readonly IHttpCompositeDomRetriever compositeDomRetriever;
        readonly ILogger<KzsCommunicatorParser> logger;
        public static readonly CultureInfo SloveneCulture = new CultureInfo("sl-SI");
        public KzsCommunicatorParser(IHttpCompositeDomRetriever compositeDomRetriever, ILogger<KzsCommunicatorParser> logger)
        {
            this.compositeDomRetriever = compositeDomRetriever;
            this.logger = logger;
        }
        public Task<LeagueOverview> GetLeagueOverviewAsync(string address, bool areStandingRequired, CancellationToken ct)
        {
            throw new NotImplementedException();
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
            throw new NotImplementedException();
        }
    }
}
