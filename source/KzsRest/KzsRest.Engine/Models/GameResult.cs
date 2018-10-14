using System;
using Righthand.Immutable;

namespace KzsRest.Engine.Models
{
    public class GameResult
    {
        public int GameId { get; }
        public DateTimeOffset Date { get; }
        public bool IsHomeGame { get; }
        public int HomeScore { get; }
        public int OpponentScore { get; }
        public int OpponentId { get; }
        public string OpponentName { get; }

        public  GameResult  ( int gameId, DateTimeOffset date, bool isHomeGame, int homeScore, int opponentScore, int opponentId, string opponentName)
        {
            GameId = gameId;
            Date = date;
            IsHomeGame = isHomeGame;
            HomeScore = homeScore;
            OpponentScore = opponentScore;
            OpponentId = opponentId;
            OpponentName = opponentName;
        }

        public GameResult Clone(Param<int>? gameId = null, Param<DateTimeOffset>? date = null, Param<bool>? isHomeGame = null, Param<int>? homeScore = null, Param<int>? opponentScore = null, Param<int>? opponentId = null, Param<string>? opponentName = null)
        {
            return new GameResult(gameId.HasValue ? gameId.Value.Value : GameId,
				date.HasValue ? date.Value.Value : Date,
				isHomeGame.HasValue ? isHomeGame.Value.Value : IsHomeGame,
				homeScore.HasValue ? homeScore.Value.Value : HomeScore,
				opponentScore.HasValue ? opponentScore.Value.Value : OpponentScore,
				opponentId.HasValue ? opponentId.Value.Value : OpponentId,
				opponentName.HasValue ? opponentName.Value.Value : OpponentName);
        }

        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType()) return false;
            var o = (GameResult)obj;
            return Equals(GameId, o.GameId) && Equals(Date, o.Date) && Equals(IsHomeGame, o.IsHomeGame) && Equals(HomeScore, o.HomeScore) && Equals(OpponentScore, o.OpponentScore) && Equals(OpponentId, o.OpponentId) && Equals(OpponentName, o.OpponentName);}

        public override int GetHashCode()
        {
            unchecked
			{
				int hash = 23;
				hash = hash * 37 + GameId.GetHashCode();
				hash = hash * 37 + Date.GetHashCode();
				hash = hash * 37 + IsHomeGame.GetHashCode();
				hash = hash * 37 + HomeScore.GetHashCode();
				hash = hash * 37 + OpponentScore.GetHashCode();
				hash = hash * 37 + OpponentId.GetHashCode();
				hash = hash * 37 + (OpponentName != null ? OpponentName.GetHashCode() : 0);
				return hash;
			}
        }
    }
}
