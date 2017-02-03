using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace HockeyScoresVS
{
    class TodayGames : ObservableCollection<HockeyGame>
    {
        private DateTime _currentGamesDate;
        private Timer _timer;
        // Once every hour
        private int pollingInterval = 3600000;

        public TodayGames()
        {
            this._timer = new Timer(CheckForTomorrow, new AutoResetEvent(true), pollingInterval, pollingInterval);
            this.Initialize();
        }

        private void Initialize()
        {
            WebRequest request = WebRequest.Create("http://live.nhl.com/GameData/SeasonSchedule-20162017.json");
            WebResponse response = request.GetResponse();

            Stream dataStream = response.GetResponseStream();
            StreamReader reader = new StreamReader(dataStream);
            
            string jsonFile = reader.ReadToEnd();
            reader.Close();
            response.Close();

            var gameData = JsonConvert.DeserializeObject<List<RawGameInfo>>(jsonFile);

            _currentGamesDate = DateTime.Now.Date;
            var todayStringCode = GetTodayStringCode();
            var todaysGames = gameData.Where(x => x.est.Contains(todayStringCode));

            List<HockeyGame> tempList = new List<HockeyGame>();

            foreach(var game in todaysGames)
            { 
                tempList.Add(new HockeyGame(ConvertRawDateToReadableString(game.est, todayStringCode), 
                    new Team(game.h), new Team(game.a), game.id, todayStringCode));
            }

            tempList.Sort();

            foreach(var game in tempList)
            {
                Add(game);
            }
        }

        /// <summary>
        /// Converts raw time into a readable string in the format of hh:mm tt and converts Eastern to Pacific time
        /// </summary>
        /// <param name="rawDate">Date retrieved from the json from the API call</param>
        /// <param name="todayStringCode">Todays date in the format the API call uses</param>
        /// <returns></returns>
        private string ConvertRawDateToReadableString(string rawDate, string todayStringCode)
        {
            return DateTime.Parse(rawDate.Substring(todayStringCode.Length + 1), CultureInfo.CurrentCulture).Subtract(new TimeSpan(0, 3, 0, 0)).ToString("h:mm tt");
        }

        private string GetTodayStringCode()
        {
            var day = DateTime.Now.Day;
            var year = DateTime.Now.Year;
            var month = DateTime.Now.Month;

            string dateCode = year.ToString();
            if (month < 10) dateCode += $"0{month}";
            else dateCode += month.ToString();
            if (day < 10) dateCode += $"0{day}";
            else dateCode += day.ToString();

            return dateCode;
        }

        private void CheckForTomorrow(object state)
        {
            // Re-initialize the game data if it's tomorrow
            if (_currentGamesDate < DateTime.Now.Date)
            {
                this.Clear();
                this.Initialize();
            }
        }
    }
}
