using System;

namespace KzsRest.Models
{
    public abstract class TeamGameEntry
    {
        public int GameId { get; }
        public DateTimeOffset Date { get; }
        public bool IsHomeGame { get; }
        public int OpponentId { get; }
        public string OpponentName { get; }

        public TeamGameEntry(int gameId, DateTimeOffset date, bool isHomeGame, int opponentId, string opponentName)
        {
            GameId = gameId;
            Date = date;
            IsHomeGame = isHomeGame;
            OpponentId = opponentId;
            OpponentName = opponentName;
        }

        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType()) return false;
            var o = (TeamGameEntry)obj;
            return Equals(GameId, o.GameId) && Equals(Date, o.Date) && Equals(IsHomeGame, o.IsHomeGame) && Equals(OpponentId, o.OpponentId) && Equals(OpponentName, o.OpponentName);
}

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
