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
    public class HockeyGame : INotifyPropertyChanged, IComparable<HockeyGame>, IDisposable
    {
        private const int DataRefreshInterval = 5000;
        private Timer _refreshDataTimer;
        private string _id;
        private string _dateCode;
        private string _seasonCode;

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
                if (SecondsLeftInPeriod != 0)
                {
                    string time = $"{SecondsLeftInPeriod / 60}:";
                    if (SecondsLeftInPeriod % 60 < 10) time += $"0{SecondsLeftInPeriod % 60}";
                    else time += (SecondsLeftInPeriod % 60).ToString();

                    return time;
                }

                if (Int32.Parse(_period) <= 3)
                {
                    return "Intermission";
                }

                return "End";
            }
        }

        private bool _hasGameStarted = false;
        public bool HasGameStartedYet
        {
            get
            {
                return DateTime.Now.Date == DateTime.ParseExact(this._dateCode, "yyyyMMdd", CultureInfo.InvariantCulture).Date &&
                    DateTime.Now.TimeOfDay > DateTime.Parse(this.StartTime, CultureInfo.CurrentCulture).TimeOfDay ||
                    DateTime.Now.Date > DateTime.ParseExact(this._dateCode, "yyyyMMdd", CultureInfo.InvariantCulture).Date;
            }
        }

        public bool IsGameOver
        {
            get
            {
                return AwayTeamScore != HomeTeamScore && _secondsLeftInPeriod == 0 && Int32.Parse(_period) >= 3 ||
                    DateTime.Now.Date > DateTime.ParseExact(this._dateCode, "yyyyMMdd", CultureInfo.InvariantCulture).Date;
            }
        }

        public HockeyGame(string startTime, Team homeTeam, Team awayTeam, string id, string dateCode, string seasonCode)
        {
            this.StartTime = startTime;
            this.HomeTeam = homeTeam;
            this.AwayTeam = awayTeam;
            this._id = id;
            this._dateCode = dateCode;
            this._seasonCode = seasonCode;

            if (IsGameOver)
            {
                this.SecondsLeftInPeriod = 0;
                this._period = "3";
            }
            else
            {
                this.SecondsLeftInPeriod = 1200;
                this._period = "1";
            }

            if (HasGameStartedYet)
            {
                Task.Run(async () => await GetGameData());
            }

            _refreshDataTimer = new Timer(RefreshGameData, new AutoResetEvent(true), DataRefreshInterval, DataRefreshInterval);
        }

        private async void RefreshGameData(object state)
        {
            if (HasGameStartedYet && !IsGameOver)
            {
                if (!_hasGameStarted)
                {
                    _hasGameStarted = true;
                    OnNotifyPropertyChanged("HasGameStartedYet");
                }
                
                await GetGameData();
            }
            if (IsGameOver)
            {
                OnNotifyPropertyChanged("IsGameOver");
                // Stop refreshing the game data if the game is over
                _refreshDataTimer.Change(Timeout.Infinite, Timeout.Infinite);
            }
        }

        private async Task GetGameData()
        {
            WebRequest request = WebRequest.Create($"http://live.nhl.com/GameData/{_seasonCode}/{_id}/gc/gcsb.jsonp");
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

        #region IComparable Members

        public int CompareTo(HockeyGame other)
        {
            if (DateTime.Parse(other.StartTime, CultureInfo.CurrentCulture).TimeOfDay > DateTime.Parse(this.StartTime, CultureInfo.CurrentCulture).TimeOfDay)
            {
                return -1;
            }

            return 1;
        }

        #endregion

        #region IDisposable Members

        public void Dispose()
        {
            _refreshDataTimer.Change(Timeout.Infinite, Timeout.Infinite);
            _refreshDataTimer.Dispose();
        }

        #endregion
    }
}
