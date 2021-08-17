using System;
using System.ComponentModel;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.Threading;
using Newtonsoft.Json.Linq;

namespace HockeyScoresVS
{
    public class HockeyGame : INotifyPropertyChanged, IComparable<HockeyGame>, IDisposable
    {
        // Every 5 seconds
        private const int DataRefreshInterval = 5000;

        // Every 10 minutes
        private const int HasGameStartedCheckInterval = 600000;

        private Timer refreshDataTimer;
        private string id;
        private string dateCode;
        private string seasonCode;
        
        public const string GoalBackgroundColor = "IndianRed";

        public string StartTime { get; }

        public Team HomeTeam { get; }

        public Team AwayTeam { get; }

        private int secondsLeftInPeriod;
        public int SecondsLeftInPeriod
        {
            get
            {
                return this.secondsLeftInPeriod;
            }

            set
            {
                this.secondsLeftInPeriod = value;
                this.OnNotifyPropertyChanged("TimeDisplay");
            }
        }

        private string period;
        public string Period
        {
            get
            {
                switch (this.period)
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

                if (this.IsPlayoffs && int.TryParse(this.period, out int periodNum))
                {
                    return (periodNum - 3).ToString() + "OT";
                }
                else
                {
                    switch (this.period)
                    {
                        case "5":
                            return "Shootout";
                        default:
                            return period;
                    }
                }
            }

            set
            {
                this.period = value;
                this.OnNotifyPropertyChanged("Period");
            }
        }

        private bool IsPlayoffs
        {
            get
            {
                DateTime gameDay = DateTime.ParseExact(this.dateCode, "yyyyMMdd", CultureInfo.InvariantCulture).Date;

                // Assume playoffs happen after 4/11 and before September
                return (gameDay.Month > 4 && gameDay.Month < 8) || (gameDay.Month == 4 && gameDay.Day > 11);
            }
        }

        private int homeGoals;
        public int HomeTeamScore
        {
            get
            {
                return this.homeGoals;
            }

            set
            {
                if (this.homeGoals != value)
                {
                    if (!this.suppressGoalHorn && value > this.homeGoals)
                    {
                        Task.Run(async () =>
                        {
                            await this.GoalHornAsync();
                        }).Forget();
                    }

                    this.homeGoals = value;
                    this.OnNotifyPropertyChanged("HomeTeamScore");
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
                    if (!suppressGoalHorn && value > _awayGoals)
                    {
                        Task.Run(async () =>
                        {
                            await GoalHornAsync();
                        }).Forget();
                    }

                    _awayGoals = value;
                    this.OnNotifyPropertyChanged("AwayTeamScore");
                }
            }
        }

        public string TimeLeftInPeriod
        {
            get
            {
                if (this.SecondsLeftInPeriod != 0)
                {
                    string time = $"{this.SecondsLeftInPeriod / 60}:";

                    if (this.SecondsLeftInPeriod % 60 < 10)
                    {
                        time += $"0{this.SecondsLeftInPeriod % 60}";
                    }
                    else
                    {
                        time += (this.SecondsLeftInPeriod % 60).ToString();
                    }

                    return time;
                }

                return "End";
            }
        }

        private bool hasGameStarted = false;
        public bool HasGameStartedYet
        {
            get
            {
                return DateTime.Now.Date == DateTime.ParseExact(this.dateCode, "yyyyMMdd", CultureInfo.InvariantCulture).Date &&
                    DateTime.Now.TimeOfDay > DateTime.Parse(this.StartTime, CultureInfo.CurrentCulture).TimeOfDay ||
                    DateTime.Now.Date > DateTime.ParseExact(this.dateCode, "yyyyMMdd", CultureInfo.InvariantCulture).Date;
            }
        }

        public bool IsGameOver
        {
            get
            {
                return this.AwayTeamScore != this.HomeTeamScore && this.secondsLeftInPeriod == 0 && int.Parse(this.period) >= 3 ||
                    DateTime.Now.Date > DateTime.ParseExact(this.dateCode, "yyyyMMdd", CultureInfo.InvariantCulture).Date;
            }
        }

        private bool suppressGoalHorn = true;

        private string backgroundColor;
        public string BackgroundColor
        {
            get
            {
                return this.backgroundColor;
            }
            set
            {
                this.backgroundColor = value;
                this.OnNotifyPropertyChanged("BackgroundColor");
            }
        }

        private bool isSelected = false;
        public bool IsSelected
        {
            get
            {
                return this.isSelected;
            }
            set
            {
                this.isSelected = value;
                this.OnNotifyPropertyChanged("IsSelected");

                if (this.isSelected)
                {
                    this.GameGoals.GetUpdateScoringSummaryAsync().Forget();
                }
            }
        }

        private const string finalText = "Final";
        public string FinalText
        {
            get
            {
                if (int.TryParse(this.period, out int periodNum) && (this.IsPlayoffs || periodNum == 4))
                {
                    string OTNumber = "";

                    if (periodNum > 3)
                    {
                        OTNumber = periodNum > 4 ? $" {(periodNum - 3).ToString()}" : "  ";
                        return $"{OTNumber}OT\n{finalText}";
                    }

                    return finalText;
                }
                else if (period == "5")
                {
                    return $"Shootout\n    {finalText}";
                }

                return finalText;
            }
        }

