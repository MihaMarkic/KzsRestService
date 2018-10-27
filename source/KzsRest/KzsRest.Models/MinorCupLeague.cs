namespace KzsRest.Models
{
    public class MinorCupLeague
    {
        public string Name { get; }
        public Gender Gender { get; }
        public string Url { get; }
        public MinorCupLeague(string name, Gender gender, string url)
        {
            Name = name;
            Gender = gender;
            Url = url;
        }
    }
}
