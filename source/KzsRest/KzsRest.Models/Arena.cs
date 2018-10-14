using Righthand.Immutable;

namespace KzsRest.Models
{
    public class Arena
    {
        public int Id { get; }
        public string Name { get; }
        public string Url { get; }

        public Arena(int id, string name, string url)
        {
            Id = id;
            Name = name;
            Url = url;
        }

        public Arena Clone(Param<int>? id = null, Param<string>? name = null, Param<string>? url = null)
        {
            return new Arena(id.HasValue ? id.Value.Value : Id,
				name.HasValue ? name.Value.Value : Name,
				url.HasValue ? url.Value.Value : Url);
        }

        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType()) return false;
            var o = (Arena)obj;
            return Equals(Id, o.Id) && Equals(Name, o.Name) && Equals(Url, o.Url);}

        public override int GetHashCode()
        {
            unchecked
			{
				int hash = 23;
				hash = hash * 37 + Id.GetHashCode();
				hash = hash * 37 + (Name != null ? Name.GetHashCode() : 0);
				hash = hash * 37 + (Url != null ? Url.GetHashCode() : 0);
				return hash;
			}
        }
    }
}
