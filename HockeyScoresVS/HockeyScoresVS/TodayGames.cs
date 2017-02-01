using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace HockeyScoresVS
{
    class TodayGames : ObservableCollection<HockeyGame>
    {
        public TodayGames()
        {
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

            var todayStringCode = GetTodayStringCode();
            var todaysGames = gameData.Where(x => x.est.Contains(todayStringCode));

            foreach(var game in todaysGames)
            {
                Add(new HockeyGame(game.est.Substring(todayStringCode.Length + 1), new Team(game.h), new Team(game.a), game.id));
            }
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
    }
}
