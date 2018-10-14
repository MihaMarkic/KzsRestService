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
    }
}
