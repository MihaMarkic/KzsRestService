namespace KzsRest.Models
{
    public class KzsOriginalLeagues
    {
        public static KzsOriginalLeagues Default { get; }
        public MajorLeagueObsolete[] MajorLeagues { get; }
        public MinorLeagueObsolete[] MinorLeagues { get; }
        public MajorCupLeagueObsolete[] MajorCupLeagues { get; }
        public MinorCupLeagueObsolete[] MinorCupLeagues { get; }
        static KzsOriginalLeagues()
        {
            Default = new KzsOriginalLeagues
                (
                    majorLeagues: new MajorLeagueObsolete[]{
                        new MajorLeagueObsolete("Liga Nova KBM", 1, Gender.Men, "clanek/Tekmovanja/Liga-Nova-KBM/cid/66"),
                        new MajorLeagueObsolete("2. SKL", 2, Gender.Men, "clanek/Tekmovanja/2.-SKL/cid/68"),
                        new MajorLeagueObsolete("3. SKL", 3, Gender.Men, "clanek/Tekmovanja/3.-SKL/cid/69"),
                        new MajorLeagueObsolete("4. SKL", 4, Gender.Men, "clanek/Tekmovanja/4.-SKL/cid/70"),
                        new MajorLeagueObsolete("1. SKL za ženske", 1, Gender.Women, "clanek/Tekmovanja/1.-SKL-za-zenske/cid/67")
                    },
                    minorLeagues: new MinorLeagueObsolete[]
                    {
                        new MinorLeagueObsolete(19, Gender.Men, new MinorLeagueDivisionObsolete[]{
                            new MinorLeagueDivisionObsolete(
                                "clanek/Tekmovanja/Mlajse-kategorije/Fantje/Fantje-U19/cid/99", 19, Gender.Men,
                                "Fantje U19 - 1. A SKL", DivisionType.FirstA),
                            new MinorLeagueDivisionObsolete(
                                "clanek/Tekmovanja/Mlajse-kategorije/Fantje/Fantje-U19/Fantje-U19---1.-B-SKL/cid/112", 19, Gender.Men,
                                "Fantje U19 - 1. B SKL", DivisionType.FirstB),
                            new MinorLeagueDivisionObsolete(
                                "clanek/Tekmovanja/Mlajse-kategorije/Fantje/Fantje-U19/Fantje-U19---2.-SKL/cid/113", 19, Gender.Men,
                                "Fantje U19 - 2. SKL", DivisionType.Second),
                            new MinorLeagueDivisionObsolete(
                                "clanek/Tekmovanja/Mlajse-kategorije/Fantje/Fantje-U19/Fantje-U19---kval.-za-1.-SKL/cid/114", 19, Gender.Men,
                                "Fantje U19 - Kval. za 1. SKL", DivisionType.FirstQualify),
                        }),
                        new MinorLeagueObsolete(17, Gender.Men, new MinorLeagueDivisionObsolete[]{
                            new MinorLeagueDivisionObsolete(
                                "clanek/Tekmovanja/Mlajse-kategorije/Fantje/Fantje-U17/Fantje-U17---1.-SKL/cid/177", 17, Gender.Men,
                                "Fantje U17 - 1. SKL", DivisionType.First),
                            new MinorLeagueDivisionObsolete(
                                "clanek/Tekmovanja/Mlajse-kategorije/Fantje/Fantje-U17/Fantje-U17---2.-SKL/cid/115", 17, Gender.Men,
                                "Fantje U17 - 2. SKL", DivisionType.Second),
                            new MinorLeagueDivisionObsolete(
                                "clanek/Tekmovanja/Mlajse-kategorije/Fantje/Fantje-U17/Fantje-U17---kval.-za-1.-SKL/cid/116", 17, Gender.Men,
                                "Fantje U17 - Kval. za 1. SKL", DivisionType.FirstQualify),
                        }),
                        new MinorLeagueObsolete(15, Gender.Men, new MinorLeagueDivisionObsolete[]{
                            new MinorLeagueDivisionObsolete(
                                "clanek/Tekmovanja/Mlajse-kategorije/Fantje/Fantje-U15/cid/101", 15, Gender.Men,
                                "Fantje U15 - 1. SKL", DivisionType.First),
                            new MinorLeagueDivisionObsolete(
                                "clanek/Tekmovanja/Mlajse-kategorije/Fantje/Fantje-U15/Fantje-U15---2.-SKL/cid/117", 15, Gender.Men,
                                "Fantje U15 - 2. SKL", DivisionType.Second),
                            new MinorLeagueDivisionObsolete(
                                "clanek/Tekmovanja/Mlajse-kategorije/Fantje/Fantje-U15/Fantje-U15---kval.-za-1.-SKL/cid/118", 15, Gender.Men,
                                "Fantje U15 - Kval. za 1. SKL", DivisionType.FirstQualify)
                        }),
                        new MinorLeagueObsolete(13, Gender.Men, new MinorLeagueDivisionObsolete[]{
                            new MinorLeagueDivisionObsolete(
                                "clanek/Tekmovanja/Mlajse-kategorije/Fantje/Fantje-U13/cid/102", 13, Gender.Men,
                                "Fantje U13 - 1. SKL", DivisionType.First),
                            new MinorLeagueDivisionObsolete(
                                "clanek/Tekmovanja/Mlajse-kategorije/Fantje/Fantje-U13/cid/119", 13, Gender.Men,
                                "Fantje U13 - 2. SKL", DivisionType.Second),
                            new MinorLeagueDivisionObsolete(
                                "clanek/Tekmovanja/Mlajse-kategorije/Fantje/Fantje-U13/Fantje-U13---kval.-za-1.-SKL/cid/120", 13, Gender.Men,
                                "Fantje U13 - Kval. za 1. SKL", DivisionType.FirstQualify)
                        }),
                        new MinorLeagueObsolete(11, Gender.Men, new MinorLeagueDivisionObsolete[]{
                            new MinorLeagueDivisionObsolete(
                                "clanek/Tekmovanja/Mlajse-kategorije/Fantje/Fantje-U11/cid/103", 11, Gender.Men,
                                "Fantje U11", DivisionType.First)
                        }),
                        new MinorLeagueObsolete(9, Gender.Men, new MinorLeagueDivisionObsolete[]{
                            new MinorLeagueDivisionObsolete(
                                "clanek/Tekmovanja/Mlajse-kategorije/Fantje-in-dekleta-U9/cid/104", 9, Gender.Men,
                                "Fantje in dekleta U9", DivisionType.First)
                        }),
                        // Women
                        new MinorLeagueObsolete(19, Gender.Women, new MinorLeagueDivisionObsolete[]{
                            new MinorLeagueDivisionObsolete(
                                "clanek/Tekmovanja/Mlajse-kategorije/Dekleta/Dekleta-U19/cid/105", 17, Gender.Men,
                                "Dekleta U19", DivisionType.First)
                        }),
                        new MinorLeagueObsolete(17, Gender.Women, new MinorLeagueDivisionObsolete[]{
                            new MinorLeagueDivisionObsolete(
                                "clanek/Tekmovanja/Mlajse-kategorije/Dekleta/Dekleta-U17/cid/106", 17, Gender.Men,
                                "Dekleta U17", DivisionType.First)
                        }),
                        new MinorLeagueObsolete(15, Gender.Women, new MinorLeagueDivisionObsolete[]{
                            new MinorLeagueDivisionObsolete(
                                "clanek/Tekmovanja/Mlajse-kategorije/Dekleta/Dekleta-U15/cid/107", 15, Gender.Men,
                                "Dekleta U15", DivisionType.First)
                        }),
                        new MinorLeagueObsolete(13, Gender.Women, new MinorLeagueDivisionObsolete[]{
                            new MinorLeagueDivisionObsolete(
                                "clanek/Tekmovanja/Mlajse-kategorije/Dekleta/Dekleta-U13/cid/108", 13, Gender.Men,
                                "Dekleta U13", DivisionType.First)
                        }),
                        new MinorLeagueObsolete(11, Gender.Women, new MinorLeagueDivisionObsolete[]{
                            new MinorLeagueDivisionObsolete(
                                "clanek/Tekmovanja/Mlajse-kategorije/Dekleta/Dekleta-U11/cid/109", 11, Gender.Men,
                                "Dekleta U11", DivisionType.First)
                        }),
                        new MinorLeagueObsolete(9, Gender.Women, new MinorLeagueDivisionObsolete[]{
                            new MinorLeagueDivisionObsolete(
                                "clanek/Tekmovanja/Mlajse-kategorije/Fantje-in-dekleta-U9/cid/104", 9, Gender.Men,
                                "Fantje in dekleta U9", DivisionType.First)
                        })
                    },
                    majorCupLeagues: new MajorCupLeagueObsolete[]
                    {
                        new MajorCupLeagueObsolete("Pokal SPAR", Gender.Men, "clanek/Tekmovanja/Pokal-Spar/cid/72"),
                        new MajorCupLeagueObsolete("Pokal članic", Gender.Women, "clanek/Tekmovanja/Pokal-clanic/cid/73")
                    },
                    minorCupLeagues: new MinorCupLeagueObsolete[]
                    {
                        new MinorCupLeagueObsolete("Mini pokal SPAR", Gender.Men, "clanek/Tekmovanja/Mini-pokal-Spar/cid/74"),
                        new MinorCupLeagueObsolete("Mini pokal deklet", Gender.Women, "clanek/Tekmovanja/Mini-pokal-deklet/cid/75")
                    }
                );
        }
        public KzsOriginalLeagues(MajorLeagueObsolete[] majorLeagues, MinorLeagueObsolete[] minorLeagues, MajorCupLeagueObsolete[] majorCupLeagues, MinorCupLeagueObsolete[] minorCupLeagues)
        {
            MajorLeagues = majorLeagues;
            MinorLeagues = minorLeagues;
            MajorCupLeagues = majorCupLeagues;
            MinorCupLeagues = minorCupLeagues;
        }
    }
}
