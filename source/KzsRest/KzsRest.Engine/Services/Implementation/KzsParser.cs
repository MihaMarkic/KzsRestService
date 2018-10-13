using HtmlAgilityPack;
using KzsRest.Engine.Models;
using KzsRest.Engine.Services.Abstract;
using System;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace KzsRest.Engine.Services.Implementation
{
    public class KzsParser : IKzsParser
    {
        readonly IDomRetriever domRetriever;
        public KzsParser(IDomRetriever domRetriever)
        {
            this.domRetriever = domRetriever;
        }
        public async Task<Team> GetTeamAsync(int teamId, int seasonId, CancellationToken ct)
        {
            // http://www.kzs.si/incl?id=967&team_id=195883&league_id=undefined&season_id=102583
            string address = $"incl?id=967&team_id={teamId}&league_id=undefined&season_id={seasonId}";
            var dom = await domRetriever.GetDomForAsync(address, ct);
            var html = new HtmlDocument();
            html.LoadHtml(dom[0].Content);
            var players = await GetPlayersAsync(address, ct);
            return new Team(players);
        }
        internal async Task<Player[]> GetPlayersAsync(string address, CancellationToken ct)
        {
            //var dom = await domCache.GetDomForAsync(address, TimeSpan.FromMinutes(15), ct, "'33-200-tab-3'");
            //var html = new HtmlDocument();
            //html.LoadHtml(dom);
            return new Player[0];
        }
        public async ValueTask<Standings[]> GetStandingsAsync(string address, CancellationToken ct)
        {
            var dom = await domRetriever.GetDomForAsync(address, ct);
            if (dom.Length > 0)
            {
                var html = new HtmlDocument();
                html.LoadHtml(dom[0].Content);

                var standingsContainer = html.DocumentNode.SelectSingleNode("//div[@id='33-301-standings-container']");
                if (standingsContainer != null)
                {
                    var titles = standingsContainer.SelectNodes("//div[@class='mbt-v2-table-header-before-table']");
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
                }
            }
            return new Standings[0];
        }
        internal static Standings ExtractStanding(HtmlNode node)
        {
            var tableDiv = GetNextElementSibling(node);
            if (tableDiv == null)
            {
                return null;
            }
            var table = tableDiv.SelectSingleNode("table");
            if (table?.Name != "table")
            {
                return null;
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
                pointsMadePerGame: pointsMadeAndReceivedPerGame.Left.Value,
                pointsReceivedPerGame: pointsMadeAndReceivedPerGame.Right.Value,
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
        internal static (int? Left, int? Right) ExtractPairAsInt(HtmlNode node)
        {
            var parts = node.InnerText.Split('/');
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
            //using (var stream = await communicator.GetContent("", ct))
            //{
            //    var doc = converter.GetXDocument(stream);
            //    var query = from e in doc.Elements("li")
            //                where string.Equals((string)e.Attribute("dropdown"), "dropdown", StringComparison.Ordinal) && e.DescendantNodes().Any()
            //                let title = e.Elements("a").First()
            //                where string.Equals(title.Value, "Tekmovanja", StringComparison.Ordinal)

            //}
            //return null;
            return new ValueTask<KzsLeagues>(
                new KzsLeagues
                (
                    majorLeagues: null,
                    minorLeagues: new MinorLeague[]
                    {
                        new MinorLeague(100, 17, Gender.Male, new MinorLeagueDivision[]{
                            new MinorLeagueDivision(177,
                                "clanek/Tekmovanja/Mlajse-kategorije/Fantje/Fantje-U17/Fantje-U17---1.-SKL/cid/177", 17, Gender.Male,
                                "Fantje U17 - 1. SKL", DivisionType.First),
                            new MinorLeagueDivision(115,
                                "clanek/Tekmovanja/Mlajse-kategorije/Fantje/Fantje-U17/Fantje-U17---2.-SKL/cid/115", 17, Gender.Male,
                                "Fantje U17 - 2. SKL", DivisionType.Second),
                            new MinorLeagueDivision(116,
                                "clanek/Tekmovanja/Mlajse-kategorije/Fantje/Fantje-U17/Fantje-U17---kval.-za-1.-SKL/cid/116", 17, Gender.Male,
                                "Fantje U17 - Kval. za 1. SKL", DivisionType.FirstQualify),
                        })
                    }
                ));
        }
    }
}
