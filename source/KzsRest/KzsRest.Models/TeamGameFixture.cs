using Righthand.Immutable;
using System;

namespace KzsRest.Models
{
    public class TeamGameFixture : TeamGameEntry
    {
        public TeamGameFixture(int gameId, DateTimeOffset date, bool isHomeGame, int opponentId, string opponentName) : base(gameId, date, isHomeGame, opponentId, opponentName)
        {
        }
        public TeamGameFixture Clone(Param<int>? gameId = null, Param<DateTimeOffset>? date = null, Param<bool>? isHomeGame = null, Param<int>? opponentId = null, Param<string>? opponentName = null)
        {
            return new TeamGameFixture(gameId.HasValue ? gameId.Value.Value : GameId,
                date.HasValue ? date.Value.Value : Date,
                isHomeGame.HasValue ? isHomeGame.Value.Value : IsHomeGame,
                opponentId.HasValue ? opponentId.Value.Value : OpponentId,
                opponentName.HasValue ? opponentName.Value.Value : OpponentName);
        }
    }
}
