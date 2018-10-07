namespace KzsRest.Engine.Models
{
    public class Standings
    {
        public string Name { get; }
        public  StandingsEntry[] Entries { get; }
        public Standings(string name, StandingsEntry[] entries)
        {
            Name = name;
            Entries = entries;
        }
    }
}
