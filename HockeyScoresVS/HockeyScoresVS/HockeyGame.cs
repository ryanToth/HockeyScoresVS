using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HockeyScoresVS
{
    public class HockeyGame
    {
        private string _id;

        public string StartTime { get; }

        public Team HomeTeam { get; }

        public Team AwayTeam { get; }

        public int MinutesLeftInPeriod { get; set; }

        public int SecondsLeftInMinute { get; set; }

        private string _period;
        public string Period
        {
            get
            {
                switch (_period)
                {
                    case "1":
                    case "2":
                    case "3":
                        return $"Period {_period}";
                    default:
                        return _period;
                }
            }
        }

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
            this._period = "1";
        }

        // TODO: Implement
        private bool HasGameStartedYet
        {
            get
            {
                return true;
            }
        }
    }
}
