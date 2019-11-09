using System;
using Righthand.Immutable;

namespace KzsRest.Models
{
    /// <summary>
    /// Auto generated and manually adjusted
    /// </summary>
    public class TeamGameResult : TeamGameEntry
    {
        public int HomeScore { get; }
        public int OpponentScore { get; }

        public TeamGameResult(int gameId, DateTimeOffset date, bool isHomeGame, int homeScore, int opponentScore, int opponentId, string opponentName)
            : base(gameId, date, isHomeGame, opponentId, opponentName)
        {
            HomeScore = homeScore;
            OpponentScore = opponentScore;
        }

        public TeamGameResult Clone(Param<int>? gameId = null, Param<DateTimeOffset>? date = null, Param<bool>? isHomeGame = null, Param<int>? homeScore = null, Param<int>? opponentScore = null, Param<int>? opponentId = null, Param<string>? opponentName = null)
        {
            return new TeamGameResult(gameId.HasValue ? gameId.Value.Value : GameId,
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
            var o = (TeamGameResult)obj;
            return base.Equals(obj) && Equals(HomeScore, o.HomeScore) && Equals(OpponentScore, o.OpponentScore);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hash = base.GetHashCode();
                hash = hash * 37 + HomeScore.GetHashCode();
                hash = hash * 37 + OpponentScore.GetHashCode();
                return hash;
            }
        }
    }
}
