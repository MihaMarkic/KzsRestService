namespace KzsRest.Models
{
    public class KzsLeagues
    {
        public MajorLeague[] MajorLeagues { get; }
        public MinorLeague[] MinorLeagues { get; }
        public KzsLeagues(MajorLeague[] majorLeagues, MinorLeague[] minorLeagues)
        {
            MajorLeagues = majorLeagues;
            MinorLeagues = minorLeagues;
        }
    }
}
