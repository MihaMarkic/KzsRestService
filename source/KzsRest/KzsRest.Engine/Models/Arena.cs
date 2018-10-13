using Righthand.Immutable;

namespace KzsRest.Engine.Models
{
    public class Arena
    {
        public string Name { get; }
        public string Url { get; }

        public Arena(string name, string url)
        {
            Name = name;
            Url = url;
        }

        public Arena Clone(Param<string>? name = null, Param<string>? url = null)
        {
            return new Arena(name.HasValue ? name.Value.Value : Name,
				url.HasValue ? url.Value.Value : Url);
        }

        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType()) return false;
            var o = (Arena)obj;
            return Equals(Name, o.Name) && Equals(Url, o.Url);}

        public override int GetHashCode()
        {
            unchecked
			{
				int hash = 23;
				hash = hash * 37 + (Name != null ? Name.GetHashCode() : 0);
				hash = hash * 37 + (Url != null ? Url.GetHashCode() : 0);
				return hash;
			}
        }
    }
}
