using AutoFixture;
using HtmlAgilityPack;
using KzsRest.Engine.Services.Abstract;
using KzsRest.Engine.Services.Implementation;
using KzsRest.Models;
using NSubstitute;
using NUnit.Framework;
using System;
using System.IO;
using System.Threading.Tasks;

namespace KzsRest.Engine.Test.Services.Implementation
{
    public class KzsStructureParserTest: BaseTest<KzsStructureParser>
    {
        internal const string U17_Male_A = "u17_male_a";
        internal const string SampleStandingsTable = "sample_standings_table";
        internal const string LeagueFixtures = "league_fixtures";
        internal const string LeagueResults = "league_results";
        internal const string LeagueNoFixtures = "league_scores_selected";
        internal const string SampleHtmlForSeasonId = "sample_html_for_seasonid";
        internal static string GetSampleContent(string file) => File.ReadAllText(Path.Combine("Samples", $"{file}.html"));
        internal static HtmlNode GetRootNode(string key)
        {

            var html = new HtmlDocument();
            html.LoadHtml(GetSampleContent(key));
            return html.DocumentNode;
        }
        [TestFixture]
        public class GetLeagueOverviewAsync : BaseTest<KzsStructureParser>
        {
            //[Test]
            //public void WhenNoData_ThrowsException()
            //{
            //    var domRetriever = fixture.Freeze<IDomRetriever>();
            //    domRetriever.GetDomForAsync(default, default).ReturnsForAnyArgs(new DomResultItem[0]);

            //    Assert.ThrowsAsync<Exception>(async () => await Target.GetLeagueOverviewAsync(default, areStandingRequired: true, default));
            //}
        }
        // ![](9094DAC2FC5B0B5EC5750D16893287AF.png)
        [TestFixture]
        public class GetStandingsAsync: KzsStructureParserTest
        {
            //[Test]
            //public async Task WhenNoDataAndStandingsNotRequired_ReturnsEmptyArray()
            //{
            //    var doc = new HtmlDocument();
            //    doc.Load(new StringReader("<div id='33-301-standings-container'></div>"));
            //    var actual = await KzsStructureParser.GetStandings(doc.DocumentNode, default);

            //    Assert.That(actual.Length, Is.Zero);
            //}
            [Test]
            public async Task WhenFiveGroups_ReturnsAllSix()
            {
                var root = GetRootNode("sample_standings_table");

                var actual = await KzsStructureParser.GetStandingsAsync(root, default);

                Assert.That(actual.Length, Is.EqualTo(5));
            }
            [Test]
            public async Task WhenSample_FirstGroupNameIsCorrect()
            {
                var root = GetRootNode("sample_standings_table");

                var actual = await KzsStructureParser.GetStandingsAsync(root, default);

                var firstGroup = actual[0];
                Assert.That(firstGroup.Name, Is.EqualTo("2. del - Skupina A1"));
            }
        }
        [TestFixture]
        public class ExtractStanding: KzsStructureParserTest
        {
            // ![](FE341BE5FF5472C7DDF163BAC18FBAF8.png)
            //[Test]
            //public void WhenSampleInput_ParsesTitleCorrectly()
            //{
            //    var html = new HtmlDocument();
            //    html.LoadHtml(GetSampleContent(SampleStandingsTable));

            //    var actual = KzsStructureParser.ExtractStanding(html.DocumentNode.SelectSingleNode("/body/div"));

            //    Assert.That(actual.Name, Is.EqualTo("2. del - Skupina A1"));
            //}
            //[Test]
            //public void WhenSampleInputHasTwoRows_NumberOfEntriesIsTwo()
            //{
            //    var html = new HtmlDocument();
            //    html.LoadHtml(GetSampleContent(SampleStandingsTable));

            //    var actual = KzsStructureParser.ExtractStanding(html.DocumentNode.SelectSingleNode("/body/div"));

