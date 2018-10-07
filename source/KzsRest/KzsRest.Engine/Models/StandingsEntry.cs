namespace KzsRest.Engine.Models
{
    public class StandingsEntry
    {
        public int TeamId { get; }
        public int? Season { get; }
        public int? League { get; }
        public int Position { get; }
        public string TeamName { get; }
        public int Games { get;}
        public int Won { get; }
        public int Lost { get; }
        public  int Points { get; }
        public int? PointsMade { get; }
        public int? PointsReceived { get; }
        public int PointsDifference { get; }
        public decimal PointsMadePerGame { get; }
        public decimal PointsReceivedPerGame { get; }
        public  int HomeWins { get; }
        public int HomeDefeats { get; }
        public int AwayWins { get; }
        public int AwayDefeats { get; }
        public decimal? HomePointsMadePerGame { get; }
        public decimal? HomePointsReceivedPerGame { get; }
        public decimal? AwayPointsMadePerGame { get; }
        public decimal? AwayPointsReceivedPerGame { get; }
        public int LastFiveGamesWon { get; }
        public int LastFiveGamesLost { get; }
        public int LastTenGamesWon { get; }
        public int LastTenGamesLost { get; }
        public int? GameSeries { get; }
        public int? HomeGameSeries { get; }
        public int? AwayGameSeries { get; }
        public int? FivePointsWins { get; }
        public int? FivePointsDefeats { get; }
        public StandingsEntry(int teamId, int? season, int? league, int position, string teamName, int games, int won, int lost, int points, int? pointsMade, int? pointsReceived, int pointsDifference, decimal pointsMadePerGame, decimal pointsReceivedPerGame, int homeWins, int homeDefeats, int awayWins, int awayDefeats, decimal? homePointsMadePerGame, decimal? homePointsReceivedPerGame, decimal? awayPointsMadePerGame, decimal? awayPointsReceivedPerGame, int lastFiveGamesWon, int lastFiveGamesLost, int lastTenGamesWon, int lastTenGamesLost, int? gameSeries, int? homeGameSeries, int? awayGameSeries,
            int? fivePointWins, int? fivePointsDefeats)
        {
            TeamId = teamId;
            Season = season;
            League = league;
            Position = position;
            TeamName = teamName;
            Games = games;
            Won = won;
            Lost = lost;
            Points = points;
            PointsMade = pointsMade;
            PointsReceived = pointsReceived;
            PointsDifference = pointsDifference;
            PointsMadePerGame = pointsMadePerGame;
            PointsReceivedPerGame = pointsReceivedPerGame;
            HomeWins = homeWins;
            HomeDefeats = homeDefeats;
            AwayWins = awayWins;
            AwayDefeats = awayDefeats;
            HomePointsMadePerGame = homePointsMadePerGame;
            HomePointsReceivedPerGame = homePointsReceivedPerGame;
            AwayPointsMadePerGame = awayPointsMadePerGame;
            AwayPointsReceivedPerGame = awayPointsReceivedPerGame;
            LastFiveGamesWon = lastFiveGamesWon;
            LastFiveGamesLost = lastFiveGamesLost;
            LastTenGamesWon = lastTenGamesWon;
            LastTenGamesLost = lastTenGamesLost;
            GameSeries = gameSeries;
            HomeGameSeries = homeGameSeries;
            AwayGameSeries = awayGameSeries;
            FivePointsWins = fivePointWins;
            FivePointsDefeats = fivePointsDefeats;
        }
    }
}
