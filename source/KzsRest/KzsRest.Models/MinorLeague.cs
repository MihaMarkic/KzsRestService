namespace KzsRest.Models
{
    public class MinorLeague
    {
        public int ULevel { get; }
        public Gender Gender { get; }
        public MinorLeagueDivision[] Divisions { get; }
        public MinorLeague(int uLevel, Gender gender, MinorLeagueDivision[] divisions)
        {
            ULevel = uLevel;
            Gender = gender;
            Divisions = divisions;
        }
    }

    public  class MinorLeagueDivision
    {
        public string Url { get; }
        public int ULevel { get; }
        public Gender Gender { get; }
        public string Name { get; }
        public DivisionType DivisionType { get; }
        public MinorLeagueDivision(string url, int uLevel, Gender gender, string name, DivisionType divisionType)
        {
            Url = url;
            ULevel = uLevel;
            Gender = gender;
            Name = name;
            DivisionType = divisionType;
        }
    }
}