            //    Assert.That(actual.Entries.Length, Is.EqualTo(2));
            //}
        }
        [TestFixture]
        public class ExtractStandingsEntry : KzsStructureParserTest
        {
            //[Test]
            //public void RowIsParsedCorrectly()
            //{
            //    var html = new HtmlDocument();
            //    html.LoadHtml(GetSampleContent(SampleStandingsTable));
            //    var node = html.DocumentNode.SelectSingleNode("//tbody/tr");

            //    var actual = KzsStructureParser.ExtractStandingsEntry(node);

            //    Assert.That(actual.TeamId, Is.EqualTo(195903));
            //    Assert.That(actual.Season, Is.EqualTo(102583));
            //    Assert.That(actual.League.HasValue, Is.False);
            //    Assert.That(actual.Position, Is.EqualTo(1));
            //    Assert.That(actual.TeamName, Is.EqualTo("Zlatorog"));
            //    Assert.That(actual.Games, Is.EqualTo(2));
            //    Assert.That(actual.Won, Is.EqualTo(2));
            //    Assert.That(actual.Lost, Is.EqualTo(0));
            //    Assert.That(actual.Points, Is.EqualTo(4));
            //    Assert.That(actual.PointsMade, Is.EqualTo(171));
            //    Assert.That(actual.PointsReceived, Is.EqualTo(141));
            //    Assert.That(actual.PointsDifference, Is.EqualTo(30));
            //    Assert.That(actual.PointsMadePerGame, Is.EqualTo(85.5m));
            //    Assert.That(actual.PointsReceivedPerGame, Is.EqualTo(70.5m));
            //    Assert.That(actual.HomeWins, Is.EqualTo(1));
            //    Assert.That(actual.HomeDefeats, Is.EqualTo(0));
            //    Assert.That(actual.AwayWins, Is.EqualTo(1));
            //    Assert.That(actual.AwayDefeats, Is.EqualTo(0));
            //    Assert.That(actual.HomePointsMadePerGame, Is.EqualTo(79m));
            //    Assert.That(actual.HomePointsReceivedPerGame, Is.EqualTo(67m));
            //    Assert.That(actual.AwayPointsMadePerGame, Is.EqualTo(92m));
            //    Assert.That(actual.AwayPointsReceivedPerGame, Is.EqualTo(74m));
            //    Assert.That(actual.LastFiveGamesWon, Is.EqualTo(2));
            //    Assert.That(actual.LastFiveGamesLost, Is.EqualTo(0));
            //    Assert.That(actual.LastTenGamesWon, Is.EqualTo(2));
            //    Assert.That(actual.LastTenGamesLost, Is.EqualTo(0));
            //    Assert.That(actual.GameSeries, Is.EqualTo(2));
            //    Assert.That(actual.HomeGameSeries, Is.EqualTo(1));
            //    Assert.That(actual.AwayGameSeries, Is.EqualTo(1));
            //    Assert.That(actual.FivePointsWins, Is.EqualTo(0));
            //    Assert.That(actual.FivePointsDefeats, Is.EqualTo(0));
            //}
        }
        [TestFixture]
        public class ExtractGameFixture : KzsStructureParserTest
        {
            // ![](3C860C6F9DDD09E69B5C230807D3E224.png)
            protected HtmlNode table;
            [SetUp]
            public new void SetUp()
            {
                var root = GetRootNode(LeagueFixtures);
                table = root.SelectSingleNode("table");
            }
            [Test]
            public void GivenSampleDataFromFirstRow_ParsesFixturesCorrectly()
            {
                var tr = table.SelectSingleNode("tbody/tr[1]");

                var actual = KzsStructureParser.ExtractGameFixtureOrResult<GameFixture>(tr, includeResults: false);

                Assert.That(actual.PlayDay, Is.EqualTo(1));
                Assert.That(actual.GameId, Is.EqualTo(5027865));
                Assert.That(actual.Date, Is.EqualTo(DateTimeOffset.Parse("17.11.2019", KzsStructureParser.SloveneCulture)));
                Assert.That(actual.HomeTeam.TeamId, Is.EqualTo(166193));
                Assert.That(actual.HomeTeam.Name, Is.EqualTo("Helios Suns Matica MB"));
                Assert.That(actual.HomeTeam.LeagueId, Is.Null);
                Assert.That(actual.HomeTeam.SeasonId, Is.EqualTo(110309));
                Assert.That(actual.HomeTeam.Score, Is.Null);
                Assert.That(actual.AwayTeam.TeamId, Is.EqualTo(195173));
                Assert.That(actual.AwayTeam.Name, Is.EqualTo("GIB Ilirija"));
                Assert.That(actual.AwayTeam.LeagueId, Is.Null);
                Assert.That(actual.AwayTeam.SeasonId, Is.EqualTo(110309));
                Assert.That(actual.AwayTeam.Score, Is.Null);
                Assert.That(actual.Arena.Id, Is.EqualTo(7053));
                Assert.That(actual.Arena.Name, Is.EqualTo("ŠD Domžale"));
            }
        }
        [TestFixture]
        public class ExtractGameResult : KzsStructureParserTest
        {
            // ![](1FF8284AA13E13A048520A026F953197.png)
            protected HtmlNode table;
            [SetUp]
            public new void SetUp()
            {
                var root = GetRootNode(LeagueResults);
                table = root.SelectSingleNode("table");
            }
            [Test]
            public void GivenSampleDataFromFirstRow_ParsesResultsCorrectly()
            {
                var tr = table.SelectSingleNode("tbody/tr[1]");

                var actual = KzsStructureParser.ExtractGameFixtureOrResult<GameResult>(tr, includeResults: true);

                Assert.That(actual.PlayDay, Is.EqualTo(6));
                Assert.That(actual.GameId, Is.EqualTo(4676885));
                Assert.That(actual.SeasonId, Is.EqualTo(110309));
                Assert.That(actual.Date, Is.EqualTo(DateTimeOffset.Parse("03.11.2019 20:00", KzsStructureParser.SloveneCulture)));
                Assert.That(actual.HomeTeam.TeamId, Is.EqualTo(166193));
                Assert.That(actual.HomeTeam.Name, Is.EqualTo("Helios Suns Matica MB"));
                Assert.That(actual.HomeTeam.LeagueId, Is.Null);
                Assert.That(actual.HomeTeam.SeasonId, Is.EqualTo(110309));
                Assert.That(actual.HomeTeam.Score, Is.EqualTo(106));
                Assert.That(actual.AwayTeam.TeamId, Is.EqualTo(4630007));
                Assert.That(actual.AwayTeam.Name, Is.EqualTo("Miklavž"));
                Assert.That(actual.AwayTeam.LeagueId, Is.Null);
                Assert.That(actual.AwayTeam.SeasonId, Is.EqualTo(110309));
                Assert.That(actual.AwayTeam.Score, Is.EqualTo(33));
                Assert.That(actual.Arena.Id, Is.EqualTo(7053));
                Assert.That(actual.Arena.Name, Is.EqualTo("ŠD Domžale"));
            }
        }
            [TestFixture]
        public class ExtractTeamData: KzsStructureParserTest
        {
            [Test]
            public void GivenSampleData_ParseCorrectly()
            {
                var html = new HtmlDocument();
                html.LoadHtml("<a href=\"http://www.kzs.si/incl?id=967&amp;team_id=195903&amp;league_id=undefined&amp;season_id=102583\" season_id=\"102583\" team_id=\"195903\" client_hash=\"39f56437f972dc4ca91d2c997f874dcfc232a688\" id=\"a-0.489635001538898660\" onlyurl=\"1\">Zlatorog</a>");

                var actual = KzsStructureParser.ExtractTeamData(html.DocumentNode);

                Assert.That(actual.TeamName, Is.EqualTo("Zlatorog"));
                Assert.That(actual.TeamId, Is.EqualTo(195903));
                Assert.That(actual.Season, Is.EqualTo(102583));
                Assert.That(actual.League.HasValue, Is.False);
            }
        }
        [TestFixture]
        public class ExtractPairAsInt : KzsStructureParserTest
        {
            [Test]
            public void WhenSourceHasValues_ParseCorrectly()
            {
                var html = new HtmlDocument();
                html.LoadHtml("<td>171<span></span>/<span></span>141</td>");

                var actual = KzsStructureParser.ExtractPairAsInt(html.DocumentNode);

                Assert.That(actual.Left, Is.EqualTo(171));
                Assert.That(actual.Right, Is.EqualTo(141));
            }
            [Test]
            public void WhenSourceHasNull_ParseCorrectly()
            {
                var html = new HtmlDocument();
                html.LoadHtml("<td>-/-</td>");

                var actual = KzsStructureParser.ExtractPairAsInt(html.DocumentNode);

                Assert.That(actual.Left.HasValue, Is.False);
                Assert.That(actual.Right.HasValue, Is.False);
            }
        }
        [TestFixture]
        public class ExtractPairAsDecimal : KzsStructureParserTest
        {
            [Test]
            public void WhenSourceHasValues_ParseCorrectly()
            {
                var html = new HtmlDocument();
                html.LoadHtml("<td>85.5<span></span>/<span></span>50.0</td>");

                var actual = KzsStructureParser.ExtractPairAsDecimal(html.DocumentNode);

                Assert.That(actual.Left, Is.EqualTo(85.5m));
                Assert.That(actual.Right, Is.EqualTo(50m));
            }
            [Test]
            public void WhenSourceHasNull_ParseCorrectly()
            {
                var html = new HtmlDocument();
                html.LoadHtml("<td>-/-</td>");

                var actual = KzsStructureParser.ExtractPairAsDecimal(html.DocumentNode);

                Assert.That(actual.Left.HasValue, Is.False);
                Assert.That(actual.Right.HasValue, Is.False);
            }
        }

