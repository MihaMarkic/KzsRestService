namespace KzsRest.Models
{
    public class MajorCupLeagueObsolete
    {
        public string Name { get; }
        public Gender Gender { get; }
        public string Url { get; }
        public MajorCupLeagueObsolete(string name, Gender gender, string url)
        {
            Name = name;
            Gender = gender;
            Url = url;
        }
    }
}
