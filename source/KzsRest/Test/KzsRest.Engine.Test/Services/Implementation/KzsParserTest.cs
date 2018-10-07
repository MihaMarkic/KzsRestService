using AutoFixture;
using HtmlAgilityPack;
using KzsRest.Engine.Services.Abstract;
using KzsRest.Engine.Services.Implementation;
using NSubstitute;
using NUnit.Framework;
using System.IO;
using System.Threading.Tasks;

namespace KzsRest.Engine.Test.Services.Implementation
{
    public class KzsParserTest: BaseTest<KzsParser>
    {
        internal const string U17_Male_A = "u17_male_a";
        internal const string SampleStandingsTable = "sample_standings_table";
        internal static string GetSampleContent(string file) => File.ReadAllText(Path.Combine("Samples", $"{file}.html"));
        [TestFixture]
        public class GetStandingsAsync: KzsParserTest
        {
            [Test]
            public async Task WhenNoData_ReturnsEmptyArray()
            {
                var domCache = fixture.Freeze<IDomCache>();
                domCache.GetDomForAsync(default, default, default).ReturnsForAnyArgs("");

                var actual = await Target.GetStandingsAsync(default, default);

                Assert.That(actual.Length, Is.EqualTo(0));
            }
            [Test]
            public async Task WhenSixGroups_ReturnsAllSix()
            {
                var domCache = fixture.Freeze<IDomCache>();
                domCache.GetDomForAsync(default, default, default).ReturnsForAnyArgs(GetSampleContent(U17_Male_A));

                var actual = await Target.GetStandingsAsync(default, default);

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
    }
}
