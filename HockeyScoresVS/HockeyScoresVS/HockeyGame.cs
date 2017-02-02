using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace HockeyScoresVS
{
    public class HockeyGame : INotifyPropertyChanged
    {
        private const int DataRefreshInterval = 5000;
        private Timer _refreshDataTimer;
        private string _id;

        public string StartTime { get; }

        public Team HomeTeam { get; }

        public Team AwayTeam { get; }

        private int _secondsLeftInPeriod;
        public int SecondsLeftInPeriod
        {
            get
            {
                return _secondsLeftInPeriod;
            }

            set
            {
                _secondsLeftInPeriod = value;
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
                    case "4":
                        return "OT";
                    case "5":
                        return "Shootout";
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

        private int _homeGoals;
        public int HomeTeamScore
        {
            get
            {
                return _homeGoals;
            }

            set
            {
                if (_homeGoals != value)
                {
                    _homeGoals = value;
                    OnNotifyPropertyChanged("HomeTeamScore");
                }
            }
        }

        private int _awayGoals;
        public int AwayTeamScore
        {
            get
            {
                return _awayGoals;
            }

            set
            {
                if (_awayGoals != value)
                {
                    _awayGoals = value;
                    OnNotifyPropertyChanged("AwayTeamScore");
                }
            }
        }

        public string TimeLeftInPeriod
        {
            get
            {
                string time = $"{SecondsLeftInPeriod/60}:";
                if (SecondsLeftInPeriod%60 < 10) time += $"0{SecondsLeftInPeriod % 60}";
                else time += (SecondsLeftInPeriod % 60).ToString();

                return time;
            }
        }

        public bool HasGameStartedYet
        {
            get
            {
                return DateTime.Now.TimeOfDay > DateTime.Parse(this.StartTime, CultureInfo.CurrentCulture).TimeOfDay;
            }
        }

        public bool IsGameOver
        {
            get
            {
                return AwayTeamScore != HomeTeamScore && _secondsLeftInPeriod == 0 && Int32.Parse(_period) >= 3;
            }
        }

        public HockeyGame(string startTime, Team homeTeam, Team awayTeam, string id)
        {
            this.StartTime = startTime;
            this.HomeTeam = homeTeam;
            this.AwayTeam = awayTeam;
            this._id = id;
            this.SecondsLeftInPeriod = 1200;
            this._period = "1";
            _refreshDataTimer = new Timer(RefreshGameData, new AutoResetEvent(true), 0, DataRefreshInterval);
        }

        private async void RefreshGameData(object state)
        {
            if (HasGameStartedYet && !IsGameOver)
            {
                OnNotifyPropertyChanged("HasGameStartedYet");

                WebRequest request = WebRequest.Create($"http://live.nhl.com/GameData/20162017/{_id}/gc/gcsb.jsonp");
                WebResponse response = await request.GetResponseAsync();

                Stream dataStream = response.GetResponseStream();
                StreamReader reader = new StreamReader(dataStream);

                string jsonFile = await reader.ReadToEndAsync();
                reader.Close();
                response.Close();

                try
                {
                    JObject gameData = JObject.Parse(jsonFile.Substring(10, jsonFile.Length - 11));
                    this.Period = gameData["p"].Value<string>();
                    this.SecondsLeftInPeriod = gameData["sr"].Value<int>();
                    this.HomeTeamScore = gameData["h"]["tot"]["g"].Value<int>();
                    this.AwayTeamScore = gameData["a"]["tot"]["g"].Value<int>();
                }
                catch (Exception) { }
            }
            else if (IsGameOver)
            {
                OnNotifyPropertyChanged("IsGameOver");
                _refreshDataTimer.Dispose();
            }
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
