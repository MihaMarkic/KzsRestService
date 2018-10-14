using System;

namespace KzsRest.Engine.Models
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
    }
}
