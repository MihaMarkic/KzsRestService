using System;
using System.Diagnostics;

namespace KzsRest.Models
{
    [DebuggerDisplay("{Date.Date} {HomeTeam.Name} - {AwayTeam.Name} {HomeTeam.Score}:{AwayTeam.Score}")]
    public class GameResult : GameData
    {
        public GameResult(int playDay, int gameId, int seasonId, DateTimeOffset date, TeamFixture homeTeam, TeamFixture awayTeam, Arena arena)
            : base(playDay, gameId, seasonId, date, homeTeam, awayTeam, arena)
        { }
    }
}
