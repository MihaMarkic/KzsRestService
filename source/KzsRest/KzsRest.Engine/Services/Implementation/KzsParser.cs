﻿using HtmlAgilityPack;
using KzsRest.Engine.Keys;
using KzsRest.Engine.Services.Abstract;
using KzsRest.Models;
using Microsoft.Extensions.Logging;
using System;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace KzsRest.Engine.Services.Implementation
{
    public class KzsParser : IKzsParser
    {
        const string Root = "Root";
        readonly IDomRetriever domRetriever;
        readonly ILogger<KzsParser> logger;
        public static readonly CultureInfo SloveneCulture = new CultureInfo("sl-SI");
        public KzsParser(IDomRetriever domRetriever, ILogger<KzsParser> logger)
        {
            this.domRetriever = domRetriever;
            this.logger = logger;
        }
        public async Task<Team> GetTeamAsync(TeamKey key, CancellationToken ct)
        {
            //const string fixturesAndStandingTab = "33-200-tab-2";
            const string playersTab = "33-200-tab-3";
            // http://www.kzs.si/incl?id=967&team_id=195883&league_id=undefined&season_id=102583
            string address = $"incl?id=967&team_id={key.TeamId}&league_id=undefined&season_id={key.SeasonId}";
            var dom = await domRetriever.GetDomForAsync(address, ct, playersTab);
            var rootPage = dom.Cast<DomResultItem?>().SingleOrDefault(d => string.Equals(d.Value.Id, Root, StringComparison.Ordinal));
            //var fixturesAndResultsPage = dom.Cast<DomResultItem?>().SingleOrDefault(d => string.Equals(d.Value.Id, fixturesAndStandingTab, StringComparison.Ordinal));
            var playersPage = dom.Cast<DomResultItem?>().SingleOrDefault(d => string.Equals(d.Value.Id, playersTab, StringComparison.Ordinal));
            if (!rootPage.HasValue)
            {
                throw new Exception("Couldn't get root tab");
            }
            var teamTask = GetTeamDataAsync(rootPage.Value, ct);
            var lastResultsTask = GetLastTeamResultsAsync(rootPage.Value, ct);
            var fixturesTask = GetShortGameFixturesAsync(rootPage.Value, ct);
            var playersTask = GetPlayersAsync(playersPage.Value, ct);
            var team = await teamTask;
            try
            {
                var lastResults = await lastResultsTask;
                team = team.Clone(lastResults: lastResults);
            }
            catch (Exception ex)
            {
                logger.LogWarning(ex, "Failed retrieving lastResults");
                throw;
            }
            try
            {
                var fixtures = await fixturesTask;
                team = team.Clone(fixtures: fixtures);
            }
            catch (Exception ex)
            {
                logger.LogWarning(ex, "Failed retrieving fixtures");
                throw;
            }
            try
            {
                var players = await playersTask;
                team = team.Clone(players: players);
            }
            catch (Exception ex)
            {
                logger.LogWarning(ex, "Failed retrieving players");
                throw;
            }
            return team;
        }
        internal static Task<GameResult[]> GetLastTeamResultsAsync(DomResultItem domItem, CancellationToken ct)
        {
            return Task.Run(() =>
            {
                var html = new HtmlDocument();
                html.LoadHtml(domItem.Content);
                var rows = html.DocumentNode.SelectNodes("//table[@id='mbt-v2-team-home-results-table']/tbody/tr");
                var result = rows.Select(r => GetTeamGameResult(r)).ToArray();
                return result;
            });
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="root">tr node for the given game</param>
        /// <returns></returns>
        internal static GameResult GetTeamGameResult(HtmlNode root)
        {
            var dateNode = root.Element("td").Element("a");
            int gameId = int.Parse(dateNode.GetAttributeValue("game_id", null));
            string dateText = dateNode.FirstChild.InnerText;
            string timeText = dateNode.Element("span").InnerText;
            DateTimeOffset gameDate = DateTimeOffset.Parse($"{dateText} {timeText}", SloveneCulture);
            var opponentNode = root.SelectSingleNode("td[2]");
            bool isTransfer = string.Equals(opponentNode.Element("span").InnerText, "pri");
            var opponentTeamNode = opponentNode.Element("a");
            var opponentTeamId = int.Parse(opponentTeamNode.GetAttributeValue("team_id", null));
            var scoreNode = root.SelectSingleNode("td[3]/a");
            string scoreText = scoreNode.InnerText;
            var scoreParts = scoreText.Split(':');
            int homeScore = int.Parse(scoreParts[0]);
            int opponentScore = int.Parse(scoreParts[1]);
            return new GameResult(gameId, gameDate, !isTransfer, homeScore, opponentScore, opponentTeamId, opponentTeamNode.InnerText);
        }
        internal static Task<ShortGameFixture[]> GetShortGameFixturesAsync(DomResultItem domItem, CancellationToken ct)
        {
            return Task.Run(() =>
            {
                var html = new HtmlDocument();
                html.LoadHtml(domItem.Content);
                var rows = html.DocumentNode.SelectNodes("//table[@id='mbt-v2-team-home-schedule-table']/tbody/tr");
                var result = rows.Select(r => GetShortGameFixture(r)).ToArray();
                return result;
            });
        }
        internal static ShortGameFixture GetShortGameFixture(HtmlNode root)
        {
            var dateNode = root.Element("td").Element("a");
            int gameId = int.Parse(dateNode.GetAttributeValue("game_id", null));
            string dateText = dateNode.FirstChild.InnerText;
            string timeText = dateNode.Element("span").InnerText;
            DateTimeOffset gameDate = DateTimeOffset.Parse($"{dateText} {timeText}", SloveneCulture);
            var opponentNode = root.SelectSingleNode("td[2]");
            bool isTransfer = string.Equals(opponentNode.Element("span").InnerText, "pri");
            var opponentTeamNode = opponentNode.Element("a");
            var opponentTeamId = int.Parse(opponentTeamNode.GetAttributeValue("team_id", null));
            return new ShortGameFixture(gameId, gameDate, !isTransfer, opponentTeamId, 
                                           opponentTeamNode.InnerText);
        }
        internal static Task<Team> GetTeamDataAsync(DomResultItem domItem, CancellationToken ct)
        {
            return Task.Run(() =>
            {
                const string infoValueSelector = "span[@class='mbt-v2-team-full-widget-main-info-value']";
                var html = new HtmlDocument();
                html.LoadHtml(domItem.Content);
                var root = html.DocumentNode.SelectSingleNode("//div[@id='33-200-qualizer-1']");
                var frame = html.DocumentNode.SelectSingleNode("(//div[@class='mbt-v2-col mbt-v2-col-6'])[2]");

                string name = null;
                string shortName = null;
                string city = null;
                string coach = null;
                Arena arena = null;

                foreach (var node in frame.SelectNodes("div[@class='mbt-v2-team-full-widget-main-info']"))
                {
                    var header = node.SelectSingleNode("span[@class='mbt-v2-team-full-widget-main-info-attribute']").InnerText;
                    if (header.Contains("Mesto:"))
                    {
                        city = node.SelectSingleNode(infoValueSelector).InnerText;
                    }
                    else if (header.Contains("Kratko ime:"))
                    {
                        shortName = node.SelectSingleNode(infoValueSelector).InnerText;
                    }
                    else if (header.Contains("Klub:"))
                    {
                        name = node.SelectSingleNode(infoValueSelector).InnerText;
                    }
                    else if (header.Contains("Trener:"))
                    {
                        coach = node.SelectSingleNode(infoValueSelector).InnerText;
                    }
                    else if (header.Contains("Dvorana:"))
                    {
                        var valueNode = node.SelectSingleNode(infoValueSelector);
                        var a = valueNode.SelectSingleNode("a");
                        int arenaId = int.Parse(a.GetAttributeValue("arena_id", null));
                        arena = new Arena(arenaId, a.InnerText, HttpUtility.HtmlDecode(a.GetAttributeValue("href", null)));
                    }
                }

                return new Team(
                    name,
                    shortName,
                    city,
                    arena,
                    coach,
                    players: null,
                    lastResults: null,
                    fixtures: null
                    );
            });
        }

        internal static Task<Player[]> GetPlayersAsync(DomResultItem domItem, CancellationToken ct)
        {
            return Task.Run(() =>
            {
                var html = new HtmlDocument();
                html.LoadHtml(domItem.Content);
                var rows = html.DocumentNode.SelectNodes("//table[@id='mbt-v2-team-roster-table']/tbody/tr");
                var result = rows.Select(r => GetPlayer(r)).ToArray();
                return result;
            });
        }
        internal static Player GetPlayer(HtmlNode domItem)
        {
            var cellNodes = domItem.SelectNodes("td");
            int? number = ParseNullableInt(cellNodes[0].InnerText);
            var nameNode = cellNodes[1].SelectSingleNode("a");
            string fullName = nameNode.InnerText;
            int playerId = int.Parse(nameNode.GetAttributeValue("player_id", null));
            int? birthYear = ParseNullableInt(cellNodes[2].InnerText);
            var nationalityNode = cellNodes[3].SelectSingleNode("img");
            string nationality = nationalityNode.GetAttributeValue("title", null);
            string nationalityCode = nationalityNode.GetAttributeValue("alt", null);
            string position = cellNodes[4].InnerText.Trim('\r','\n',' ', '\t');
            int? height = ParseNullableInt(cellNodes[5].InnerText.Trim('\r', '\n', ' ', 'c', 'm', '\t'));
            return new Player(playerId, number, fullName, nationalityCode, nationality, birthYear, position != "-" ? position: null, height);
        }

        public async Task<LeagueOverview> GetLeagueOverviewAsync(string address, bool areStandingRequired, CancellationToken ct)
        {
            var dom = await domRetriever.GetDomForAsync(address, ct, "33-303-tab-2");
            if (dom.Length > 0)
            {
                var leagueOverviewTask = ExtractStandingsAndFixturesAsync(dom[0], areStandingRequired, ct);
                var resultsTask = ExtractLeagueGameResultsAsync(dom[1], ct);
                var leagueOverview = await leagueOverviewTask;
                return leagueOverview.Clone(results: await resultsTask);
            }
            else
            {
                throw new Exception("No DOM retrieved");
            }
        }

        internal static Task<GameFixture[]> ExtractLeagueGameResultsAsync(DomResultItem item, CancellationToken ct)
        {
            return Task.Run(() =>
            {
                var html = new HtmlDocument();
                html.LoadHtml(item.Content);

                var resultsTable = html.DocumentNode.SelectSingleNode("//table[@id='mbt-v2-schedule-table']");
                var result = ExtractFixturesOrResults(resultsTable, includeResults: true, ct);
                return result;
            });
        }

        internal static Task<LeagueOverview> ExtractStandingsAndFixturesAsync(DomResultItem item, bool areStandingRequired, CancellationToken ct)
        {
            return Task.Run(() =>
            {
                var html = new HtmlDocument();
                html.LoadHtml(item.Content);

                var standings = GetStandings(html, areStandingRequired, ct);
                var fixturesTable = html.DocumentNode.SelectSingleNode("//table[@id='mbt-v2-schedule-table']");
                var fixtures = ExtractFixturesOrResults(fixturesTable, includeResults: false, ct);
                return new LeagueOverview(
                        standings: standings,
                        fixtures: fixtures,
                        results: null
                    );
            });
        }

        internal static GameFixture[] ExtractFixturesOrResults(HtmlNode table, bool includeResults, CancellationToken ct)
        {
            var rows = table.SelectNodes("tbody/tr");
            return rows.Select(r => ExtractGameFixtureOrResult(r, includeResults)).ToArray();
        }

        internal static GameFixture ExtractGameFixtureOrResult(HtmlNode tr, bool includeResults)
        {
            var cells = tr.SelectNodes("td");
            var dateA = cells[1].SelectSingleNode("a");
            var homeA = cells[2].SelectSingleNode("a") ?? cells[2].SelectSingleNode("span/a");
            var awayA = cells[4].SelectSingleNode("a") ?? cells[4].SelectSingleNode("span/a");
            var arenaA = cells[5].SelectSingleNode("a");
            int? homeScore = null;
            int? awayScore = null;
            if (includeResults)
            {
                (homeScore, awayScore) = ExtractPairAsInt(cells[3].SelectSingleNode("a"), split: ':');
            }
            return new GameFixture(
                playDay: ExtractInt(cells[0].InnerText).Value,
                gameId: ExtractInt(dateA.GetAttributeValue("game_id", null)).Value,
                date: DateTimeOffset.Parse(dateA.InnerText, SloveneCulture),
                homeTeam: ExtractTeamFixture(homeA, homeScore),
                awayTeam: ExtractTeamFixture(awayA, awayScore),
                arena: ExtractArena(arenaA)
            );
        }

        internal static Arena ExtractArena(HtmlNode a)
        {
            return new Arena(
                id: ExtractInt(a.GetAttributeValue("arena_id", null)).Value,
                name: a.InnerText,
                url: null
            );
        }

        internal static TeamFixture ExtractTeamFixture(HtmlNode a, int? score)
        {
            return new TeamFixture(
                ExtractInt(a.GetAttributeValue("team_id", null)).Value,
                a.InnerText,
                leagueId: ExtractInt(a.GetAttributeValue("league_id", null)),
                seasonId: ExtractInt(a.GetAttributeValue("season_id", null)).Value,
                score);
        }

        internal static Standings[] GetStandings(HtmlDocument html, bool areStandingRequired, CancellationToken ct)
        {
            var standingsContainer = html.DocumentNode.SelectSingleNode("//div[@id='33-301-standings-container']");
            if (standingsContainer == null)
            {
                throw new Exception("Couldn't find standings container");
            }
            var titles = standingsContainer.SelectNodes("//div[@class='mbt-v2-table-header-before-table']");
            if (titles == null)
            {
                if (areStandingRequired)
                {
                    throw new Exception("Couldn't find titles");
                }
                else
                {
                    return new Standings[0];
                }
            }
            if (titles?.Count > 0)
            {
                var standings = new Standings[titles.Count];
                for (int i = 0; i < titles.Count; i++)
                {
                    var standing = ExtractStanding(titles[i]);
                    if (standing != null)
                    {
                        standings[i] = standing;
                    }
                }
                return standings;
            }
            else
            {
                return new Standings[0];
            }
        }

        internal static Standings ExtractStanding(HtmlNode node)
        {
            var tableDiv = GetNextElementSibling(node);
            if (tableDiv == null)
            {
                throw new Exception("Couldn't find table div");
            }
            var table = tableDiv.SelectSingleNode("table");
            if (table?.Name != "table")
            {
                throw new Exception("Couldn't find table element");
            }
            var rows = table.SelectNodes("tbody/tr");
            return new Standings(node.InnerText, rows.Select(r => ExtractStandingsEntry(r)).ToArray());
        }
        internal static StandingsEntry ExtractStandingsEntry(HtmlNode tr)
        {
            var cells = tr.SelectNodes("td");
            var teamData = ExtractTeamData(cells[1]);
            var pointsMadeAndReceived = ExtractPairAsInt(cells[6]);
            var pointsMadeAndReceivedPerGame = ExtractPairAsDecimal(cells[8]);
            var homeResults = ExtractPairAsInt(cells[9]);
            var awayResults = ExtractPairAsInt(cells[10]);
            var homePointsPerGame = ExtractPairAsDecimal(cells[11]);
            var awayPointsPerGame = ExtractPairAsDecimal(cells[12]);
            var lastFive = ExtractPairAsInt(cells[13]);
            var lastTen = ExtractPairAsInt(cells[14]);
            var fivePointsGames = ExtractPairAsInt(cells[18]);
            return new StandingsEntry(
                teamData.TeamId,
                teamData.Season,
                teamData.League,
                position: ExtractPosition(cells[0]),
                teamData.TeamName,
                games: ExtractInt(cells[2]).Value,
                won: ExtractInt(cells[3]).Value,
                lost: ExtractInt(cells[4]).Value,
                points: ExtractInt(cells[5]).Value,
                pointsMade: pointsMadeAndReceived.Left,
                pointsReceived: pointsMadeAndReceived.Right,
                pointsDifference: ExtractInt(cells[7]).Value,
                pointsMadePerGame: pointsMadeAndReceivedPerGame.Left,
                pointsReceivedPerGame: pointsMadeAndReceivedPerGame.Right,
                homeWins: homeResults.Left.Value,
                homeDefeats: homeResults.Right.Value,
                awayWins: awayResults.Left.Value,
                awayDefeats: awayResults.Right.Value,
                homePointsMadePerGame: homePointsPerGame.Left,
                homePointsReceivedPerGame: homePointsPerGame.Right,
                awayPointsMadePerGame: awayPointsPerGame.Left,
                awayPointsReceivedPerGame: awayPointsPerGame.Right,
                lastFiveGamesWon: lastFive.Left.Value,
                lastFiveGamesLost: lastFive.Right.Value,
                lastTenGamesWon: lastTen.Left.Value,
                lastTenGamesLost: lastTen.Right.Value,
                gameSeries: ExtractInt(cells[15]),
                homeGameSeries: ExtractInt(cells[16]),
                awayGameSeries: ExtractInt(cells[17]),
                fivePointWins: fivePointsGames.Left,
                fivePointsDefeats: fivePointsGames.Right
            );
        }
        internal static int? ExtractInt(HtmlNode node)
        {
            if (int.TryParse(node.InnerText, out int value))
            {
                return value;
            }
            return null;
        }
        internal static int? ExtractInt(string text)
        {
            if (int.TryParse(text, out int value))
            {
                return value;
            }
            return null;
        }
        internal static (int? Left, int? Right) ExtractPairAsInt(HtmlNode node, char split = '/')
        {
            var parts = node.InnerText.Split(split);
            (int? Left, int? Right) result = default;
            if (int.TryParse(parts[0], out int left))
            {
                result.Left = left;
            }
            if (int.TryParse(parts[1], out int right))
            {
                result.Right = right;
            }
            return result;
        }
        internal static (decimal? Left, decimal? Right) ExtractPairAsDecimal(HtmlNode node)
        {
            var parts = node.InnerText.Split('/');
            (decimal? Left, decimal? Right) result = default;
            if (decimal.TryParse(parts[0], NumberStyles.Number, CultureInfo.InvariantCulture, out decimal left))
            {
                result.Left = left;
            }
            if (decimal.TryParse(parts[1], NumberStyles.Number, CultureInfo.InvariantCulture, out decimal right))
            {
                result.Right = right;
            }
            return result;
        }
        internal static (string TeamName, int TeamId, int? League, int? Season) ExtractTeamData(HtmlNode node)
        {
            (string TeamName, int TeamId, int? League, int? Season) result = default;
            var a = node.SelectSingleNode("a");
            result.TeamName = a.InnerText;
            result.TeamId = int.Parse(a.GetAttributeValue("team_id", null));
            string leagueIdText = a.GetAttributeValue("league_id", null);
            if (int.TryParse(leagueIdText, out var leagueId))
            {
                result.League = leagueId;
            }
            string seasonIdText = a.GetAttributeValue("season_id", null);
            if (int.TryParse(seasonIdText, out var seasonId))
            {
                result.Season = seasonId;
            }
            return result;
        }
        internal static int ExtractPosition(HtmlNode node)
        {
            return int.Parse(node.InnerText.Trim('.', '\r', '\n', ' ', '\t'));
        }
        internal static HtmlNode GetNextElementSibling(HtmlNode node)
        {
            HtmlNode next = node;
            while ((next = next.NextSibling) != null)
            {
                if (next.NodeType == HtmlNodeType.Element)
                {
                    break;
                }
            }
            return next;
        }
        public ValueTask<KzsLeagues> GetTopLevelAsync(CancellationToken ct)
        {
            return new ValueTask<KzsLeagues>(
                new KzsLeagues
                (
                    majorLeagues: new MajorLeague[]{
                        new MajorLeague("Liga Nova KBM", 1, Gender.Men, "clanek/Tekmovanja/Liga-Nova-KBM/cid/66"),
                        new MajorLeague("2. SKL", 2, Gender.Men, "clanek/Tekmovanja/2.-SKL/cid/68"),
                        new MajorLeague("3. SKL", 3, Gender.Men, "clanek/Tekmovanja/3.-SKL/cid/69"),
                        new MajorLeague("4. SKL", 4, Gender.Men, "clanek/Tekmovanja/4.-SKL/cid/70"),
                        new MajorLeague("1. SKL za ženske", 1, Gender.Women, "clanek/Tekmovanja/1.-SKL-za-zenske/cid/67")
                    },
                    minorLeagues: new MinorLeague[]
                    {
                        new MinorLeague(19, Gender.Men, new MinorLeagueDivision[]{
                            new MinorLeagueDivision(
                                "clanek/Tekmovanja/Mlajse-kategorije/Fantje/Fantje-U19/cid/99", 19, Gender.Men,
                                "Fantje U19 - 1. A SKL", DivisionType.FirstA),
                            new MinorLeagueDivision(
                                "clanek/Tekmovanja/Mlajse-kategorije/Fantje/Fantje-U19/Fantje-U19---1.-B-SKL/cid/112", 19, Gender.Men,
                                "Fantje U19 - 2. B SKL", DivisionType.FirstB),
                            new MinorLeagueDivision(
                                "clanek/Tekmovanja/Mlajse-kategorije/Fantje/Fantje-U19/Fantje-U19---2.-SKL/cid/113", 19, Gender.Men,
                                "Fantje U19 - 2. SKL", DivisionType.Second),
                            new MinorLeagueDivision(
                                "clanek/Tekmovanja/Mlajse-kategorije/Fantje/Fantje-U19/Fantje-U19---kval.-za-1.-SKL/cid/114", 19, Gender.Men,
                                "Fantje U19 - Kval. za 1. SKL", DivisionType.FirstQualify),
                        }),
                        new MinorLeague(17, Gender.Men, new MinorLeagueDivision[]{
                            new MinorLeagueDivision(
                                "clanek/Tekmovanja/Mlajse-kategorije/Fantje/Fantje-U17/Fantje-U17---1.-SKL/cid/177", 17, Gender.Men,
                                "Fantje U17 - 1. SKL", DivisionType.First),
                            new MinorLeagueDivision(
                                "clanek/Tekmovanja/Mlajse-kategorije/Fantje/Fantje-U17/Fantje-U17---2.-SKL/cid/115", 17, Gender.Men,
                                "Fantje U17 - 2. SKL", DivisionType.Second),
                            new MinorLeagueDivision(
                                "clanek/Tekmovanja/Mlajse-kategorije/Fantje/Fantje-U17/Fantje-U17---kval.-za-1.-SKL/cid/116", 17, Gender.Men,
                                "Fantje U17 - Kval. za 1. SKL", DivisionType.FirstQualify),
                        }),
                        new MinorLeague(15, Gender.Men, new MinorLeagueDivision[]{
                            new MinorLeagueDivision(
                                "clanek/Tekmovanja/Mlajse-kategorije/Fantje/Fantje-U15/cid/101", 15, Gender.Men,
                                "Fantje U15 - 1. SKL", DivisionType.First),
                            new MinorLeagueDivision(
                                "clanek/Tekmovanja/Mlajse-kategorije/Fantje/Fantje-U15/Fantje-U15---2.-SKL/cid/117", 15, Gender.Men,
                                "Fantje U15 - 2. SKL", DivisionType.Second),
                            new MinorLeagueDivision(
                                "clanek/Tekmovanja/Mlajse-kategorije/Fantje/Fantje-U15/Fantje-U15---kval.-za-1.-SKL/cid/118", 15, Gender.Men,
                                "Fantje U15 - Kval. za 1. SKL", DivisionType.FirstQualify)
                        }),
                        new MinorLeague(13, Gender.Men, new MinorLeagueDivision[]{
                            new MinorLeagueDivision(    
                                "clanek/Tekmovanja/Mlajse-kategorije/Fantje/Fantje-U13/cid/102", 13, Gender.Men,
                                "Fantje U13 - 1. SKL", DivisionType.First),
                            new MinorLeagueDivision(
                                "clanek/Tekmovanja/Mlajse-kategorije/Fantje/Fantje-U13/cid/119", 13, Gender.Men,
                                "Fantje U13 - 2. SKL", DivisionType.Second),
                            new MinorLeagueDivision(
                                "clanek/Tekmovanja/Mlajse-kategorije/Fantje/Fantje-U13/Fantje-U13---kval.-za-1.-SKL/cid/120", 13, Gender.Men,
                                "Fantje U13 - Kval. za 1. SKL", DivisionType.FirstQualify)
                        }),
                        new MinorLeague(11, Gender.Men, new MinorLeagueDivision[]{
                            new MinorLeagueDivision(
                                "clanek/Tekmovanja/Mlajse-kategorije/Fantje/Fantje-U11/cid/103", 11, Gender.Men,
                                "Fantje U11", DivisionType.First)
                        }),
                        new MinorLeague(9, Gender.Men, new MinorLeagueDivision[]{
                            new MinorLeagueDivision(
                                "clanek/Tekmovanja/Mlajse-kategorije/Fantje-in-dekleta-U9/cid/104", 9, Gender.Men,
                                "Fantje in dekleta U9", DivisionType.First)
                        }),
                        // Women
                        new MinorLeague(19, Gender.Women, new MinorLeagueDivision[]{
                            new MinorLeagueDivision(
                                "clanek/Tekmovanja/Mlajse-kategorije/Dekleta/Dekleta-U19/cid/105", 17, Gender.Men,
                                "Dekleta U19", DivisionType.First)
                        }),
                        new MinorLeague(17, Gender.Women, new MinorLeagueDivision[]{
                            new MinorLeagueDivision(
                                "clanek/Tekmovanja/Mlajse-kategorije/Dekleta/Dekleta-U17/cid/106", 17, Gender.Men,
                                "Dekleta U17", DivisionType.First)
                        }),
                        new MinorLeague(15, Gender.Women, new MinorLeagueDivision[]{
                            new MinorLeagueDivision(
                                "clanek/Tekmovanja/Mlajse-kategorije/Dekleta/Dekleta-U15/cid/107", 15, Gender.Men,
                                "Dekleta U15", DivisionType.First)
                        }),
                        new MinorLeague(13, Gender.Women, new MinorLeagueDivision[]{
                            new MinorLeagueDivision(
                                "clanek/Tekmovanja/Mlajse-kategorije/Dekleta/Dekleta-U13/cid/108", 13, Gender.Men,
                                "Dekleta U13", DivisionType.First)
                        }),
                        new MinorLeague(11, Gender.Women, new MinorLeagueDivision[]{
                            new MinorLeagueDivision(
                                "clanek/Tekmovanja/Mlajse-kategorije/Dekleta/Dekleta-U11/cid/109", 11, Gender.Men,
                                "Dekleta U11", DivisionType.First)
                        }),
                        new MinorLeague(9, Gender.Women, new MinorLeagueDivision[]{
                            new MinorLeagueDivision(
                                "clanek/Tekmovanja/Mlajse-kategorije/Fantje-in-dekleta-U9/cid/104", 9, Gender.Men,
                                "Fantje in dekleta U9", DivisionType.First)
                        })
                    },
                    majorCupLeagues: new MajorCupLeague[]
                    {
                        new MajorCupLeague("Pokal SPAR", Gender.Men, "clanek/Tekmovanja/Pokal-Spar/cid/72"),
                        new MajorCupLeague("Pokal članic", Gender.Women, "clanek/Tekmovanja/Pokal-clanic/cid/73")
                    },
                    minorCupLeagues: new MinorCupLeague[]
                    {
                        new MinorCupLeague("Mini pokal SPAR", Gender.Men, "clanek/Tekmovanja/Mini-pokal-Spar/cid/74"),
                        new MinorCupLeague("Mini pokal deklet", Gender.Women, "clanek/Tekmovanja/Mini-pokal-deklet/cid/75")
                    }
                ));
        }

        internal static int? ParseNullableInt(string text)
        {
            if (int.TryParse(text, out var result))
            {
                return result;
            }
            return null;
        }
    }
}
