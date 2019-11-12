namespace KzsRest.Models
{
    public class MajorLeagueObsolete
    {
        public string Name { get; }
        public int Division { get; }
        public Gender Gender { get; }
        public string Url { get; }
        public MajorLeagueObsolete(string name, int division, Gender gender, string url)
        {
            Name = name;
            Division = division;
            Gender = gender;
            Url = url;
        }
    }
}
