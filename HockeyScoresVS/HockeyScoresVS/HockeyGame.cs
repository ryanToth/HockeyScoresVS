using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HockeyScoresVS
{
    public class HockeyGame : INotifyPropertyChanged
    {
        private string _id;

        public string StartTime { get; }

        public Team HomeTeam { get; }

        public Team AwayTeam { get; }

        private int _minutesLeftInPeriod;
        public int MinutesLeftInPeriod
        {
            get
            {
                return _minutesLeftInPeriod;
            }

            set
            {
                _minutesLeftInPeriod = value;
                OnNotifyPropertyChanged("TimeLeftInPeriod");
            }
        }

        private int _secondsLeftInMinute;
        public int SecondsLeftInMinute
        {
            get
            {
                return _secondsLeftInMinute;
            }

            set
            {
                _secondsLeftInMinute = value;
                OnNotifyPropertyChanged("TimeLeftInPeriod");
            }
        }

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

            set
            {
                _period = value;
                OnNotifyPropertyChanged("Period");
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

        // TODO: Implement
        private bool HasGameStartedYet
        {
            get
            {
                return false;
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

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnNotifyPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        #endregion
    }
}