        [TestFixture]
        public class GetTeamDataAsync : KzsStructureParserTest
        {
            // ![](A6AE0C321B566CC5879B0886165AD67E.png)
            HtmlNode node => GetRootNode("team_root");

            [Test]
            public async Task ExtractsSampleData()
            {
                var actual = await KzsStructureParser.GetTeamDataAsync(node, default);

                Assert.That(actual.Name, Is.EqualTo("Nova Gorica mladi"));
                Assert.That(actual.ShortName, Is.EqualTo("Nova Gorica"));
                Assert.That(actual.City, Is.EqualTo("Nova Gorica"));
                Assert.That(actual.Coach, Is.Null);
                Assert.That(actual.Arena, Is.EqualTo(new Arena(7593, "ŠD OŠ Milojke Štrukelj", "http://www.kzs.si/incl?id=119&arena_id=7593")));
            }
        }
        [TestFixture]
        public class GetLastTeamResultsObsoleteAsync: KzsStructureParserTest
        {
            // ![](F937C186BDD85A7BE8D7A8EC74B98D7C.png)
            //DomResultItem domItem => new DomResultItem("Root", GetSampleContent("team_root"));
            //[Test]
            //public async Task ExtractsSampleData()
            //{
            //    var actual = await KzsStructureParser.GetLastTeamResultsAsync(GetRootNode(domItem), default);

