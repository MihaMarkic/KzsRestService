namespace KzsRest.Models
{
    public class KzsLeagues
    {
        public MajorLeague[] MajorLeagues { get; }
        public MinorLeague[] MinorLeagues { get; }
        public MajorCupLeague[] MajorCupLeagues { get; }
        public MinorCupLeague[] MinorCupLeagues { get; }
        public KzsLeagues(MajorLeague[] majorLeagues, MinorLeague[] minorLeagues, MajorCupLeague[] majorCupLeagues, MinorCupLeague[] minorCupLeagues)
        {
            MajorLeagues = majorLeagues;
            MinorLeagues = minorLeagues;
            MajorCupLeagues = majorCupLeagues;
            MinorCupLeagues = minorCupLeagues;
        }
    }
}
