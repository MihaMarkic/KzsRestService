namespace KzsRest.Models
{
    public class KzsLeagues
    {
        public static KzsLeagues Default { get; }
        public MajorLeague[] MajorLeagues { get; }
        public MinorLeague[] MinorLeagues { get; }
        public MajorCupLeague[] MajorCupLeagues { get; }
        public MinorCupLeague[] MinorCupLeagues { get; }
        static KzsLeagues()
        {
            Default = new KzsLeagues
                (
                    majorLeagues: new MajorLeague[]{
                        new MajorLeague("Liga Nova KBM", 1, Gender.Men, "clanek/Tekmovanja/Liga-Nova-KBM/cid/66"),
                        new MajorLeague("2. SKL", 2, Gender.Men, "clanek/Tekmovanja/2.-SKL/cid/68"),
                        new MajorLeague("3. SKL", 3, Gender.Men, "clanek/Tekmovanja/3.-SKL/cid/69"),
                        new MajorLeague("4. SKL", 4, Gender.Men, "clanek/Tekmovanja/4.-SKL/cid/70"),
                        new MajorLeague("1. SKL za ženske", 1, Gender.Women, "clanek/Tekmovanja/1.-SKL-za-zenske/cid/67")
                    },
                    minorLeagues: new MinorLeague[]
                    {
                        new MinorLeague(19, Gender.Men, new MinorLeagueDivision[]{
                            new MinorLeagueDivision(
                                "clanek/Tekmovanja/Mlajse-kategorije/Fantje/Fantje-U19/cid/99", 19, Gender.Men,
                                "Fantje U19 - 1. A SKL", DivisionType.FirstA),
                            new MinorLeagueDivision(
                                "clanek/Tekmovanja/Mlajse-kategorije/Fantje/Fantje-U19/Fantje-U19---1.-B-SKL/cid/112", 19, Gender.Men,
                                "Fantje U19 - 2. B SKL", DivisionType.FirstB),
                            new MinorLeagueDivision(
                                "clanek/Tekmovanja/Mlajse-kategorije/Fantje/Fantje-U19/Fantje-U19---2.-SKL/cid/113", 19, Gender.Men,
                                "Fantje U19 - 2. SKL", DivisionType.Second),
                            new MinorLeagueDivision(
                                "clanek/Tekmovanja/Mlajse-kategorije/Fantje/Fantje-U19/Fantje-U19---kval.-za-1.-SKL/cid/114", 19, Gender.Men,
                                "Fantje U19 - Kval. za 1. SKL", DivisionType.FirstQualify),
                        }),
                        new MinorLeague(17, Gender.Men, new MinorLeagueDivision[]{
                            new MinorLeagueDivision(
                                "clanek/Tekmovanja/Mlajse-kategorije/Fantje/Fantje-U17/Fantje-U17---1.-SKL/cid/177", 17, Gender.Men,
                                "Fantje U17 - 1. SKL", DivisionType.First),
                            new MinorLeagueDivision(
                                "clanek/Tekmovanja/Mlajse-kategorije/Fantje/Fantje-U17/Fantje-U17---2.-SKL/cid/115", 17, Gender.Men,
                                "Fantje U17 - 2. SKL", DivisionType.Second),
                            new MinorLeagueDivision(
                                "clanek/Tekmovanja/Mlajse-kategorije/Fantje/Fantje-U17/Fantje-U17---kval.-za-1.-SKL/cid/116", 17, Gender.Men,
                                "Fantje U17 - Kval. za 1. SKL", DivisionType.FirstQualify),
                        }),
                        new MinorLeague(15, Gender.Men, new MinorLeagueDivision[]{
                            new MinorLeagueDivision(
                                "clanek/Tekmovanja/Mlajse-kategorije/Fantje/Fantje-U15/cid/101", 15, Gender.Men,
                                "Fantje U15 - 1. SKL", DivisionType.First),
                            new MinorLeagueDivision(
                                "clanek/Tekmovanja/Mlajse-kategorije/Fantje/Fantje-U15/Fantje-U15---2.-SKL/cid/117", 15, Gender.Men,
                                "Fantje U15 - 2. SKL", DivisionType.Second),
                            new MinorLeagueDivision(
                                "clanek/Tekmovanja/Mlajse-kategorije/Fantje/Fantje-U15/Fantje-U15---kval.-za-1.-SKL/cid/118", 15, Gender.Men,
                                "Fantje U15 - Kval. za 1. SKL", DivisionType.FirstQualify)
                        }),
                        new MinorLeague(13, Gender.Men, new MinorLeagueDivision[]{
                            new MinorLeagueDivision(
                                "clanek/Tekmovanja/Mlajse-kategorije/Fantje/Fantje-U13/cid/102", 13, Gender.Men,
                                "Fantje U13 - 1. SKL", DivisionType.First),
                            new MinorLeagueDivision(
                                "clanek/Tekmovanja/Mlajse-kategorije/Fantje/Fantje-U13/cid/119", 13, Gender.Men,
                                "Fantje U13 - 2. SKL", DivisionType.Second),
                            new MinorLeagueDivision(
                                "clanek/Tekmovanja/Mlajse-kategorije/Fantje/Fantje-U13/Fantje-U13---kval.-za-1.-SKL/cid/120", 13, Gender.Men,
                                "Fantje U13 - Kval. za 1. SKL", DivisionType.FirstQualify)
                        }),
                        new MinorLeague(11, Gender.Men, new MinorLeagueDivision[]{
                            new MinorLeagueDivision(
                                "clanek/Tekmovanja/Mlajse-kategorije/Fantje/Fantje-U11/cid/103", 11, Gender.Men,
                                "Fantje U11", DivisionType.First)
                        }),
                        new MinorLeague(9, Gender.Men, new MinorLeagueDivision[]{
                            new MinorLeagueDivision(
                                "clanek/Tekmovanja/Mlajse-kategorije/Fantje-in-dekleta-U9/cid/104", 9, Gender.Men,
                                "Fantje in dekleta U9", DivisionType.First)
                        }),
                        // Women
                        new MinorLeague(19, Gender.Women, new MinorLeagueDivision[]{
                            new MinorLeagueDivision(
                                "clanek/Tekmovanja/Mlajse-kategorije/Dekleta/Dekleta-U19/cid/105", 17, Gender.Men,
                                "Dekleta U19", DivisionType.First)
                        }),
                        new MinorLeague(17, Gender.Women, new MinorLeagueDivision[]{
                            new MinorLeagueDivision(
                                "clanek/Tekmovanja/Mlajse-kategorije/Dekleta/Dekleta-U17/cid/106", 17, Gender.Men,
                                "Dekleta U17", DivisionType.First)
                        }),
                        new MinorLeague(15, Gender.Women, new MinorLeagueDivision[]{
                            new MinorLeagueDivision(
                                "clanek/Tekmovanja/Mlajse-kategorije/Dekleta/Dekleta-U15/cid/107", 15, Gender.Men,
                                "Dekleta U15", DivisionType.First)
                        }),
                        new MinorLeague(13, Gender.Women, new MinorLeagueDivision[]{
                            new MinorLeagueDivision(
                                "clanek/Tekmovanja/Mlajse-kategorije/Dekleta/Dekleta-U13/cid/108", 13, Gender.Men,
                                "Dekleta U13", DivisionType.First)
                        }),
                        new MinorLeague(11, Gender.Women, new MinorLeagueDivision[]{
                            new MinorLeagueDivision(
                                "clanek/Tekmovanja/Mlajse-kategorije/Dekleta/Dekleta-U11/cid/109", 11, Gender.Men,
                                "Dekleta U11", DivisionType.First)
                        }),
                        new MinorLeague(9, Gender.Women, new MinorLeagueDivision[]{
                            new MinorLeagueDivision(
                                "clanek/Tekmovanja/Mlajse-kategorije/Fantje-in-dekleta-U9/cid/104", 9, Gender.Men,
                                "Fantje in dekleta U9", DivisionType.First)
                        })
                    },
                    majorCupLeagues: new MajorCupLeague[]
                    {
                        new MajorCupLeague("Pokal SPAR", Gender.Men, "clanek/Tekmovanja/Pokal-Spar/cid/72"),
                        new MajorCupLeague("Pokal članic", Gender.Women, "clanek/Tekmovanja/Pokal-clanic/cid/73")
                    },
                    minorCupLeagues: new MinorCupLeague[]
                    {
                        new MinorCupLeague("Mini pokal SPAR", Gender.Men, "clanek/Tekmovanja/Mini-pokal-Spar/cid/74"),
                        new MinorCupLeague("Mini pokal deklet", Gender.Women, "clanek/Tekmovanja/Mini-pokal-deklet/cid/75")
                    }
                );
        }
        public KzsLeagues(MajorLeague[] majorLeagues, MinorLeague[] minorLeagues, MajorCupLeague[] majorCupLeagues, MinorCupLeague[] minorCupLeagues)
        {
            MajorLeagues = majorLeagues;
            MinorLeagues = minorLeagues;
            MajorCupLeagues = majorCupLeagues;
            MinorCupLeagues = minorCupLeagues;
        }
    }
}