            //    Assert.That(actual.Length, Is.EqualTo(3));
            //}
        }
        [TestFixture]
        public class GetLastTeamResultsAsync : KzsStructureParserTest
        {
            // ![](A44C5BADCAC053328BDDE4BB3E4404CE.png)
            HtmlNode node => GetRootNode("team_fixtures_and_results");
            [Test]
            public async Task ExtractsSampleData_CountIsCorrect()
            {
                var actual = await KzsStructureParser.GetLastTeamResultsAsync(4596801, node, default);

                Assert.That(actual.Length, Is.EqualTo(6));
            }
            
            [Test]
            public async Task WhenExtracting_FirstEntryIsCorrect()
            {
                var actual = await KzsStructureParser.GetLastTeamResultsAsync(4596801, node, default);

                var first = actual[0];

                Assert.That(first.IsHomeGame, Is.False);
                Assert.That(first.GameId, Is.EqualTo(4677211));
            }
        }
        [TestFixture]
        public class GetTeamGameResult: KzsStructureParserTest
        {
            // ![](E7E718F16D2C6C2E4C81BDE2F8586801.png)
            HtmlNode node => GetRootNode("sample_team_game_result");
            [Test]
            public void ExtractsSampleGameCorrectly()
            {
                var actual = KzsStructureParser.GetTeamGameResult(4596801, node.SelectSingleNode("tr"));

                Assert.That(actual.IsHomeGame, Is.False);
                Assert.That(actual.GameId, Is.EqualTo(4677211));
                Assert.That(actual.Date, Is.EqualTo(new DateTimeOffset(2019, 9, 28, 17, 00, 00, actual.Date.Offset)));
                Assert.That(actual.OpponentName, Is.EqualTo("GGD Šenčur"));
                Assert.That(actual.OpponentScore, Is.EqualTo(72));
                Assert.That(actual.HomeScore, Is.EqualTo(66));
            }
        }
        [TestFixture]
        public class GetTeamGameFixture : KzsStructureParserTest
        {
            // ![](FEFBFE953C56F70687F0C804F59EA28F.png)
            HtmlNode node => GetRootNode("sample_team_game_fixture");
            [Test]
            public void ExtractsSampleGameCorrectly()
            {
                var actual = KzsStructureParser.GetTeamGameFixture(4596801, node.SelectSingleNode("tr"));

                Assert.That(actual.IsHomeGame, Is.False);
                Assert.That(actual.GameId, Is.EqualTo(5028091));
                Assert.That(actual.Date, Is.EqualTo(new DateTimeOffset(2019, 11, 17, 19, 00, 00, actual.Date.Offset)));
                Assert.That(actual.OpponentName, Is.EqualTo("Šentvid - Ljubljana"));
            }
        }
        //[TestFixture]
        //public class GetTeamGameResult : KzsStructureParserTest
        //{
        //    DomResultItem domItem => new DomResultItem("Root", GetSampleContent("team_root"));
        //    [Test]
        //    public void ExtractsSampleData()
        //    {
        //var html = new HtmlDocument();
        //html.LoadHtml(domItem.Content);
        //var root = html.DocumentNode.SelectSingleNode("//table[@id='mbt-v2-team-home-results-table']/tbody/tr[1]");

