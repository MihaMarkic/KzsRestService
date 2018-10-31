using System;
using System.Diagnostics;

namespace KzsRest.Models
{
    [DebuggerDisplay("{Date.Date} {HomeTeam.Name} - {AwayTeam.Name}")]
    public class GameFixture: GameData
    {
        public GameFixture(int playDay, int gameId, int seasonId, DateTimeOffset date, TeamFixture homeTeam, TeamFixture awayTeam, Arena arena)
            : base(playDay, gameId, seasonId, date, homeTeam, awayTeam, arena)
        {}
    }
}
