using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
    public class TodayGames : ObservableCollection<HockeyGame>, INotifyPropertyChanged, IDisposable
    {
        private DateTime _currentGamesDate = DateTime.Now.Date;
        private string cachedSeasonScheduleYears = "";

        private List<RawGameInfo> _rawGameInfo;
        private List<RawGameInfo> RawGameInfo
        {
            get
            {
                string currentSeasonScheduleYears = GetSeasonScheduleYear();
                if (_rawGameInfo == null || currentSeasonScheduleYears != cachedSeasonScheduleYears)
                {
                    cachedSeasonScheduleYears = currentSeasonScheduleYears;
                    string jsonFile = NetworkCalls.GetJsonFromApi($"http://live.nhl.com/GameData/SeasonSchedule-{cachedSeasonScheduleYears}.json");

                    _rawGameInfo = JsonConvert.DeserializeObject<List<RawGameInfo>>(jsonFile);
                }

                return _rawGameInfo;
            }
        }
        //private Timer _timer;
        // Once every hour
        //private int pollingInterval = 3600000;
        private object _lock = new object();

        public TodayGames()
        {
            lock(_lock)
            {
                //this._timer = new Timer(CheckForTomorrow, new AutoResetEvent(true), 0, pollingInterval);
                this.Initialize();
            }
        }

        private void Initialize()
        {
            var todayStringCode = GetTodayStringCode();
            var todaysGames = RawGameInfo.Where(x => x.est.Contains(todayStringCode));

            List<HockeyGame> tempList = new List<HockeyGame>();

            foreach(var game in todaysGames)
            { 
                tempList.Add(new HockeyGame(ConvertRawDateToReadableString(game.est, todayStringCode), 
                    new Team(game.h), new Team(game.a), game.id, todayStringCode, cachedSeasonScheduleYears));
            }

            tempList.Sort();

            foreach(var game in tempList)
            {
                Add(game);
            }

            OnNotifyPropertyChanged("AnyGamesToday");
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
            var day = _currentGamesDate.Day;
            var year = _currentGamesDate.Year;
            var month = _currentGamesDate.Month;

            string dateCode = year.ToString();
            if (month < 10) dateCode += $"0{month}";
            else dateCode += month.ToString();
            if (day < 10) dateCode += $"0{day}";
            else dateCode += day.ToString();

            return dateCode;
        }

        private string GetSeasonScheduleYear()
        {
            string years = "";

            // If we are getting games for after July 1st
            if (_currentGamesDate.Month > 6 && _currentGamesDate.Day > 1)
            {
                years = _currentGamesDate.Year.ToString() + (_currentGamesDate.Year + 1).ToString();
            }
            else
            {
                years = (_currentGamesDate.Year - 1).ToString() + (_currentGamesDate.Year).ToString();
            }

            return years;
        }

        /*
        private void CheckForTomorrow(object state)
        {
            lock(_lock)
            {
                // Re-initialize the game data if it's tomorrow
                if (_currentGamesDate < DateTime.Now.Date)
                {
                    this.Clear();
                    this.Initialize();
                }
            }
        }
        */

        public void ChangeGameDay(DateTime newDate)
        {
            lock (_lock)
            {
                _currentGamesDate = newDate.Date;
                this.Clear();
                this.Initialize();
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

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnNotifyPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        #endregion

        public void Dispose()
        {
            //_timer.Change(Timeout.Infinite, Timeout.Infinite);
            //_timer.Dispose();

            foreach (var game in this)
            {
                game.Dispose();
            }
        }
    }
}
