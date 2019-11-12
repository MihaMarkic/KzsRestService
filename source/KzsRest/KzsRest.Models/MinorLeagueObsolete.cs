namespace KzsRest.Models
{
    public class MinorLeagueObsolete
    {
        public int ULevel { get; }
        public Gender Gender { get; }
        public MinorLeagueDivisionObsolete[] Divisions { get; }
        public MinorLeagueObsolete(int uLevel, Gender gender, MinorLeagueDivisionObsolete[] divisions)
        {
            ULevel = uLevel;
            Gender = gender;
            Divisions = divisions;
        }
    }

    public  class MinorLeagueDivisionObsolete
    {
        public string Url { get; }
        public int ULevel { get; }
        public Gender Gender { get; }
        public string Name { get; }
        public DivisionType DivisionType { get; }
        public MinorLeagueDivisionObsolete(string url, int uLevel, Gender gender, string name, DivisionType divisionType)
        {
            Url = url;
            ULevel = uLevel;
            Gender = gender;
            Name = name;
            DivisionType = divisionType;
        }
    }
}
