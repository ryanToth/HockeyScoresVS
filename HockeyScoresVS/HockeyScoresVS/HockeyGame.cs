using Newtonsoft.Json.Linq;
using System;
using System.ComponentModel;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.PlatformUI;

namespace HockeyScoresVS
{
    public class HockeyGame : INotifyPropertyChanged, IComparable<HockeyGame>, IDisposable
    {
        // Every 5 seconds
        private const int DataRefreshInterval = 5000;
        // Every 10 minutes
        private const int HasGameStartedCheckInterval = 600000;
        private Timer _refreshDataTimer;
        private string _id;
        private string _dateCode;
        private string _seasonCode;
        
        public const string GoalBackgroundColor = "IndianRed";

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
                OnNotifyPropertyChanged("TimeDisplay");
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
                        return "1st";
                    case "2":
                        return "2nd";
                    case "3":
                        return "3rd";
                    case "4":
                        return "OT";
                }

                if (_isPlayoffs && Int32.TryParse(_period, out int periodNum))
                {
                    return (periodNum - 3).ToString() + "OT";
                }
                else
                {
                    switch (_period)
                    {
                        case "5":
                            return "Shootout";
                        default:
                            return _period;
                    }
                }
            }

            set
            {
                _period = value;
                OnNotifyPropertyChanged("Period");
            }
        }

        private bool _isPlayoffs
        {
            get
            {
                DateTime gameDay = DateTime.ParseExact(this._dateCode, "yyyyMMdd", CultureInfo.InvariantCulture).Date;

                // Assume playoffs happen after 4/11 and before September
                return (gameDay.Month > 4 && gameDay.Month < 8) || (gameDay.Month == 4 && gameDay.Day > 11);
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
                    if (!_suppressGoalHorn && value > _homeGoals)
                    {
                        Task.Run(async () =>
                        {
                            await GoalHorn();
                        });
                    }

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
                    if (!_suppressGoalHorn && value > _awayGoals)
                    {
                        Task.Run(async () =>
                        {
                            await GoalHorn();
                        });
                    }

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

        private bool _suppressGoalHorn = true;

        private string _backgroundColor;
        public string BackgroundColor
        {
            get
            {
                return _backgroundColor;
            }
            set
            {
                _backgroundColor = value;
                OnNotifyPropertyChanged("BackgroundColor");
            }
        }

        private bool _isSelected = false;
        public bool IsSelected
        {
            get
            {
                return _isSelected;
            }
            set
            {
                _isSelected = value;
                OnNotifyPropertyChanged("IsSelected");

                if (_isSelected)
                { 
                    this.GameGoals.GetUpdateScoringSummary().ConfigureAwait(continueOnCapturedContext: false);
                }
            }
        }

        private string _finalText = "Final";
        public string FinalText
        {
            get
            {
                if (Int32.TryParse(_period, out int periodNum) && (_isPlayoffs || periodNum == 4))
                {
                    string OTNumber = "";

                    if (periodNum > 3)
                    {
                        OTNumber = periodNum > 4 ? $" {(periodNum - 3).ToString()}" : "  ";
                        return $"{OTNumber}OT\n{_finalText}";
                    }

                    return _finalText;
                }
                else if (_period == "5")
                {
                    return $"Shootout\n    {_finalText}";
                }

                return _finalText;
            }
        }

        public bool _isGameLive;
        public bool IsGameLive
        {
            get
            {
                return _isGameLive;
            }
            set
            {
                _isGameLive = value;
                OnNotifyPropertyChanged("IsGameLive");
            }
        }

        public bool _isIntermission;
        public bool IsIntermission
        {
            get
            {
                return _isIntermission;
            }
            set
            {
                _isIntermission = value;
                OnNotifyPropertyChanged("IsIntermission");
            }
        }

        private bool ShowStartTime
        {
            get
            {
                return !HasGameStartedYet || ((HasGameStartedYet && (_period == "1" && TimeLeftInPeriod == "20:00") || _period == null) && !IsGameOver);
            }
        }

        public string TimeDisplay
        {
            get
            {
                if (ShowStartTime)
                {
                    return StartTime;
                }
                else if (HasGameStartedYet && !IsGameOver)
                {
                    if (!IsGameLive)
                    {
                        IsGameLive = true;
                        IsIntermission = false;
                    }

                    if (_period == "5")
                    {
                        return Period;
                    }

                    if (SecondsLeftInPeriod == 0)
                    {
                        IsGameLive = false;
                        IsIntermission = true;
                        return $"End {Period}";
                    }

                    IsIntermission = false;

                    return $"{Period} {TimeLeftInPeriod}";
                }
                // Game is Over
                else
                {
                    if (IsGameLive)
                    {
                        IsGameLive = false;
                        IsIntermission = false;
                    }

                    return FinalText;
                }
            }
        }

        public GameGoals GameGoals { get; }

        public HockeyGame(string startTime, Team homeTeam, Team awayTeam, string id, string dateCode, string seasonCode)
        {
            this.GameGoals = new GameGoals(seasonCode, id);
            this.StartTime = startTime;
            this.HomeTeam = homeTeam;
            this.AwayTeam = awayTeam;
            this._id = id;
            this._dateCode = dateCode;
            this._seasonCode = seasonCode;
            this._backgroundColor = DefaultColors.DefaultBackgroundColor;

            int initialInterval;

            if (HasGameStartedYet)
            {
                initialInterval = DataRefreshInterval;
                Task.Run(async () => 
                {
                    await RefreshGameData();
                });
            }
            else
            {
                initialInterval = HasGameStartedCheckInterval;
            }

            _refreshDataTimer = new Timer(RefreshGameData, new AutoResetEvent(true), initialInterval, initialInterval);
        }

        private async void RefreshGameData(object state)
        {
            if (HasGameStartedYet && !IsGameOver)
            {
                if (!_hasGameStarted)
                {
                    _hasGameStarted = true;
                    OnNotifyPropertyChanged("HasGameStartedYet");
                    OnNotifyPropertyChanged("TimeDisplay");
                    _refreshDataTimer.Change(DataRefreshInterval, DataRefreshInterval);
                }
                
                await RefreshGameData();
            }
        }

        private async Task RefreshGameData()
        {
            try
            {
                JObject gameData = await NetworkCalls.ApiCallAsync($"http://live.nhl.com/GameData/{_seasonCode}/{_id}/gc/gcsb.jsonp");

                this.Period = gameData["p"].Value<string>();
                this.SecondsLeftInPeriod = gameData["sr"].Value<int>();
                this.HomeTeamScore = gameData["h"]["tot"]["g"].Value<int>();
                this.AwayTeamScore = gameData["a"]["tot"]["g"].Value<int>();
                
                // After the first time the scores are set the goal horn is enabled
                if (_suppressGoalHorn)
                {
                    _suppressGoalHorn = false;
                }
            }
            catch (Exception) { /* Don't crash VS if any of the above lines throw, just try to poll again next period */ }

            if (IsGameOver)
            {
                OnNotifyPropertyChanged("IsGameOver");
                OnNotifyPropertyChanged("TimeDisplay");
                OnNotifyPropertyChanged("FinalText");
                this.IsGameLive = false;
                // Stop refreshing the game data if the game is over
                _refreshDataTimer.Change(Timeout.Infinite, Timeout.Infinite);
            }
        }

        private async Task GoalHorn()
        {
            await Task.Yield();
            string originalBackgroundColor = BackgroundColor;
            for (int i = 0; i < 10; i++)
            {
                BackgroundColor = GoalBackgroundColor;
                Thread.Sleep(200);

                // If the user changes their favourite team as a goal celebration is happening it could mess up the UI
                if (BackgroundColor != GoalBackgroundColor)
                {
                    originalBackgroundColor = BackgroundColor;
                }
                else
                {
                    BackgroundColor = originalBackgroundColor;
                }
                
                Thread.Sleep(200);

                if (originalBackgroundColor != BackgroundColor)
                {
                    originalBackgroundColor = BackgroundColor;
                }
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
