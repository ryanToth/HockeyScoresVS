using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.VisualStudio.Threading;
using Newtonsoft.Json;

namespace HockeyScoresVS
{
    public class TodayGames : ObservableCollection<HockeyGame>, INotifyPropertyChanged, IDisposable
    {
        public DateTime CurrentGamesDate = DateTime.Now.Date;
        private string cachedSeasonScheduleYears = string.Empty;

        public string BackButtonPath => Path.Combine(Utilities.ExecutingAssemblyDirectory, "Icons/chevron-left-8x.png");
        public string NextButtonPath => Path.Combine(Utilities.ExecutingAssemblyDirectory, "Icons/chevron-right-8x.png");

        private string favouriteTeam = string.Empty;
        public string FavouriteTeam
        {
            get
            {
                return this.favouriteTeam;
            }

            set
            {
                this.favouriteTeam = value;
                this.OrderGamesForStartTime();
                this.MoveFavouriteTeamGame();
            }
        }

        private bool isLoading;
        public bool IsLoading
        {
            get
            {
                return this.isLoading;
            }
            set
            {
                this.isLoading = value;
                this.OnNotifyPropertyChanged("IsLoading");
            }
        }

        private List<RawGameInfo> rawGameInfo;

        private object syncLock = new object();

        public TodayGames(string favouriteTeam)
        {
            lock(syncLock)
            {
                this.IsLoading = true;
                this.InitializeAsync(favouriteTeam).Forget();
            }
        }

        private async Task InitializeAsync(string favouriteTeam)
        {
            string todayStringCode = this.GetTodayStringCode();
            var todaysGames = await this.GetGameScheduleAsync(todayStringCode);
            this.favouriteTeam = favouriteTeam;

            List<HockeyGame> tempList = new List<HockeyGame>();

            foreach(var game in todaysGames)
            { 
                tempList.Add(new HockeyGame(this.ConvertRawDateToReadableString(game.est, todayStringCode), 
                    new Team(game.h), new Team(game.a), game.id, todayStringCode, this.cachedSeasonScheduleYears));
            }

            tempList.Sort();

            foreach(var game in tempList)
            {
                Add(game);
            }

            this.MoveFavouriteTeamGame();

            this.IsLoading = false;
            this.OnNotifyPropertyChanged("IsLoading");
            this.OnNotifyPropertyChanged("AnyGamesToday");
        }

        private async Task<IEnumerable<RawGameInfo>> GetGameScheduleAsync(string todayStringCode)
        {
            string currentSeasonScheduleYears = this.GetSeasonScheduleYear();
            if (this.rawGameInfo == null || currentSeasonScheduleYears != this.cachedSeasonScheduleYears)
            {
                this.cachedSeasonScheduleYears = currentSeasonScheduleYears;
                string jsonFile = await NetworkCalls.GetJsonFromApiAsync($"http://live.nhl.com/GameData/SeasonSchedule-{cachedSeasonScheduleYears}.json");

                this.rawGameInfo = JsonConvert.DeserializeObject<List<RawGameInfo>>(jsonFile);

                if (this.rawGameInfo == null)
                {
                    this.rawGameInfo = new List<RawGameInfo>();
                }
            }

            return this.rawGameInfo.Where(x => x.est.Contains(todayStringCode));
        }

        /// <summary>
        /// Converts raw time into a readable string in the format of hh:mm tt and converts Eastern Standard Time to local time
        /// </summary>
        /// <param name="rawDate">Date retrieved from the json from the API call</param>
        /// <param name="todayStringCode">Todays date in the format the API call uses</param>
        /// <returns></returns>
        private string ConvertRawDateToReadableString(string rawDate, string todayStringCode)
        {
            // All times from the API are in eastern standard time
            TimeZoneInfo tz = TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time");
            DateTime ESTTime = DateTime.Parse(rawDate.Substring(todayStringCode.Length + 1), CultureInfo.CurrentCulture);
            DateTime UTCTime = TimeZoneInfo.ConvertTimeToUtc(ESTTime, tz);

            return UTCTime.ToLocalTime().ToString("h:mm tt");
        }

        private string GetTodayStringCode()
        {
            var day = this.CurrentGamesDate.Day;
            var year = this.CurrentGamesDate.Year;
            var month = this.CurrentGamesDate.Month;

            string dateCode = year.ToString();
            if (month < 10) dateCode += $"0{month}";
            else dateCode += month.ToString();
            if (day < 10) dateCode += $"0{day}";
            else dateCode += day.ToString();

            return dateCode;
        }

        private string GetSeasonScheduleYear()
        {
            string years = string.Empty;

            // If we are getting games for after July 1st
            if (this.CurrentGamesDate.Month > 6 && this.CurrentGamesDate.Day > 1)
            {
                years = this.CurrentGamesDate.Year.ToString() + (this.CurrentGamesDate.Year + 1).ToString();
            }
            else
            {
                years = (this.CurrentGamesDate.Year - 1).ToString() + this.CurrentGamesDate.Year.ToString();
            }

            return years;
        }

        private void MoveFavouriteTeamGame()
        {
            if (this.FavouriteTeam != null)
            {
                var game = this.FirstOrDefault(x => x.HomeTeam.TeamCode == FavouriteTeam || x.AwayTeam.TeamCode == FavouriteTeam);
                if (game != null)
                {
                    this.MoveItem(this.IndexOf(game), 0);
                }
            }
        }

        private void OrderGamesForStartTime()
        {
            List<HockeyGame> games = new List<HockeyGame>(this.AsEnumerable());
            games.Sort();
            this.Clear();
            foreach (var game in games)
            {
                Add(game);
            }
        }

        public void ChangeGameDay(DateTime newDate)
        {
            lock (syncLock)
            {
                this.CurrentGamesDate = newDate.Date;
                this.Clear();
                this.IsLoading = true;
                this.InitializeAsync(FavouriteTeam).Forget();
            }
        }

        public bool AnyGamesToday
        {
            get
            {
                return this.Any();
            }
        }

        #region INotifyPropertyChanged Members

#pragma warning disable CS0114 // Member hides inherited member; missing override keyword
        public event PropertyChangedEventHandler PropertyChanged;
#pragma warning restore CS0114 // Member hides inherited member; missing override keyword

        private void OnNotifyPropertyChanged(string propertyName)
        {
            if (this.PropertyChanged != null)
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        #endregion

        public void Dispose()
        {
            foreach (var game in this)
            {
                game.Dispose();
            }
        }
    }
}
