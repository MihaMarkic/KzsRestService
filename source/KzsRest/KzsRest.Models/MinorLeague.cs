namespace KzsRest.Models
{
    public class MinorLeague
    {
        public  int Cid { get; }
        public int ULevel { get; }
        public Gender Gender { get; }
        public MinorLeagueDivision[] Divisions { get; }
        public MinorLeague(int cid, int uLevel, Gender gender, MinorLeagueDivision[] divisions)
        {
            Cid = cid;
            ULevel = uLevel;
            Gender = gender;
            Divisions = divisions;
        }
    }

    public  class MinorLeagueDivision
    {
        public  int Cid { get; }
        public string Url { get; }
        public int ULevel { get; }
        public Gender Gender { get; }
        public string Name { get; }
        public DivisionType DivisionType { get; }
        public MinorLeagueDivision(int cid, string url, int uLevel, Gender gender, string name, DivisionType divisionType)
        {
            Cid = cid;
            Url = url;
            ULevel = uLevel;
            Gender = gender;
            Name = name;
            DivisionType = divisionType;
        }
    }
}
