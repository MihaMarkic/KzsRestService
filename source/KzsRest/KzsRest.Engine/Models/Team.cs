namespace KzsRest.Engine.Models
{
    public class Team
    {
        public Player[] Players { get; }
        public Team(Player[] players)
        {
            Players = players;
        }
    }
}
