using System;

namespace KzsRest.Models
{
    public abstract class GameData
    {
        public int PlayDay { get; }
        public int GameId { get; }
        public int SeasonId { get; }
        public DateTimeOffset Date { get; }
        public TeamFixture HomeTeam { get; }
        public TeamFixture AwayTeam { get; }
        public Arena Arena { get; }

        public GameData(int playDay, int gameId, int seasonId, DateTimeOffset date, TeamFixture homeTeam, TeamFixture awayTeam, Arena arena)
        {
            PlayDay = playDay;
            GameId = gameId;
            SeasonId = seasonId;
            Date = date;
            HomeTeam = homeTeam;
            AwayTeam = awayTeam;
            Arena = arena;
        }

        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType()) return false;
            var o = (GameData)obj;
            return Equals(PlayDay, o.PlayDay) && Equals(GameId, o.GameId) && Equals(SeasonId, o.SeasonId) && Equals(Date, o.Date) && Equals(HomeTeam, o.HomeTeam) && Equals(AwayTeam, o.AwayTeam) && Equals(Arena, o.Arena);
}

        public override int GetHashCode()
        {
            unchecked
			{
				int hash = 23;
				hash = hash * 37 + PlayDay.GetHashCode();
				hash = hash * 37 + GameId.GetHashCode();
				hash = hash * 37 + SeasonId.GetHashCode();
				hash = hash * 37 + Date.GetHashCode();
				hash = hash * 37 + (HomeTeam != null ? HomeTeam.GetHashCode() : 0);
				hash = hash * 37 + (AwayTeam != null ? AwayTeam.GetHashCode() : 0);
				hash = hash * 37 + (Arena != null ? Arena.GetHashCode() : 0);
				return hash;
			}
        }
    }
}
