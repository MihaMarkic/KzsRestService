using HtmlAgilityPack;
using Righthand.Immutable;

namespace KzsRest.Engine.Models
{
    public struct TeamHtmlData
    {
        public HtmlNode Info { get; }
        public HtmlNode LastResults { get; }
        public HtmlNode Fixtures { get; }
        public HtmlNode Players { get; }

        public TeamHtmlData(HtmlNode info, HtmlNode lastResults, HtmlNode fixtures, HtmlNode players)
        {
            Info = info;
            LastResults = lastResults;
            Fixtures = fixtures;
            Players = players;
        }

        public TeamHtmlData Clone(Param<HtmlNode>? info = null, Param<HtmlNode>? lastResults = null, Param<HtmlNode>? fixtures = null, Param<HtmlNode>? players = null)
        {
            return new TeamHtmlData(info.HasValue ? info.Value.Value : Info,
				lastResults.HasValue ? lastResults.Value.Value : LastResults,
				fixtures.HasValue ? fixtures.Value.Value : Fixtures,
				players.HasValue ? players.Value.Value : Players);
        }

        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType()) return false;
            var o = (TeamHtmlData)obj;
            return Equals(Info, o.Info) && Equals(LastResults, o.LastResults) && Equals(Fixtures, o.Fixtures) && Equals(Players, o.Players);
}

        public override int GetHashCode()
        {
            unchecked
			{
				int hash = base.GetHashCode();
				hash = hash * 37 + (Info != null ? Info.GetHashCode() : 0);
				hash = hash * 37 + (LastResults != null ? LastResults.GetHashCode() : 0);
				hash = hash * 37 + (Fixtures != null ? Fixtures.GetHashCode() : 0);
				hash = hash * 37 + (Players != null ? Players.GetHashCode() : 0);
				return hash;
			}
        }
    }
}
