namespace KzsRest.Engine.Models
{
    public class MajorLeague
    {
        public int Cid { get; }
        public string Name { get; }
        public string Url { get; }
        public MajorLeague(int cid, string name, string url)
        {
            Cid = cid;
            Name = name;
            Url = url;
        }
    }
}
