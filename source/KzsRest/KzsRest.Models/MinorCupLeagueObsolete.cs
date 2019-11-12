namespace KzsRest.Models
{
    public class MinorCupLeagueObsolete
    {
        public string Name { get; }
        public Gender Gender { get; }
        public string Url { get; }
        public MinorCupLeagueObsolete(string name, Gender gender, string url)
        {
            Name = name;
            Gender = gender;
            Url = url;
        }
    }
}
