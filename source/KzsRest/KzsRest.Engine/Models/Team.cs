﻿using Righthand.Immutable;
using System.Diagnostics;

namespace KzsRest.Engine.Models
{
    [DebuggerDisplay("{Name,nq}")]
    public class Team
    {
        public string Name { get; }
        public string ShortName { get; }
        public string City { get; }
        public Arena Arena { get; }
        public string Coach { get; }
        public Player[] Players { get; }

        public Team(string name, string shortName, string city, Arena arena, string coach, Player[] players)
        {
            Name = name;
            ShortName = shortName;
            City = city;
            Arena = arena;
            Coach = coach;
            Players = players;
        }

        public Team Clone(Param<string>? name = null, Param<string>? shortName = null, Param<string>? city = null, Param<Arena>? arena = null, Param<string>? coach = null, Param<Player[]>? players = null)
        {
            return new Team(name.HasValue ? name.Value.Value : Name,
				shortName.HasValue ? shortName.Value.Value : ShortName,
				city.HasValue ? city.Value.Value : City,
				arena.HasValue ? arena.Value.Value : Arena,
				coach.HasValue ? coach.Value.Value : Coach,
				players.HasValue ? players.Value.Value : Players);
        }

        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType()) return false;
            var o = (Team)obj;
            return Equals(Name, o.Name) && Equals(ShortName, o.ShortName) && Equals(City, o.City) && Equals(Arena, o.Arena) && Equals(Coach, o.Coach) && Equals(Players, o.Players);}

        public override int GetHashCode()
        {
            unchecked
			{
				int hash = 23;
				hash = hash * 37 + (Name != null ? Name.GetHashCode() : 0);
				hash = hash * 37 + (ShortName != null ? ShortName.GetHashCode() : 0);
				hash = hash * 37 + (City != null ? City.GetHashCode() : 0);
				hash = hash * 37 + (Arena != null ? Arena.GetHashCode() : 0);
				hash = hash * 37 + (Coach != null ? Coach.GetHashCode() : 0);
				hash = hash * 37 + (Players != null ? Players.GetHashCode() : 0);
				return hash;
			}
        }
    }
}