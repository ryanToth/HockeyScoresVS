using System.Collections.Generic;

namespace HockeyScoresVS
{
    public class Team
    {
        public string TeamCode { get; }

        public string Name => Converters.TeamNameConverter(this.TeamCode);

        public string LogoPath => $"Icons/Logos/{this.TeamCode}.gif";

        public List<Goal> Goals { get; }

        public Team(string teamCode)
        {
            this.TeamCode = teamCode;
            this.Goals = new List<Goal>();
        }
    }
}
