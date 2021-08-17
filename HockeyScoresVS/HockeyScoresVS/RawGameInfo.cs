namespace HockeyScoresVS
{
    internal class RawGameInfo
    {
        public RawGameInfo(string id, string est, string a, string h)
        {
            this.id = id;
            this.est = est;
            this.a = a;
            this.h = h;
        }

        public string id;
        public string est;
        public string a;
        public string h;
    }
}
