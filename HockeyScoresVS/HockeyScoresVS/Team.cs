using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HockeyScoresVS
{
    public class Team
    {
        public string Name { get; private set; }
        public List<Goal> Goals { get; }

        public Team(string name)
        {
            this.Name = name;
            this.Goals = new List<Goal>();
        }
    }
}