        public bool isGameLive;
        public bool IsGameLive
        {
            get
            {
                return this.isGameLive;
            }
            set
            {
                this.isGameLive = value;
                this.OnNotifyPropertyChanged("IsGameLive");
            }
        }

        public bool isIntermission;
        public bool IsIntermission
        {
            get
            {
                return this.isIntermission;
            }
            set
            {
                this.isIntermission = value;
                this.OnNotifyPropertyChanged("IsIntermission");
            }
        }

        private bool ShowStartTime
        {
            get
            {
                return !this.HasGameStartedYet || ((this.HasGameStartedYet && (this.period == "1" && this.TimeLeftInPeriod == "20:00") || this.period == null) && !this.IsGameOver);
            }
        }

        public string TimeDisplay
        {
            get
            {
                if (this.ShowStartTime)
                {
                    return this.StartTime;
                }
                else if (this.HasGameStartedYet && !this.IsGameOver)
                {
                    if (!this.IsGameLive)
                    {
                        this.IsGameLive = true;
                        this.IsIntermission = false;
                    }

                    if (this.period == "5")
                    {
                        return this.Period;
                    }

                    if (this.SecondsLeftInPeriod == 0)
                    {
                        this.IsGameLive = false;
                        this.IsIntermission = true;
                        return $"End {this.Period}";
                    }

                    this.IsIntermission = false;

                    return $"{this.Period} {this.TimeLeftInPeriod}";
                }
                // Game is over
                else
                {
                    if (this.IsGameLive)
                    {
                        this.IsGameLive = false;
                        this.IsIntermission = false;
                    }

                    return this.FinalText;
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
            this.id = id;
            this.dateCode = dateCode;
            this.seasonCode = seasonCode;
            this.backgroundColor = DefaultColors.DefaultBackgroundColor;

            int initialInterval;

            if (this.HasGameStartedYet)
            {
                initialInterval = DataRefreshInterval;
                Task.Run(async () =>
                {
                    await RefreshGameDataAsync();
                }).Forget();
            }
            else
            {
                initialInterval = HasGameStartedCheckInterval;
            }

            this.refreshDataTimer = new Timer((state) => this.RefreshGameDataAsync(state).Forget(), new AutoResetEvent(true), initialInterval, initialInterval);
        }

        private async Task RefreshGameDataAsync(object state)
        {
            if (this.HasGameStartedYet && !this.IsGameOver)
            {
                if (!this.hasGameStarted)
                {
                    this.hasGameStarted = true;
                    this.OnNotifyPropertyChanged("HasGameStartedYet");
                    this.OnNotifyPropertyChanged("TimeDisplay");
                    this.refreshDataTimer.Change(DataRefreshInterval, DataRefreshInterval);
                }
                
                await this.RefreshGameDataAsync();
            }
        }

        private async Task RefreshGameDataAsync()
        {
            try
            {
                JObject gameData = await NetworkCalls.ApiCallAsync($"http://live.nhl.com/GameData/{seasonCode}/{id}/gc/gcsb.jsonp");

                this.Period = gameData["p"].Value<string>();
                this.SecondsLeftInPeriod = gameData["sr"].Value<int>();
                this.HomeTeamScore = gameData["h"]["tot"]["g"].Value<int>();
                this.AwayTeamScore = gameData["a"]["tot"]["g"].Value<int>();
                
                // After the first time the scores are set the goal horn is enabled
                if (this.suppressGoalHorn)
                {
                    this.suppressGoalHorn = false;
                }
            }
            catch (Exception) { /* Don't crash VS if any of the above lines throw, just try to poll again next period */ }

            if (this.IsGameOver)
            {
                this.OnNotifyPropertyChanged("IsGameOver");
                this.OnNotifyPropertyChanged("TimeDisplay");
                this.OnNotifyPropertyChanged("FinalText");
                this.IsGameLive = false;

                // Stop refreshing the game data if the game is over
                this.refreshDataTimer.Change(Timeout.Infinite, Timeout.Infinite);
            }
        }

        private async Task GoalHornAsync()
        {
            await Task.Yield();
            string originalBackgroundColor = this.BackgroundColor;
            for (int i = 0; i < 10; i++)
            {
                this.BackgroundColor = GoalBackgroundColor;
                await Task.Delay(200);

                // If the user changes their favourite team as a goal celebration is happening it could mess up the UI
                if (this.BackgroundColor != GoalBackgroundColor)
                {
                    originalBackgroundColor = BackgroundColor;
                }
                else
                {
                    this.BackgroundColor = originalBackgroundColor;
                }
                
                await Task.Delay(200);

                if (originalBackgroundColor != this.BackgroundColor)
                {
                    originalBackgroundColor = this.BackgroundColor;
                }
            }
        }

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnNotifyPropertyChanged(string propertyName)
        {
            if (this.PropertyChanged != null)
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
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
            this.refreshDataTimer.Change(Timeout.Infinite, Timeout.Infinite);
            this.refreshDataTimer.Dispose();
        }

        #endregion

    }
}
