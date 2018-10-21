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
    public class KzsParserTest: BaseTest<KzsParser>
    {
        internal const string U17_Male_A = "u17_male_a";
        internal const string SampleStandingsTable = "sample_standings_table";
        internal const string LeagueFixtures = "league_fixtures";
        internal const string LeagueResults = "league_results";
        internal static string GetSampleContent(string file) => File.ReadAllText(Path.Combine("Samples", $"{file}.html"));
        [TestFixture]
        public class GetLeagueOverviewAsync : BaseTest<KzsParser>
        {
            [Test]
            public void WhenNoData_ThrowsException()
            {
                var domRetriever = fixture.Freeze<IDomRetriever>();
                domRetriever.GetDomForAsync(default, default).ReturnsForAnyArgs(new DomResultItem[0]);

                Assert.ThrowsAsync<Exception>(async () => await Target.GetLeagueOverviewAsync(default, default));
            }
        }
        [TestFixture]
        public class GetStandingsAsync: KzsParserTest
        {
            [Test]
            public void WhenNoData_ThrowsException()
            {
                Assert.Throws<Exception>(() => KzsParser.GetStandings(new HtmlDocument(), default));
            }
            [Test]
            public void WhenSixGroups_ReturnsAllSix()
            {
                //var domRetriever = fixture.Freeze<IDomRetriever>();
                //domRetriever.GetDomForAsync(default, default, default).ReturnsForAnyArgs(
                //    new DomResultItem[] {
                //        new DomResultItem(id: "Root", GetSampleContent(U17_Male_A))
                //    });
                var html = new HtmlDocument();
                html.LoadHtml(GetSampleContent(U17_Male_A));

                var actual = KzsParser.GetStandings(html, default);

                Assert.That(actual.Length, Is.EqualTo(6));
            }
        }
        [TestFixture]
        public class ExtractStanding: KzsParserTest
        {
            // ![](FE341BE5FF5472C7DDF163BAC18FBAF8.png)
            [Test]
            public void WhenSampleInput_ParsesTitleCorrectly()
            {
                var html = new HtmlDocument();
                html.LoadHtml(GetSampleContent(SampleStandingsTable));

                var actual = KzsParser.ExtractStanding(html.DocumentNode.SelectSingleNode("/body/div"));

                Assert.That(actual.Name, Is.EqualTo("1. del - Skupina 2"));
            }
            [Test]
            public void WhenSampleInputHasTwoRows_NumberOfEntriesIsTwo()
            {
                var html = new HtmlDocument();
                html.LoadHtml(GetSampleContent(SampleStandingsTable));

                var actual = KzsParser.ExtractStanding(html.DocumentNode.SelectSingleNode("/body/div"));

                Assert.That(actual.Entries.Length, Is.EqualTo(2));
            }
        }
        [TestFixture]
        public class ExtractStandingsEntry : KzsParserTest
        {
            [Test]
            public void RowIsParsedCorrectly()
            {
                var html = new HtmlDocument();
                html.LoadHtml(GetSampleContent(SampleStandingsTable));
                var node = html.DocumentNode.SelectSingleNode("//tbody/tr");

                var actual = KzsParser.ExtractStandingsEntry(node);

                Assert.That(actual.TeamId, Is.EqualTo(195903));
                Assert.That(actual.Season, Is.EqualTo(102583));
                Assert.That(actual.League.HasValue, Is.False);
                Assert.That(actual.Position, Is.EqualTo(1));
                Assert.That(actual.TeamName, Is.EqualTo("Zlatorog"));
                Assert.That(actual.Games, Is.EqualTo(2));
                Assert.That(actual.Won, Is.EqualTo(2));
                Assert.That(actual.Lost, Is.EqualTo(0));
                Assert.That(actual.Points, Is.EqualTo(4));
                Assert.That(actual.PointsMade, Is.EqualTo(171));
                Assert.That(actual.PointsReceived, Is.EqualTo(141));
                Assert.That(actual.PointsDifference, Is.EqualTo(30));
                Assert.That(actual.PointsMadePerGame, Is.EqualTo(85.5m));
                Assert.That(actual.PointsReceivedPerGame, Is.EqualTo(70.5m));
                Assert.That(actual.HomeWins, Is.EqualTo(1));
                Assert.That(actual.HomeDefeats, Is.EqualTo(0));
                Assert.That(actual.AwayWins, Is.EqualTo(1));
                Assert.That(actual.AwayDefeats, Is.EqualTo(0));
                Assert.That(actual.HomePointsMadePerGame, Is.EqualTo(79m));
                Assert.That(actual.HomePointsReceivedPerGame, Is.EqualTo(67m));
                Assert.That(actual.AwayPointsMadePerGame, Is.EqualTo(92m));
                Assert.That(actual.AwayPointsReceivedPerGame, Is.EqualTo(74m));
                Assert.That(actual.LastFiveGamesWon, Is.EqualTo(2));
                Assert.That(actual.LastFiveGamesLost, Is.EqualTo(0));
                Assert.That(actual.LastTenGamesWon, Is.EqualTo(2));
                Assert.That(actual.LastTenGamesLost, Is.EqualTo(0));
                Assert.That(actual.GameSeries, Is.EqualTo(2));
                Assert.That(actual.HomeGameSeries, Is.EqualTo(1));
                Assert.That(actual.AwayGameSeries, Is.EqualTo(1));
                Assert.That(actual.FivePointsWins, Is.EqualTo(0));
                Assert.That(actual.FivePointsDefeats, Is.EqualTo(0));
            }
        }
        [TestFixture]
        public class ExtractGameFixture : KzsParserTest
        {
            // ![](5ED4E42C5E64CC83B5B675018DF495D0.png)
            protected HtmlNode fixturesTable;
            protected HtmlNode resultsTable;
            [SetUp]
            public new void SetUp()
            {
                var html = new HtmlDocument();
                html.LoadHtml(GetSampleContent(LeagueFixtures));
                fixturesTable = html.DocumentNode.SelectSingleNode("//table[@id='mbt-v2-schedule-table']");
                html = new HtmlDocument();
                html.LoadHtml(GetSampleContent(LeagueResults));
                resultsTable = html.DocumentNode.SelectSingleNode("//table[@id='mbt-v2-schedule-table']");
            }
            [Test]
            public void GivenSampleDataFromFirstRow_ParsesFixturesCorrectly()
            {
                var tr = fixturesTable.SelectSingleNode("tbody/tr[1]");

                var actual = KzsParser.ExtractGameFixtureOrResult(tr, includeResults: false);

                Assert.That(actual.PlayDay, Is.EqualTo(5));
                Assert.That(actual.GameId, Is.EqualTo(4240363));
                Assert.That(actual.Date, Is.EqualTo(DateTimeOffset.Parse("20.10.2018 10:00", KzsParser.SloveneCulture)));
                Assert.That(actual.HomeTeam.TeamId, Is.EqualTo(196363));
                Assert.That(actual.HomeTeam.Name, Is.EqualTo("Calcit Basketball"));
                Assert.That(actual.HomeTeam.LeagueId, Is.Null);
                Assert.That(actual.HomeTeam.SeasonId, Is.EqualTo(102583));
                Assert.That(actual.HomeTeam.Score, Is.Null);
                Assert.That(actual.AwayTeam.TeamId, Is.EqualTo(195883));
                Assert.That(actual.AwayTeam.Name, Is.EqualTo("Konjice - Zreče"));
                Assert.That(actual.AwayTeam.LeagueId, Is.Null);
                Assert.That(actual.AwayTeam.SeasonId, Is.EqualTo(102583));
                Assert.That(actual.AwayTeam.Score, Is.Null);
                Assert.That(actual.Arena.Id, Is.EqualTo(8553));
                Assert.That(actual.Arena.Name, Is.EqualTo("ŠD Kamnik"));
            }
            // ![](023BE71E0BC5C7F6D4F4ADA38FDE6422.png)
            [Test]
            public void GivenSampleDataFromFirstRow_ParsesResultsCorrectly()
            {
                var tr = resultsTable.SelectSingleNode("tbody/tr[1]");

                var actual = KzsParser.ExtractGameFixtureOrResult(tr, includeResults: true);

                Assert.That(actual.PlayDay, Is.EqualTo(5));
                Assert.That(actual.GameId, Is.EqualTo(4240341));
                Assert.That(actual.Date, Is.EqualTo(DateTimeOffset.Parse("20.10.2018 12:45", KzsParser.SloveneCulture)));
                Assert.That(actual.HomeTeam.TeamId, Is.EqualTo(195923));
                Assert.That(actual.HomeTeam.Name, Is.EqualTo("Petrol Olimpija A"));
                Assert.That(actual.HomeTeam.LeagueId, Is.Null);
                Assert.That(actual.HomeTeam.SeasonId, Is.EqualTo(102583));
                Assert.That(actual.HomeTeam.Score, Is.EqualTo(87));
                Assert.That(actual.AwayTeam.TeamId, Is.EqualTo(196413));
                Assert.That(actual.AwayTeam.Name, Is.EqualTo("Ljubljana A"));
                Assert.That(actual.AwayTeam.LeagueId, Is.Null);
                Assert.That(actual.AwayTeam.SeasonId, Is.EqualTo(102583));
                Assert.That(actual.AwayTeam.Score, Is.EqualTo(91));
                Assert.That(actual.Arena.Id, Is.EqualTo(7683));
                Assert.That(actual.Arena.Name, Is.EqualTo("Stožice, mala dv."));
            }
        }
        [TestFixture]
        public class ExtractTeamData: KzsParserTest
        {
            [Test]
            public void GivenSampleData_ParseCorrectly()
            {
                var html = new HtmlDocument();
                html.LoadHtml("<a href=\"http://www.kzs.si/incl?id=967&amp;team_id=195903&amp;league_id=undefined&amp;season_id=102583\" season_id=\"102583\" team_id=\"195903\" client_hash=\"39f56437f972dc4ca91d2c997f874dcfc232a688\" id=\"a-0.489635001538898660\" onlyurl=\"1\">Zlatorog</a>");

                var actual = KzsParser.ExtractTeamData(html.DocumentNode);

                Assert.That(actual.TeamName, Is.EqualTo("Zlatorog"));
                Assert.That(actual.TeamId, Is.EqualTo(195903));
                Assert.That(actual.Season, Is.EqualTo(102583));
                Assert.That(actual.League.HasValue, Is.False);
            }
        }
        [TestFixture]
        public class ExtractPairAsInt : KzsParserTest
        {
            [Test]
            public void WhenSourceHasValues_ParseCorrectly()
            {
                var html = new HtmlDocument();
                html.LoadHtml("<td>171<span></span>/<span></span>141</td>");

                var actual = KzsParser.ExtractPairAsInt(html.DocumentNode);

                Assert.That(actual.Left, Is.EqualTo(171));
                Assert.That(actual.Right, Is.EqualTo(141));
            }
            [Test]
            public void WhenSourceHasNull_ParseCorrectly()
            {
                var html = new HtmlDocument();
                html.LoadHtml("<td>-/-</td>");

                var actual = KzsParser.ExtractPairAsInt(html.DocumentNode);

                Assert.That(actual.Left.HasValue, Is.False);
                Assert.That(actual.Right.HasValue, Is.False);
            }
        }
        [TestFixture]
        public class ExtractPairAsDecimal : KzsParserTest
        {
            [Test]
            public void WhenSourceHasValues_ParseCorrectly()
            {
                var html = new HtmlDocument();
                html.LoadHtml("<td>85.5<span></span>/<span></span>50.0</td>");

                var actual = KzsParser.ExtractPairAsDecimal(html.DocumentNode);

                Assert.That(actual.Left, Is.EqualTo(85.5m));
                Assert.That(actual.Right, Is.EqualTo(50m));
            }
            [Test]
            public void WhenSourceHasNull_ParseCorrectly()
            {
                var html = new HtmlDocument();
                html.LoadHtml("<td>-/-</td>");

                var actual = KzsParser.ExtractPairAsDecimal(html.DocumentNode);

                Assert.That(actual.Left.HasValue, Is.False);
                Assert.That(actual.Right.HasValue, Is.False);
            }
        }

        [TestFixture]
        public class GetTeamDataAsync : KzsParserTest
        {
            // ![](A6AE0C321B566CC5879B0886165AD67E.png)
            DomResultItem domItem => new DomResultItem("Root", GetSampleContent("team_root"));

            [Test]
            public async Task ExtractsSampleData()
            {
                var actual = await KzsParser.GetTeamDataAsync(domItem, default);

                Assert.That(actual.Name, Is.EqualTo("Nova Gorica mladi"));
                Assert.That(actual.ShortName, Is.EqualTo("Nova Gorica"));
                Assert.That(actual.City, Is.EqualTo("Nova Gorica"));
                Assert.That(actual.Coach, Is.Null);
                Assert.That(actual.Arena, Is.EqualTo(new Arena(7593, "ŠD OŠ Milojke Štrukelj", "http://www.kzs.si/incl?id=119&arena_id=7593")));
            }
        }
        [TestFixture]
        public class GetLastTeamResultsAsync: KzsParserTest
        {
            // ![](F937C186BDD85A7BE8D7A8EC74B98D7C.png)
            DomResultItem domItem => new DomResultItem("Root", GetSampleContent("team_root"));
            [Test]
            public async Task ExtractsSampleData()
            {
                var actual = await KzsParser.GetLastTeamResultsAsync(domItem, default);

                Assert.That(actual.Length, Is.EqualTo(3));
            }
        }
        [TestFixture]
        public class GetTeamGameResult : KzsParserTest
        {
            DomResultItem domItem => new DomResultItem("Root", GetSampleContent("team_root"));
            [Test]
            public void ExtractsSampleData()
            {
                var html = new HtmlDocument();
                html.LoadHtml(domItem.Content);
                var root = html.DocumentNode.SelectSingleNode("//table[@id='mbt-v2-team-home-results-table']/tbody/tr[1]");

                var actual = KzsParser.GetTeamGameResult(root);

                Assert.That(actual.Date, Is.EqualTo(DateTimeOffset.Parse("13.10.2018 9:30", KzsParser.SloveneCulture)));
                Assert.That(actual.GameId, Is.EqualTo(4240435));
                Assert.That(actual.IsHomeGame, Is.False);
                Assert.That(actual.HomeScore, Is.EqualTo(85));
                Assert.That(actual.OpponentScore, Is.EqualTo(49));
                Assert.That(actual.OpponentId, Is.EqualTo(196003));
                Assert.That(actual.OpponentName, Is.EqualTo("Stražišče Kranj"));
            }
        }
        [TestFixture]
        public class GetShortGameFixturesAsync : KzsParserTest
        {
            // ![](F937C186BDD85A7BE8D7A8EC74B98D7C.png)
            DomResultItem domItem => new DomResultItem("Root", GetSampleContent("team_root"));
            [Test]
            public async Task ExtractsSampleData()
            {
                var actual = await KzsParser.GetShortGameFixturesAsync(domItem, default);

                Assert.That(actual.Length, Is.EqualTo(3));
            }
        }
        [TestFixture]
        public class GetShortGameFixture : KzsParserTest
        {
            DomResultItem domItem => new DomResultItem("Root", GetSampleContent("team_root"));
            [Test]
            public void ExtractsSampleData()
            {
                var html = new HtmlDocument();
                html.LoadHtml(domItem.Content);
                var root = html.DocumentNode.SelectSingleNode("//table[@id='mbt-v2-team-home-schedule-table']/tbody/tr[1]");

                var actual = KzsParser.GetShortGameFixture(root);

                Assert.That(actual.Date, Is.EqualTo(DateTimeOffset.Parse("20.10.2018 12:00", KzsParser.SloveneCulture)));
                Assert.That(actual.GameId, Is.EqualTo(4240439));
                Assert.That(actual.IsHomeGame, Is.True);
                Assert.That(actual.OpponentId, Is.EqualTo(195973));
                Assert.That(actual.OpponentName, Is.EqualTo("Grosuplje A"));
            }
        }
        [TestFixture]
        public class GetPlayer : KzsParserTest
        {
            // ![](7D3D6999B2C6EB2438F10D8D84EF88DB.png)
            DomResultItem domItem => new DomResultItem("Root", GetSampleContent("team_players"));
            [Test]
            public void ExtractsPlayerWithoutHeight()
            {
                var html = new HtmlDocument();
                html.LoadHtml(domItem.Content);
                var root = html.DocumentNode.SelectSingleNode("//table[@id='mbt-v2-team-roster-table']/tbody/tr[1]");

                var actual = KzsParser.GetPlayer(root);

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

                var actual = KzsParser.GetPlayer(root);

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
        public class GetPlayersAsync : KzsParserTest
        {
            // ![](7D3D6999B2C6EB2438F10D8D84EF88DB.png)
            DomResultItem domItem => new DomResultItem("Root", GetSampleContent("team_players"));
            [Test]
            public async Task ExtractsSampleData()
            {
                var actual = await KzsParser.GetPlayersAsync(domItem, default);

                Assert.That(actual.Length, Is.EqualTo(11));
            }
        }
    }
}
