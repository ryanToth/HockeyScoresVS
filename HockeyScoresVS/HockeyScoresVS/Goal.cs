using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HockeyScoresVS
{
    public class Goal
    {
        public Player ScoredBy { get; set; }
        public Player PrimaryAssist { get; set; }
        public Player SecondaryAssist { get; set; }

        public Goal()
        {
        }

        public Goal(Player scoredBy, Player primaryAssist, Player secondaryAssist)
        {
            this.ScoredBy = scoredBy;
            this.PrimaryAssist = primaryAssist;
            this.SecondaryAssist = secondaryAssist;
        }
    }
}
