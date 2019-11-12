namespace KzsRest.Models
{
    public class KzsLeagues
    {
        public static KzsLeagues Default { get; }
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
        static KzsLeagues()
        {
            Default = new KzsLeagues(
                majorLeagues: new MajorLeague[]
                {
                    new MajorLeague(9333, Gender.Men, "1. SKL za moške", 1, "clanek/Tekmovanja/1.-SKL-za-moske/cid/66"),
                    new MajorLeague(9343, Gender.Women, "1. SKL za ženske", 1, "clanek/Tekmovanja/1.-SKL-za-zenske/cid/67"),
                    new MajorLeague(9353, Gender.Men, "2. SKL", 2, "clanek/Tekmovanja/2.-SKL/cid/68"),
                    new MajorLeague(10573, Gender.Men, "3. SKL", 3, "clanek/Tekmovanja/3.-SKL/cid/69"),
                    new MajorLeague(10583, Gender.Men, "4. SKL", 4, "clanek/Tekmovanja/4.-SKL/cid/70"),
                },
                minorLeagues: new MinorLeague[]
                {
                    // Men
                    new MinorLeague(10293, Gender.Men, 19, DivisionType.FirstA, "Fantje U19 - 1.A SKL", "clanek/Tekmovanja/Mlajse-kategorije/Fantje/Fantje-U19/cid/99"),
                    new MinorLeague(30295, Gender.Men, 19, DivisionType.FirstB, "Fantje U19 - 1.B SKL", "clanek/Tekmovanja/Mlajse-kategorije/Fantje/Fantje-U19/Fantje-U19---1.-B-SKL/cid/112"),
                    new MinorLeague(10333, Gender.Men, 19, DivisionType.Second, "Fantje U19 - 2. SKL", "clanek/Tekmovanja/Mlajse-kategorije/Fantje/Fantje-U19/Fantje-U19---2.-SKL/cid/113"),
                    new MinorLeague(28751, Gender.Men, 19, DivisionType.FirstQualify, "Fantje U19 - Kval. za 1. SKL", "clanek/Tekmovanja/Mlajse-kategorije/Fantje/Fantje-U19/Fantje-U19---kval.-za-1.-SKL/cid/114"),

                    new MinorLeague(10343, Gender.Men, 17, DivisionType.First, "Fantje U17 - 1. SKL",  "clanek/Tekmovanja/Mlajse-kategorije/Fantje/Fantje-U17/Fantje-U17---1.-SKL/cid/177"),
                    new MinorLeague(10353, Gender.Men, 17, DivisionType.Second, "Fantje U17 - 2. SKL","clanek/Tekmovanja/Mlajse-kategorije/Fantje/Fantje-U17/Fantje-U17---2.-SKL/cid/115"),
                    new MinorLeague(28761, Gender.Men, 17, DivisionType.FirstQualify, "Fantje U17 - Kval. za 1. SK","clanek/Tekmovanja/Mlajse-kategorije/Fantje/Fantje-U17/Fantje-U17---kval.-za-1.-SKL/cid/116"),

                    new MinorLeague(11013, Gender.Men, 15, DivisionType.First, "Fantje U15 - 1. SKL", "clanek/Tekmovanja/Mlajse-kategorije/Fantje/Fantje-U15/cid/101"),
                    new MinorLeague(11533, Gender.Men, 15, DivisionType.Second, "Fantje U15 - 2. SKL", "clanek/Tekmovanja/Mlajse-kategorije/Fantje/Fantje-U15/Fantje-U15---2.-SKL/cid/117"),
                    new MinorLeague(29969, Gender.Men, 15, DivisionType.FirstQualify, "Fantje U15 - Kval. za 1. SKL", "clanek/Tekmovanja/Mlajse-kategorije/Fantje/Fantje-U15/Fantje-U15---kval.-za-1.-SKL/cid/118"),

                    new MinorLeague(10933, Gender.Men, 13, DivisionType.First, "Fantje U13 - 1. SKL","clanek/Tekmovanja/Mlajse-kategorije/Fantje/Fantje-U13/cid/102"),
                    new MinorLeague(30297, Gender.Men, 13, DivisionType.Second, "Fantje U13 - 2. SKL","clanek/Tekmovanja/Mlajse-kategorije/Fantje/Fantje-U13/cid/119"),
                    new MinorLeague(29971, Gender.Men, 13, DivisionType.FirstQualify, "Fantje U13 - Kval. za 1. SKL","clanek/Tekmovanja/Mlajse-kategorije/Fantje/Fantje-U13/Fantje-U13---kval.-za-1.-SKL/cid/120"),

                    new MinorLeague(10823, Gender.Men, 11, DivisionType.First, "Fantje U11","clanek/Tekmovanja/Mlajse-kategorije/Fantje/Fantje-U11/cid/103"),
                    new MinorLeague(30301, Gender.Men | Gender.Women, 9, DivisionType.First, "Fantje in dekleta U9","clanek/Tekmovanja/Mlajse-kategorije/Fantje-in-dekleta-U9/cid/104"),
                    // Women
                    new MinorLeague(10313, Gender.Women, 19, DivisionType.First, "Dekleta U19", "clanek/Tekmovanja/Mlajse-kategorije/Dekleta/Dekleta-U19/cid/105"),
                    new MinorLeague(10363, Gender.Women, 17, DivisionType.First, "Dekleta U17", "clanek/Tekmovanja/Mlajse-kategorije/Dekleta/Dekleta-U17/cid/106"),
                    new MinorLeague(11183, Gender.Women, 15, DivisionType.First, "Dekleta U15", "clanek/Tekmovanja/Mlajse-kategorije/Dekleta/Dekleta-U15/cid/107"),
                    new MinorLeague(10893, Gender.Women, 13, DivisionType.First, "Dekleta U13", "clanek/Tekmovanja/Mlajse-kategorije/Dekleta/Dekleta-U13/cid/108"),
                    new MinorLeague(10863, Gender.Women, 11, DivisionType.First, "Dekleta U11", "clanek/Tekmovanja/Mlajse-kategorije/Dekleta/Dekleta-U11/cid/109#mbt:33-303$t&0=1"),
                },
                majorCupLeagues: new MajorCupLeague[]
                {
                    new MajorCupLeague(10593, Gender.Men, "Pokal Spar", "clanek/Tekmovanja/Pokal-Spar/cid/72"),
                    new MajorCupLeague(10613, Gender.Women, "Pokal članic", "clanek/Tekmovanja/Pokal-clanic/cid/73"),

                },
                minorCupLeagues: new MinorCupLeague[]
                {
                    new MinorCupLeague(10813, Gender.Men, "Mini pokal Spar", "clanek/Tekmovanja/Mini-pokal-Spar/cid/74"),
                    new MinorCupLeague(29801, Gender.Women, "Mini pokal deklet", "clanek/Tekmovanja/Mini-pokal-deklet/cid/75"),
                }
            );
        }
    }
}