        //var actual = KzsStructureParser.GetTeamGameResult(root);

        //Assert.That(actual.Date, Is.EqualTo(DateTimeOffset.Parse("13.10.2018 9:30", KzsStructureParser.SloveneCulture)));
        //Assert.That(actual.GameId, Is.EqualTo(4240435));
        //Assert.That(actual.IsHomeGame, Is.False);
        //Assert.That(actual.HomeScore, Is.EqualTo(85));
        //Assert.That(actual.OpponentScore, Is.EqualTo(49));
        //Assert.That(actual.OpponentId, Is.EqualTo(196003));
        //Assert.That(actual.OpponentName, Is.EqualTo("Stražišče Kranj"));
        //    }
        //}
        [TestFixture]
        public class GetShortGameFixturesAsync : KzsStructureParserTest
        {
            // ![](F937C186BDD85A7BE8D7A8EC74B98D7C.png)
            //HtmlNode node = GetRootNode("team_root");
            //[Test]
            //public async Task ExtractsSampleData()
            //{
            //    var actual = await KzsStructureParser.GetShortGameFixturesAsync(node, default);

            //    Assert.That(actual.Length, Is.EqualTo(3));
            //}
        }
        [TestFixture]
        public class GetShortGameFixture : KzsStructureParserTest
        {
            //DomResultItem domItem => new DomResultItem("Root", GetSampleContent("team_root"));
            //[Test]
            //public void ExtractsSampleData()
            //{
            //    var html = new HtmlDocument();
            //    html.LoadHtml(domItem.Content);
            //    var root = html.DocumentNode.SelectSingleNode("//table[@id='mbt-v2-team-home-schedule-table']/tbody/tr[1]");

            //    var actual = KzsStructureParser.GetShortGameFixture(root);

