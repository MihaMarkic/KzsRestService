using Righthand.Immutable;

namespace KzsRest.Engine.Models
{
    public class Player
    {
        public int Id { get; }
        public int? Number { get; }
        public string FullName { get; }
        public string NationalityCode { get; }
        public string Nationality { get; }
        public int? BirthYear { get; }
        public string Position { get; }
        public int? Height { get; }

        public Player(int id, int? number, string fullName, string nationalityCode, string nationality, int? birthYear, string position, int? height)
        {
            Id = id;
            Number = number;
            FullName = fullName;
            NationalityCode = nationalityCode;
            Nationality = nationality;
            BirthYear = birthYear;
            Position = position;
            Height = height;
        }

        public Player Clone(Param<int>? id = null, Param<int?>? number = null, Param<string>? fullName = null, Param<string>? nationalityCode = null, Param<string>? nationality = null, Param<int?>? birthYear = null, Param<string>? position = null, Param<int?>? height = null)
        {
            return new Player(id.HasValue ? id.Value.Value : Id,
				number.HasValue ? number.Value.Value : Number,
				fullName.HasValue ? fullName.Value.Value : FullName,
				nationalityCode.HasValue ? nationalityCode.Value.Value : NationalityCode,
				nationality.HasValue ? nationality.Value.Value : Nationality,
				birthYear.HasValue ? birthYear.Value.Value : BirthYear,
				position.HasValue ? position.Value.Value : Position,
				height.HasValue ? height.Value.Value : Height);
        }

        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType()) return false;
            var o = (Player)obj;
            return Equals(Id, o.Id) && Equals(Number, o.Number) && Equals(FullName, o.FullName) && Equals(NationalityCode, o.NationalityCode) && Equals(Nationality, o.Nationality) && Equals(BirthYear, o.BirthYear) && Equals(Position, o.Position) && Equals(Height, o.Height);}

        public override int GetHashCode()
        {
            unchecked
			{
				int hash = 23;
				hash = hash * 37 + Id.GetHashCode();
				hash = hash * 37 + (Number != null ? Number.GetHashCode() : 0);
				hash = hash * 37 + (FullName != null ? FullName.GetHashCode() : 0);
				hash = hash * 37 + (NationalityCode != null ? NationalityCode.GetHashCode() : 0);
				hash = hash * 37 + (Nationality != null ? Nationality.GetHashCode() : 0);
				hash = hash * 37 + (BirthYear != null ? BirthYear.GetHashCode() : 0);
				hash = hash * 37 + (Position != null ? Position.GetHashCode() : 0);
				hash = hash * 37 + (Height != null ? Height.GetHashCode() : 0);
				return hash;
			}
        }
    }
}
