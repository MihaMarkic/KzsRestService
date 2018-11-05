    using Righthand.Immutable;

namespace KzsRest.Models
{
    public class LeagueOverview
    {
        public Standings[] Standings { get; }
        public GameFixture[] Fixtures { get; }
        public GameResult[] Results { get; }

        public LeagueOverview(Standings[] standings, GameFixture[] fixtures, GameResult[] results)
        {
            Standings = standings;
            Fixtures = fixtures;
            Results = results;
        }

        public LeagueOverview Clone(Param<Standings[]>? standings = null, Param<GameFixture[]>? fixtures = null, Param<GameResult[]>? results = null)
        {
            return new LeagueOverview(standings.HasValue ? standings.Value.Value : Standings,
				fixtures.HasValue ? fixtures.Value.Value : Fixtures,
				results.HasValue ? results.Value.Value : Results);
        }

        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType()) return false;
            var o = (LeagueOverview)obj;
            return Equals(Standings, o.Standings) && Equals(Fixtures, o.Fixtures) && Equals(Results, o.Results);}

        public override int GetHashCode()
        {
            unchecked
			{
				int hash = 23;
				hash = hash * 37 + (Standings != null ? Standings.GetHashCode() : 0);
				hash = hash * 37 + (Fixtures != null ? Fixtures.GetHashCode() : 0);
				hash = hash * 37 + (Results != null ? Results.GetHashCode() : 0);
				return hash;
			}
        }
    }
}
