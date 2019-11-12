using HtmlAgilityPack;
using KzsRest.Models;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Immutable;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace KzsRest.Engine.Services.Implementation
{
    public class KzsStructureParser
    {
        readonly ILogger<KzsStructureParser> logger;
        public static readonly CultureInfo SloveneCulture = new CultureInfo("sl-SI");
        static readonly Regex seasonIdRegex = new Regex("season_id\\s*:\\s*\\\\'(?<id>\\d+)\\\\'",
            RegexOptions.Singleline | RegexOptions.CultureInvariant | RegexOptions.Compiled);
        public KzsStructureParser(ILogger<KzsStructureParser> logger)
        {
            this.logger = logger;
        }

        internal static Task<TeamGameResult[]> GetLastTeamResultsAsync(int teamId, HtmlNode node, CancellationToken ct)
        {
            return Task.Run(() =>
            {
                var rows = node.SelectNodes("//table[@id='mbt-v2-team-schedule-and-results-tab']/tbody/tr");
                var result = rows.Where(r => IsResult(r)).Select(r => GetTeamGameResult(teamId, r)).ToArray();
                return result;
            });
        }

        internal static Task<TeamGameFixture[]> GetTeamFixturesAsync(int teamId, HtmlNode node, CancellationToken ct)
        {
            return Task.Run(() =>
            {
                var rows = node.SelectNodes("//table[@id='mbt-v2-team-schedule-and-results-tab']/tbody/tr");
                var result = rows.Where(r => !IsResult(r)).Select(r => GetTeamGameFixture(teamId, r)).ToArray();
                return result;
            });
        }

        internal static bool IsResult(HtmlNode root) => root.SelectSingleNode("td[4]")?.InnerText?.Contains(":") ?? false;
        internal static bool IsFixture(HtmlNode root) => !IsResult(root);

        internal static (int Id, DateTimeOffset Date) GetEntryGameInfo(HtmlNode root)
        {
            var dateNode = root.SelectSingleNode("td[2]").Element("a");
            int gameId = int.Parse(dateNode.GetAttributeValue("game_id", null));
            var dateTimeText = dateNode.InnerText;
            DateTimeOffset gameDate = DateTimeOffset.Parse(dateTimeText, SloveneCulture);
            return (gameId, gameDate);
        }
        internal static (int Id, string Name) GetEntryTeamInfo(int tdIndex, HtmlNode root)
        {
            var teamNode = root.SelectSingleNode($"td[{tdIndex}]//a");
            int teamId = int.Parse(teamNode.GetAttributeValue("team_id", null));
            string teamName = HtmlTrim(teamNode.InnerText);
            return (teamId, teamName);
        }
        internal static string HtmlTrim(string source)
        {
            var parts = Regex.Split(source, "\r\n");
            return string.Join(" ", parts.Select(p => p.Trim())).Trim();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="root">tr node for the given game</param>
        /// <returns></returns>
        internal static TeamGameResult GetTeamGameResult(int teamId, HtmlNode root)
        {
            var gameInfo = GetEntryGameInfo(root);
            var homeTeamInfo = GetEntryTeamInfo(3, root);
            var awayTeamInfo = GetEntryTeamInfo(5, root);

            bool isTransfer = teamId != homeTeamInfo.Id;

            var scoreNode = root.SelectSingleNode("td[4]");
            string scoreText = HtmlTrim(scoreNode.InnerText.Replace(" ", ""));
            var scoreParts = scoreText.Split(':');
            int homeScore = int.Parse(scoreParts[0]);
            int awayScore = int.Parse(scoreParts[1]);

            return new TeamGameResult(gameInfo.Id, gameInfo.Date, !isTransfer, 
                homeScore: isTransfer ? awayScore: homeScore,
                opponentScore: isTransfer ? homeScore: awayScore,
                opponentId: isTransfer ? homeTeamInfo.Id: awayTeamInfo.Id,
                opponentName: isTransfer ? homeTeamInfo.Name: awayTeamInfo.Name);
        }

        internal static TeamGameFixture GetTeamGameFixture(int teamId, HtmlNode root)
        {
            var gameInfo = GetEntryGameInfo(root);
            var homeTeamInfo = GetEntryTeamInfo(3, root);
            var awayTeamInfo = GetEntryTeamInfo(5, root);

            bool isTransfer = teamId != homeTeamInfo.Id;

            return new TeamGameFixture(gameInfo.Id, gameInfo.Date, !isTransfer,
                opponentId: isTransfer ? homeTeamInfo.Id : awayTeamInfo.Id,
                opponentName: isTransfer ? homeTeamInfo.Name : awayTeamInfo.Name);
        }

        internal static Task<Team> GetTeamDataAsync(HtmlNode node, CancellationToken ct)
        {
            return Task.Run(() =>
            {
                const string infoValueSelector = "span[@class='mbt-v2-team-full-widget-main-info-value']";
                var root = node.SelectSingleNode("//div[@id='33-200-qualizer-1']");
                var frame = node.SelectSingleNode("(//div[@class='mbt-v2-col mbt-v2-col-6'])[2]");

                string name = null;
                string shortName = null;
                string city = null;
                string coach = null;
                Arena arena = null;

                foreach (var n in frame.SelectNodes("div[@class='mbt-v2-team-full-widget-main-info']"))
                {
                    var header = n.SelectSingleNode("span[@class='mbt-v2-team-full-widget-main-info-attribute']").InnerText;
                    if (header.Contains("Mesto:"))
                    {
                        city = n.SelectSingleNode(infoValueSelector).InnerText;
                    }
                    else if (header.Contains("Kratko ime:"))
                    {
                        shortName = n.SelectSingleNode(infoValueSelector).InnerText;
                    }
                    else if (header.Contains("Klub:"))
                    {
                        name = n.SelectSingleNode(infoValueSelector).InnerText;
                    }
                    else if (header.Contains("Trener:"))
                    {
                        coach = n.SelectSingleNode(infoValueSelector).InnerText;
                    }
                    else if (header.Contains("Dvorana:"))
                    {
                        var valueNode = n.SelectSingleNode(infoValueSelector);
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

        internal static Task<Player[]> GetPlayersAsync(HtmlNode node, CancellationToken ct)
        {
            return Task.Run(() =>
            {
                var rows = node.SelectNodes("//table[@id='mbt-v2-team-roster-table']/tbody/tr");
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

        internal static Task<GameResult[]> ExtractLeagueGameResultsAsync(DomResultItem item, CancellationToken ct)
        {
            return Task.Run(() =>
            {
                var html = new HtmlDocument();
                html.LoadHtml(item.Content);

                var resultsTable = html.DocumentNode.SelectSingleNode("//table[@id='mbt-v2-schedule-table']");
                if (resultsTable != null)
                {
                    var result = ExtractFixturesOrResults<GameResult>(resultsTable, includeResults: true, ct);
                    return result;
                }
                else
                {
                    var container = html.DocumentNode.SelectSingleNode("//div[@id='33-303-container']");
                    if (container.Element("p")?.InnerText.StartsWith("Nobena tekma ne ustreza vašim kriterijem.") ?? false)
                    {
                        return new GameResult[0];
                    }
                    else
                    {
                        throw new Exception("No results table");
                    }
                }
            });
        }

        //internal static Task<LeagueOverview> ExtractStandingsAndFixturesAsync(DomResultItem item, bool areStandingRequired, CancellationToken ct)
        //{
        //    return Task.Run(() =>
        //    {
        //        var html = new HtmlDocument();
        //        html.LoadHtml(item.Content);

        //        var standings = GetStandings(html, areStandingRequired, ct);
        //        var fixturesTable = html.DocumentNode.SelectSingleNode("//table[@id='mbt-v2-schedule-table']");
        //        GameFixture[] fixtures;
        //        // when there are no fixtures, pages shows only results
        //        if (IsFixturesTabActive(html))
        //        {
        //            fixtures = ExtractFixturesOrResults<GameFixture>(fixturesTable, includeResults: false, ct);
        //        }
        //        else
        //        {
        //            fixtures = new GameFixture[0];
        //        }
        //        return new LeagueOverview(
        //                standings: standings,
        //                fixtures: fixtures.Where(f => f != null).ToArray(),
        //                results: null
        //            );
        //    });
        //}

        internal static T[] ExtractFixturesOrResults<T>(HtmlNode root, bool includeResults, CancellationToken ct)
            where T: GameData
        {
            var table = root.SelectSingleNode("table");
            var rows = table.SelectNodes("tbody/tr");
            return rows.Select(r => ExtractGameFixtureOrResult<T>(r, includeResults)).ToArray();
        }

        internal static T ExtractGameFixtureOrResult<T>(HtmlNode tr, bool includeResults)
            where T: GameData
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
            if (typeof(T) == typeof(GameFixture))
            {
                // TODO cups might not have complete fixtures 
                if (dateA != null)
                {
                    return new GameFixture(
                        playDay: ExtractInt(cells[0].InnerText).Value,
                        gameId: ExtractInt(dateA.GetAttributeValue("game_id", null)).Value,
                        seasonId: ExtractInt(dateA.GetAttributeValue("season_id", null)).Value,
                        date: DateTimeOffset.Parse(dateA.InnerText, SloveneCulture),
                        homeTeam: ExtractTeamFixture(homeA, homeScore),
                        awayTeam: ExtractTeamFixture(awayA, awayScore),
                        arena: ExtractArena(arenaA)
                    ) as T;
                }
                else
                {
                    return null;
                }
            }
            else
            {
                return new GameResult(
                    playDay: ExtractInt(cells[0].InnerText).Value,
                    gameId: ExtractInt(dateA.GetAttributeValue("game_id", null)).Value,
                    seasonId: ExtractInt(dateA.GetAttributeValue("season_id", null)).Value,
                    date: DateTimeOffset.Parse(dateA.InnerText, SloveneCulture),
                    homeTeam: ExtractTeamFixture(homeA, homeScore),
                    awayTeam: ExtractTeamFixture(awayA, awayScore),
                    arena: ExtractArena(arenaA)
                ) as T;
            }
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
            // TODO temporal solution for Cup fixtures where team can be null
            var teamId = a.GetAttributeValue("team_id", null);
            if (teamId != null)
            {
                return new TeamFixture(
                    ExtractInt(teamId).Value,
                    HtmlTrim(a.InnerText),
                    leagueId: ExtractInt(a.GetAttributeValue("league_id", null)),
                    seasonId: ExtractInt(a.GetAttributeValue("season_id", null)).Value,
                    score);
            }
            else
            {
                return null;
            }
        }

        internal static Task<Standings[]> GetStandingsAsync(HtmlNode root, CancellationToken ct)
        {
            return Task.Run(() =>
            {
                var titles = root.SelectNodes("//div[@class='mbt-v2-table-header-before-table']");
                if (titles == null)
                {
                    return new Standings[0];
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
            });
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
        internal static int? ExtractSeasonId(string html)
        {
            var match = seasonIdRegex.Match(html);
            if (match.Success)
            {
                var group = match.Groups["id"];
                if (int.TryParse(group.Value, out int seasonId))
                {
                    return seasonId;
                }
            }
            return null;
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
