using System;
using Righthand.Immutable;

namespace KzsRest.Models
{
    public class ShortGameFixture
    {
        public int GameId { get; }
        public DateTimeOffset Date { get; }
        public bool IsHomeGame { get; }
        public int OpponentId { get; }
        public string OpponentName { get; }

        public ShortGameFixture(int gameId, DateTimeOffset date, bool isHomeGame, int opponentTeamId, string opponentName)
        {
            GameId = gameId;
            Date = date;
            IsHomeGame = isHomeGame;
            OpponentId = opponentTeamId;
            OpponentName = opponentName;
        }

        public ShortGameFixture Clone(Param<int>? gameId = null, Param<DateTimeOffset>? date = null, Param<bool>? isHomeGame = null, Param<int>? opponentTeamId = null, Param<string>? opponentName = null)
        {
            return new ShortGameFixture(gameId.HasValue ? gameId.Value.Value : GameId,
				date.HasValue ? date.Value.Value : Date,
				isHomeGame.HasValue ? isHomeGame.Value.Value : IsHomeGame,
				opponentTeamId.HasValue ? opponentTeamId.Value.Value : OpponentId,
				opponentName.HasValue ? opponentName.Value.Value : OpponentName);
        }

        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType()) return false;
            var o = (ShortGameFixture)obj;
            return Equals(GameId, o.GameId) && Equals(Date, o.Date) && Equals(IsHomeGame, o.IsHomeGame) && Equals(OpponentId, o.OpponentId) && Equals(OpponentName, o.OpponentName);}

        public override int GetHashCode()
        {
            unchecked
			{
				int hash = 23;
				hash = hash * 37 + GameId.GetHashCode();
				hash = hash * 37 + Date.GetHashCode();
				hash = hash * 37 + IsHomeGame.GetHashCode();
				hash = hash * 37 + OpponentId.GetHashCode();
				hash = hash * 37 + (OpponentName != null ? OpponentName.GetHashCode() : 0);
				return hash;
			}
        }
    }
}
