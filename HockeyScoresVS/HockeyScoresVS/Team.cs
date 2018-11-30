using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HockeyScoresVS
{
    public class Team
    {
        public string TeamCode { get; private set; }

        public string Name
        {
            get
            {
                return Converters.TeamNameConverter(TeamCode);
            }
        }

        public string LogoPath
        {
            get
            {
                return $"Icons/Logos/{TeamCode}.gif";
            }
        }

        public List<Goal> Goals { get; }

        public Team(string teamCode)
        {
            this.TeamCode = teamCode;
            this.Goals = new List<Goal>();
        }
    }
}
