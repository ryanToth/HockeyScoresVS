using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HockeyScoresVS
{
    public class GameGoals : INotifyPropertyChanged
    {
        public ObservableCollection<Goal> FirstPeriodGoals { get; }
        public ObservableCollection<Goal> SecondPeriodGoals { get; }
        public ObservableCollection<Goal> ThirdPeriodGoals { get; }
        public ObservableCollection<Goal> OTGoals { get; }

        private string _seasonCode;
        private string _gameCode;

        public GameGoals(string seasonCode, string gameCode)
        {
            this._seasonCode = seasonCode;
            this._gameCode = gameCode;

            FirstPeriodGoals = new ObservableCollection<Goal>();
            SecondPeriodGoals = new ObservableCollection<Goal>();
            ThirdPeriodGoals = new ObservableCollection<Goal>();
            OTGoals = new ObservableCollection<Goal>();
        }

        public bool AnyGoalsScored
        {
            get
            {
                return FirstPeriodGoals.Any() || SecondPeriodGoals.Any() || ThirdPeriodGoals.Any() || OTGoals.Any();
            }
        }

        public void Clear()
        {
            FirstPeriodGoals.Clear();
            SecondPeriodGoals.Clear();
            ThirdPeriodGoals.Clear();
            OTGoals.Clear();
        }

        public void RefreshGoalSummary(List<List<Goal>> updatedScoringSummary)
        {
            this.Clear();

            foreach (var goal in updatedScoringSummary[0])
            {
                this.FirstPeriodGoals.Add(goal);
            }
            foreach (var goal in updatedScoringSummary[1])
            {
                this.SecondPeriodGoals.Add(goal);
            }
            foreach (var goal in updatedScoringSummary[2])
            {
                this.ThirdPeriodGoals.Add(goal);
            }
            foreach (var goal in updatedScoringSummary[3])
            {
                this.OTGoals.Add(goal);
            }

            OnNotifyPropertyChanged("AnyGoalsScored");
        }

        public async Task GetUpdateScoringSummary()
        {
            List<Goal> tempFirstPeriodGoals = new List<Goal>();
            List<Goal> tempSecondPeriodGoals = new List<Goal>();
            List<Goal> tempThirdPeriodGoals = new List<Goal>();
            List<Goal> tempOTGoals = new List<Goal>();

            JObject gameData = await NetworkCalls.ApiCallAsync($"http://live.nhl.com/GameData/{_seasonCode}/{_gameCode}/gc/gcbx.jsonp");

            var goals = gameData["goalSummary"].Values();
            int i = 1;
            foreach (var goalsList in goals)
            {
                foreach (var goal in goalsList.First())
                {
                    string goalString = "";
                    string team = "";

                    try
                    {
                        goalString = goal["desc"].Value<string>();
                        team = goal["t1"].Value<string>();
                    }
                    catch (Exception) { }

                    if (i == 1)
                    {
                        tempFirstPeriodGoals.Add(new Goal(team, goalString));
                    }
                    if (i == 3)
                    {
                        tempSecondPeriodGoals.Add(new Goal(team, goalString));
                    }
                    if (i == 5)
                    {
                        tempThirdPeriodGoals.Add(new Goal(team, goalString));
                    }
                    if (i == 7)
                    {
                        tempOTGoals.Add(new Goal(team, goalString));
                    }
                }

                i++;
            }

            var list = new List<List<Goal>>() { tempFirstPeriodGoals, tempSecondPeriodGoals, tempThirdPeriodGoals, tempOTGoals };
            this.RefreshGoalSummary(list);
        }

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        public void OnNotifyPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        #endregion
    }
}
