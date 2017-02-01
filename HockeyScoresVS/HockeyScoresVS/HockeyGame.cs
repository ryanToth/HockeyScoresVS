using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HockeyScoresVS
{
    public class HockeyGame
    {
        public string StartTime { get; set; }
        public Team HomeTeam { get; set; }
        public Team AwayTeam { get; set; }
        public int MinutesLeftInPeriod { get; set; }
        public int SecondsLeftInMinute { get; set; }
        public string Period { get; set; }
        private string _id;

        public int HomeTeamScore
        {
            get
            {
                return HomeTeam.Goals.Count();
            }
        }

        public int AwayTeamScore
        {
            get
            {
                return AwayTeam.Goals.Count();
            }
        }

        public string TimeLeftInPeriod
        {
            get
            {
                string time = $"{MinutesLeftInPeriod}:";
                if (SecondsLeftInMinute < 10) time += $"0{SecondsLeftInMinute}";
                else time += SecondsLeftInMinute.ToString();

                return time;
            }
        }


        public HockeyGame(string startTime, Team homeTeam, Team awayTeam, string id)
        {
            this.StartTime = startTime;
            this.HomeTeam = homeTeam;
            this.AwayTeam = awayTeam;
            this._id = id;
            this.MinutesLeftInPeriod = 20;
            this.SecondsLeftInMinute = 0;
            this.Period = "1";
        }
    }
}