            //    Assert.That(actual.Date, Is.EqualTo(DateTimeOffset.Parse("20.10.2018 12:00", KzsStructureParser.SloveneCulture)));
            //    Assert.That(actual.GameId, Is.EqualTo(4240439));
            //    Assert.That(actual.IsHomeGame, Is.True);
            //    Assert.That(actual.OpponentId, Is.EqualTo(195973));
            //    Assert.That(actual.OpponentName, Is.EqualTo("Grosuplje A"));
            //}
        }
        [TestFixture]
        public class GetPlayer : KzsStructureParserTest
        {
            // ![](7D3D6999B2C6EB2438F10D8D84EF88DB.png)
            DomResultItem domItem => new DomResultItem("Root", GetSampleContent("team_players"));
            [Test]
            public void ExtractsPlayerWithoutHeight()
            {
                var html = new HtmlDocument();
                html.LoadHtml(domItem.Content);
                var root = html.DocumentNode.SelectSingleNode("//table[@id='mbt-v2-team-roster-table']/tbody/tr[1]");

                var actual = KzsStructureParser.GetPlayer(root);

                Assert.That(actual.Number, Is.Null);
                Assert.That(actual.FullName, Is.EqualTo("Matej Bunc"));
                Assert.That(actual.BirthYear, Is.EqualTo(2002));
                Assert.That(actual.NationalityCode, Is.EqualTo("SI"));
                Assert.That(actual.Nationality, Is.EqualTo("Slovenija"));
                Assert.That(actual.Position, Is.Null);
                Assert.That(actual.Height.HasValue, Is.False);
            }
            [Test]
            public void ExtractsPlayerWithHeight()
            {
                var html = new HtmlDocument();
                html.LoadHtml(domItem.Content);
                var root = html.DocumentNode.SelectSingleNode("//table[@id='mbt-v2-team-roster-table']/tbody/tr[9]");

                var actual = KzsStructureParser.GetPlayer(root);

                Assert.That(actual.Number, Is.Null);
                Assert.That(actual.FullName, Is.EqualTo("Luka Ščuka"));
                Assert.That(actual.BirthYear, Is.EqualTo(2002));
                Assert.That(actual.NationalityCode, Is.EqualTo("SI"));
                Assert.That(actual.Nationality, Is.EqualTo("Slovenija"));
                Assert.That(actual.Position, Is.Null);
                Assert.That(actual.Height, Is.EqualTo(201));
            }
        }
        [TestFixture]
        public class GetPlayersAsync : KzsStructureParserTest
        {
            // ![](7D3D6999B2C6EB2438F10D8D84EF88DB.png)
            HtmlNode node = GetRootNode("team_players");
            [Test]
            public async Task ExtractsSampleData()
            {
                var actual = await KzsStructureParser.GetPlayersAsync(node, default);

                Assert.That(actual.Length, Is.EqualTo(11));
            }
        }
        [TestFixture]
        public class IsFixturesTabActive: KzsStructureParserTest
        {
            //DomResultItem scoresDomItem => new DomResultItem("Root", GetSampleContent(LeagueNoFixtures));
            //DomResultItem fixturesDomItem => new DomResultItem("Root", GetSampleContent(LeagueFixtures));
            //[Test]
            //public void WhenFixturesPageAndOnlyResults_FixturesTabIsNotActive()
            //{
            //    var html = new HtmlDocument();
            //    html.LoadHtml(scoresDomItem.Content);

            //    var actual = KzsStructureParser.IsFixturesTabActive(html);

            //    Assert.That(actual, Is.False);
            //}
            //[Test]
            //public void WhenFixturesPage_FixturesTabIsActive()
            //{
            //    var html = new HtmlDocument();
            //    html.LoadHtml(fixturesDomItem.Content);

            //    var actual = KzsStructureParser.IsFixturesTabActive(html);

            //    Assert.That(actual, Is.True);
            //}
        }
        [TestFixture]
        public class ExtractSeasonId : KzsCommunicationParserTest
        {
            [Test]
            public void WhenGivenSample_ExtractsId()
            {
                string html = GetSampleContent(SampleHtmlForSeasonId);

                int? actual = KzsStructureParser.ExtractSeasonId(html);

                Assert.That(actual, Is.EqualTo(108919));
            }
        }
    }
}
