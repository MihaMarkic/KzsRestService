using System;
using System.Diagnostics;
using Righthand.Immutable;

namespace KzsRest.Models
{
    [DebuggerDisplay("{Name,nq}")]
    public class TeamFixture
    {
        public int TeamId { get; }
        public string Name { get; }
        public int? LeagueId { get; }
        public int SeasonId { get; }
        public int? Score { get; }

        public TeamFixture(int teamId, string name, int? leagueId, int seasonId, int? score)
        {
            TeamId = teamId;
            Name = name;
            LeagueId = leagueId;
            SeasonId = seasonId;
            Score = score;
        }

        public TeamFixture Clone(Param<int>? teamId = null, Param<string>? name = null, Param<int?>? leagueId = null, Param<int>? seasonId = null, Param<int?>? score = null)
        {
            return new TeamFixture(teamId.HasValue ? teamId.Value.Value : TeamId,
				name.HasValue ? name.Value.Value : Name,
				leagueId.HasValue ? leagueId.Value.Value : LeagueId,
				seasonId.HasValue ? seasonId.Value.Value : SeasonId,
				score.HasValue ? score.Value.Value : Score);
        }

        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType()) return false;
            var o = (TeamFixture)obj;
            return Equals(TeamId, o.TeamId) && Equals(Name, o.Name) && Equals(LeagueId, o.LeagueId) && Equals(SeasonId, o.SeasonId) && Equals(Score, o.Score);}

        public override int GetHashCode()
        {
            unchecked
			{
				int hash = 23;
				hash = hash * 37 + TeamId.GetHashCode();
				hash = hash * 37 + (Name != null ? Name.GetHashCode() : 0);
				hash = hash * 37 + (LeagueId != null ? LeagueId.GetHashCode() : 0);
				hash = hash * 37 + SeasonId.GetHashCode();
				hash = hash * 37 + (Score != null ? Score.GetHashCode() : 0);
				return hash;
			}
        }
    }
}
