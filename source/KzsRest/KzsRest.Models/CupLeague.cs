namespace KzsRest.Models
{
    public class MajorCupLeague
    {
        public string Name { get; }
        public Gender Gender { get; }
        public string Url { get; }
        public MajorCupLeague(string name, Gender gender, string url)
        {
            Name = name;
            Gender = gender;
            Url = url;
        }
    }
}
