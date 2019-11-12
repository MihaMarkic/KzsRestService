using Flurl;
using System;

namespace KzsRest.Models
{
    public abstract class KzsLeague
    {
        public int Id { get; }
        public Gender Gender { get; }
        public string Name { get; }
        public Uri Uri { get; }
        public KzsLeague(int id, Gender gender, string name, string uri)
        {
            Id = id;
            Gender = gender;
            Name = name;
            Uri = new Uri(Url.Combine("https://www.kzs.si/", uri));
        }
    }

    public class MinorLeague: KzsLeague
    {
        public int ULevel { get; }
        public DivisionType DivisionType { get; }
        public MinorLeague(int id, Gender gender, int uLevel, DivisionType divisionType, string name, string url) : base(id, gender, name, url)
        {
            ULevel = uLevel;
            DivisionType = divisionType;
        }
    }

    public class MinorCupLeague : KzsLeague
    {
        public MinorCupLeague(int id, Gender gender, string name, string url) : base(id, gender, name, url)
        {
        }
    }

    public class MajorLeague: KzsLeague
    {
        public int Division { get; }
        public MajorLeague(int id, Gender gender, string name, int division, string url) : base(id, gender, name, url)
        {
            Division = division;
        }
    }
}
