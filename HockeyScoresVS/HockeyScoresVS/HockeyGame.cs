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
        // Every 5 seconds
        private const int DataRefreshInterval = 5000;
        // Every 10 minutes
        private const int HasGameStartedCheckInterval = 600000;
        private Timer _refreshDataTimer;
        private string _id;
        private string _dateCode;
        private string _seasonCode;
        public const string DefaultBorderColor = "CornflowerBlue";
        public const string FavouriteBorderColor = "Goldenrod";
        private const string GoalBorderColor = "IndianRed";

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

        private bool _suppressGoalHorn = true;

        private string _borderColor = DefaultBorderColor;
        public string BorderColor
        {
            get
            {
                return _borderColor;
            }
            set
            {
                _borderColor = value;
                OnNotifyPropertyChanged("BorderColor");
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
                if (_period == "5")
                {
                    return $"Shootout\n    {_finalText}";
                }

                return _finalText;
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

            int initialInterval;

            if (HasGameStartedYet)
            {
                initialInterval = DataRefreshInterval;
                Task.Run(async () => await RefreshGameData());
            }
            else
            {
                initialInterval = HasGameStartedCheckInterval;
            }

            if (HomeTeam.Name == ScoresToolWindowCommand.Instance.FavouriteTeam || AwayTeam.Name == ScoresToolWindowCommand.Instance.FavouriteTeam)
            {
                this.BorderColor = FavouriteBorderColor;
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
                _suppressGoalHorn = false;
            }
            catch (Exception) { /* Don't crash VS if any of the above lines throw, just try to poll again next period */ }

            if (IsGameOver)
            {
                OnNotifyPropertyChanged("IsGameOver");
                OnNotifyPropertyChanged("FinalText");
                // Stop refreshing the game data if the game is over
                _refreshDataTimer.Change(Timeout.Infinite, Timeout.Infinite);
            }
        }

        private async Task GoalHorn()
        {
            await Task.Yield();
            string originalBorderColor = BorderColor;
            for (int i = 0; i < 10; i++)
            {
                BorderColor = GoalBorderColor;
                Thread.Sleep(200);

                // If the user changes their favourite team as a goal celebration is happening it could mess up the UI
                if (BorderColor != GoalBorderColor)
                {
                    originalBorderColor = BorderColor;
                }
                else
                {
                    BorderColor = originalBorderColor;
                }
                
                Thread.Sleep(200);

                if (originalBorderColor != BorderColor)
                {
                    originalBorderColor = BorderColor;
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
