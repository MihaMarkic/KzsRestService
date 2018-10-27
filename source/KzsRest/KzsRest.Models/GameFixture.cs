using System;
using System.Diagnostics;
using Righthand.Immutable;

namespace KzsRest.Models
{
    [DebuggerDisplay("{Date.Date} {HomeTeam.Name} {HomeTeam.Score}:{AwayTeam.Score} {AwayTeam.Name}")]
    public class GameFixture
    {
        public int PlayDay { get; }
        public int GameId { get; }
        public DateTimeOffset Date { get; }
        public TeamFixture HomeTeam { get; }
        public TeamFixture AwayTeam { get; }
        public Arena Arena { get; }

        public GameFixture(int playDay, int gameId, DateTimeOffset date, TeamFixture homeTeam, TeamFixture awayTeam, Arena arena)
        {
            PlayDay = playDay;
            GameId = gameId;
            Date = date;
            HomeTeam = homeTeam;
            AwayTeam = awayTeam;
            Arena = arena;
        }

        public GameFixture Clone(Param<int>? playDay = null, Param<int>? gameId = null, Param<DateTimeOffset>? date = null, Param<TeamFixture>? homeTeam = null, Param<TeamFixture>? awayTeam = null, Param<Arena>? arena = null)
        {
            return new GameFixture(playDay.HasValue ? playDay.Value.Value : PlayDay,
				gameId.HasValue ? gameId.Value.Value : GameId,
				date.HasValue ? date.Value.Value : Date,
				homeTeam.HasValue ? homeTeam.Value.Value : HomeTeam,
				awayTeam.HasValue ? awayTeam.Value.Value : AwayTeam,
				arena.HasValue ? arena.Value.Value : Arena);
        }

        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType()) return false;
            var o = (GameFixture)obj;
            return Equals(PlayDay, o.PlayDay) && Equals(GameId, o.GameId) && Equals(Date, o.Date) && Equals(HomeTeam, o.HomeTeam) && Equals(AwayTeam, o.AwayTeam) && Equals(Arena, o.Arena);}

        public override int GetHashCode()
        {
            unchecked
			{
				int hash = 23;
				hash = hash * 37 + PlayDay.GetHashCode();
				hash = hash * 37 + GameId.GetHashCode();
				hash = hash * 37 + Date.GetHashCode();
				hash = hash * 37 + (HomeTeam != null ? HomeTeam.GetHashCode() : 0);
				hash = hash * 37 + (AwayTeam != null ? AwayTeam.GetHashCode() : 0);
				hash = hash * 37 + (Arena != null ? Arena.GetHashCode() : 0);
				return hash;
			}
        }
    }
}
